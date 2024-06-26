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
using Models.DTO;
using Models.People;
using Services.People;
using UpBank.ClientAPI.Data;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace UpBank.ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly UpBankClientAPIContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _extenalAPIUrl;
        private readonly ClientService _clientService;

        public ClientsController(UpBankClientAPIContext context, HttpClient httpClient, IConfiguration configuration, ClientService clientService)
        {
            _context = context;
            _httpClient = httpClient;
            _extenalAPIUrl = configuration.GetValue<string>("https://localhost:7084/api/Addresses");
            _clientService = clientService;
            Console.WriteLine("Contexto inicializado com sucesso");
        }

        //_________________________________________________________________________________________________


        // GET: api/Clients
        [HttpGet] //Trás lista de todos os clientes, incluindo os endereços associados a cada cliente.
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClient()// o Task/async siguinifica que é uma tarefa assincrona( pode ser executada em paralelo com outras tarefas)
        {
            List<Client> clientsList = new List<Client>();
            
            var Clients = await _context.Clients.ToListAsync();
            foreach (var Client in Clients)
            {
                Client clientResult = new Client()
                {
                    CPF = Client.CPF,
                    Name = Client.Name,
                    BirthDt = Client.BirthDt,
                    Sex = Client.Sex,
                    Address = await _clientService.GetAddressById(Client.AddressId),
                    Salary = Client.Salary,
                    Phone = Client.Phone,
                    Email = Client.Email
                };
               clientsList.Add(clientResult);
            }
            return clientsList;
        }

        // GET: api/Clients/5 
        [HttpGet("{cpf}")] //busca e retorna um único cliente com base no CPF fornecido.
        public async Task<ActionResult<Client>> GetClient(string cpf)
        {
            var client = await _context.Clients.FindAsync(cpf);
            if (client == null)
            {
                return NotFound("CPF não encontrato no banco de dados");
            }

            Client clientResult = new Client()
            {
                CPF = client.CPF,
                Name = client.Name,
                BirthDt = client.BirthDt,
                Sex = client.Sex,
                Address = await _clientService.GetAddressById(client.AddressId),
                Salary = client.Salary,
                Phone = client.Phone,
                Email = client.Email
            };
            return Ok (clientResult);
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
        public async Task<ActionResult<Client>> PostClient(ClientDTOPost clientDTOPost)
        {
            if (!ValidateCPF(clientDTOPost.CPF))
            {
                return BadRequest("CPF Invalido");
            }
            if (_context.Client == null)
            {
                return Problem("Entity set 'UpBankClientAPIContext.Client' is null.");
            }

            if (clientDTOPost.BirthDt == null)
            {
                return BadRequest("A data de nascimento é obrigatória.");
            }

            // Verifica se o CPF do cliente já existe no banco de dados.
            if (ClientExists(clientDTOPost.CPF))
            {
                return Conflict("Já existe um cliente com este CPF.");
            }

            Address address = await _clientService.PostAddress(clientDTOPost.AddressDTO);

            ClientDTO clientDTO = new ClientDTO()
            {
                CPF = clientDTOPost.CPF,
                Name = clientDTOPost.Name,
                BirthDt = clientDTOPost.BirthDt,
                Sex = clientDTOPost.Sex,
                AddressId = address.Id,
                Salary = clientDTOPost.Salary,
                Phone = clientDTOPost.Phone,
                Email = clientDTOPost.Email
            };


            _context.Clients.Add(clientDTO);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Não foi possível adicionar o cliente. Por favor, tente novamente.");
            }

            // Retorna o cliente criado usando o CPF como identificador no retorno.
            return Ok (clientDTO);
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
            if (_context.Client == null || _context.ClientCancelled == null)
            {
                return NotFound("Contexto de clientes não encontrado");
            }

            // Busca cliente original pelo ID
            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound("Cliente não encontrado");
            }

            // Cria nova entrada para a tabela de clientes cancelados
            var clientCancelled = new ClientCancelled
            {
                Name = client.Name,
                CPF = client.CPF,
                BirthDt = client.BirthDt,
                Sex = client.Sex,
                Address = client.Address,
                Salary = client.Salary,
                Phone = client.Phone,
                Email = client.Email

            };

            // Add cliente cancelado na tabela d clientes cancelados
            _context.ClientCancelled.Add(clientCancelled);

            // Remove cliente original do contexto
            _context.Client.Remove(client);

            await _context.SaveChangesAsync();

            return Ok("Cliente movido para a tabela de clientes cancelados e excluído com sucesso.");
        }

        private bool ClientExists(string id)
        {
            return _context.Client.Any(e => e.CPF == id);
        }

        public static bool ValidateCPF(string cpf)
        {
            // Removendo caracteres não numéricos do CPF
            cpf = new string(cpf.Where(char.IsDigit).ToArray());
            // Verificando se o CPF possui 11 dígitos
            if (cpf.Length != 11)
            {
                return false;
            }
            // Verificando se todos os dígitos são iguais
            if (cpf.Distinct().Count() == 1)
            {
                return false;
            }
            // Calculando o primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (10 - i);
            }
            int resto = soma % 11;
            int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;
            // Verificando o primeiro dígito verificador
            if (int.Parse(cpf[9].ToString()) != digitoVerificador1)
            {
                return false;
            }
            // Calculando o segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (11 - i);
            }
            resto = soma % 11;
            int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;
            // Verificando o segundo dígito verificador
            if (int.Parse(cpf[10].ToString()) != digitoVerificador2)
            {
                return false;
            }
            return true;
        }
    }


}





