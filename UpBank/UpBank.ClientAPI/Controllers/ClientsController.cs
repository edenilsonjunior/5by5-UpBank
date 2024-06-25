using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Bank;
using Models.People;
using UpBank.ClientAPI.Data;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace UpBank.ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly UpBankClientAPIContext _context;

        public ClientsController(UpBankClientAPIContext context)
        {
            _context = context;
            Console.WriteLine("Contexto inicializado com sucesso!");
        }

        //_________________________________________________________________________________________________
        //O primeiro método lista todos os clientes por CPF
        //O segundo método  busca e retorna um único cliente com base no CPF fornecido.
        // GET: api/Clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClients()// o Task/async siguinifica que é uma tarefa assincrona( pode ser executada em paralelo com outras tarefas)
        {
            if (_context.Client == null)
            {
                return NotFound();
            }
            return await _context.Client.Include(c => c.Address).ToListAsync();
        }

        // GET: api/Clients/5 
        [HttpGet("{cpf}")]
        public async Task<ActionResult<Client>> GetClient(string cpf)
        {
            if (_context.Client == null)
            {
                return NotFound();
            }
            var client = await _context.Client.FindAsync(cpf);

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }
        //_________________________________________________________________________________________________


        //O método PutClient atualiza as informações d um cliente pelo CPF e retorna mensagen se o CPF não corresponder ou se ocorrerem problemas durante a atualização.
        // PUT: api/Clients/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(string id, Client client)
        {
            if (id != client.CPF)
            {
                return BadRequest();
            }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // O [HttpPost("AddClientToAccount")] tenta adicionar um novo cliente a uma conta existente e responde com base no resultado da tentativa, confirmando o sucesso, informando q a conta não foi encontrada ou indicando que houve um erro com a solicitação.
      
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        
        [HttpPost("AddClientToAccount")]
        public async Task<ActionResult> AddClientToAccount(string accountId, [FromBody] Client newClient)
        {
            try
            {
                bool result = await AddClientToAccountInternal(accountId, newClient);
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return NotFound("Conta não encontrada");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private async Task<bool> AddClientToAccountInternal(string accountId, Client newClient)
        {
            // Tenta encontrar a conta pelo accountId, incluindo os clientes associados a ela.
            var account = await _context.Accounts
                .Include(a => a.Clients)
                .FirstOrDefaultAsync(n => n.Number == accountId);

            // Verifica se a conta foi encontrada.
            if (account == null) return false;

            // Calcula a idade do cliente.
            int clientAge = DateTime.Now.Year - newClient.DateOfBirth.Year;
            // Ajusta a idade se o aniversário ainda não ocorreu este ano.
            if (newClient.DateOfBirth > DateTime.Now.AddYears(-clientAge)) clientAge--;

            // Verifica se o cliente é menor de 18 anos.
            if (clientAge < 18)
            {
                // Verifica se existe pelo menos um adulto na conta.
                bool hasAdult = account.Clients.Any(c =>
                    {
                        int age = DateTime.Now.Year - c.DateOfBirth.Year;
                        // Ajusta a idade se o aniversário ainda não ocorreu este ano.
                        if (c.DateOfBirth > DateTime.Now.AddYears(-age)) age--;
                        return age >= 18;
                    });

                // Em ñ havendo um responsavel legal ele trás a exceção 
                if (!hasAdult)
                {
                    throw new Exception("Clientes menores de 18 anos só podem ter conta conjunta com seu responsável legal.");
                }
            }
            // Salva as alterações no contexto.
            await _context.SaveChangesAsync();

            return true;
        }

        // POST: api/Clients
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            if (_context.Client == null)
            {
                return Problem("Entity set 'UpBankClientAPIContext.Client' is null.");
            }

            if (client.DateOfBirth == null)
            {
                return BadRequest("A data de nascimento é obrigatória.");
            }

            // Verifica se o CPF do cliente já existe no banco de dados.
            if (ClientExists(client.CPF))
            {
                return Conflict("Já existe um cliente com este CPF.");
            }

            _context.Client.Add(client);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Não foi possível adicionar o cliente. Por favor, tente novamente.");
            }

            // Retorna o cliente criado usando o CPF como identificador no retorno.
            return CreatedAtAction("GetClient", new { cpf = client.CPF }, client);
        }

        // PATCH: api/Clients/AddRestriction/{cpf}
        [HttpPatch("AddRestriction/{cpf}")]
        public async Task<IActionResult> AddRestriction(string id)
        {
            var client = await _context.Client.FirstOrDefaultAsync(c => c.CPF == id);
            if (client == null)
            {
                return NotFound("Cliente não encontrado.");
            }

            client.Restriction = true;
            await _context.SaveChangesAsync();

            return Ok("Restrição adicionada com sucesso.");
        }

        // PATCH: api/Clients/RemoveRestriction/{cpf}
        [HttpPatch("RemoveRestriction/{cpf}")]
        public async Task<IActionResult> RemoveRestriction(string id)
        {
            var client = await _context.Client.FirstOrDefaultAsync(c => c.CPF == id);
            if (client == null)
            {
                return NotFound("Cliente não encontrado.");
            }

            client.Restriction = false;
            await _context.SaveChangesAsync();

            return Ok("Restrição removida com sucesso.");
        }

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(string id)
        {
            if (_context.Client == null)
            {
                return NotFound();
            }
            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Client.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(string id)
        {
            return _context.Client.Any(e => e.CPF == id);
        }
    }
}



   

