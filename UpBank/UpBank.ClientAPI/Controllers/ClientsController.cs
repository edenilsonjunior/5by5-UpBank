using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Bank;
using Models.DTO;
using Models.People;
using Newtonsoft.Json;
using Services.People;
using UpBank.ClientAPI.Data;

namespace UpBank.ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly UpBankClientAPIContext _context;
        private readonly ClientService _clientService;

        public ClientsController(UpBankClientAPIContext context, ClientService clientService)
        {
            _context = context;
            _clientService = clientService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClient()
        {
            List<Client> clientsList = new List<Client>();

            var Clients = await _context.Clients.ToListAsync();
            foreach (var dto in Clients)
            {
                Client clientResult = new(dto);
                clientResult.Restriction = dto.Restriction;
                clientResult.Address = await _clientService.GetAddressById(dto.AddressId);

                clientsList.Add(clientResult);
            }
            return clientsList;
        }


        [HttpGet("{cpf}")]
        public async Task<ActionResult<Client>> GetClient(string cpf)
        {
            var list = await _context.Clients.ToListAsync();

            var clientDTO = list.FirstOrDefault(c => c.CPF.Equals(cpf));

            if (clientDTO == null)
            {
                return NotFound("CPF não encontrato no banco de dados");
            }

            Client clientResult = new(clientDTO);
            clientResult.Restriction = clientDTO.Restriction;
            clientResult.Address = await _clientService.GetAddressById(clientDTO.AddressId);

            return Ok(clientResult);
        }


        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(ClientDTOPost clientDTOPost)
        {
            if (!ValidateCPF(clientDTOPost.CPF))
            {
                return BadRequest("CPF Invalido");
            }
            if (_context.Clients == null)
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


            if (clientDTOPost.Salary <= 0)
            {
                return BadRequest("Salário inválido.");
            }

            if (clientDTOPost.BirthDt.AddYears(16) > DateTime.Now)
            {
                return BadRequest("O cliente deve ter no mínimo 16 anos.");
            }


            Address address = await _clientService.PostAddress(clientDTOPost.Address);

            clientDTOPost.CPF = new string(clientDTOPost.CPF.Where(char.IsDigit).ToArray());

            Client client = new()
            {
                Name = clientDTOPost.Name,
                CPF = clientDTOPost.CPF,
                BirthDt = clientDTOPost.BirthDt,
                Sex = clientDTOPost.Sex,
                Address = address,
                Salary = clientDTOPost.Salary,
                Phone = clientDTOPost.Phone,
                Email = clientDTOPost.Email,
                Restriction = false
            };

            ClientDTO dto = new()
            {
                Name = clientDTOPost.Name,
                CPF = clientDTOPost.CPF,
                BirthDt = clientDTOPost.BirthDt,
                Sex = clientDTOPost.Sex,
                AddressId = address.Id,
                Salary = clientDTOPost.Salary,
                Phone = clientDTOPost.Phone,
                Email = clientDTOPost.Email,
            };

            _context.Clients.Add(dto);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(client);

            }
            catch (Exception)
            {
                return BadRequest("Não foi possível adicionar o cliente. Por favor, tente novamente.");
            }
        }


        [HttpPut]
        public async Task<ActionResult<Client>> PutClient(ClientUpdateDTO clientUpdateDTO)
        {

            ClientDTO client = await _context.Clients.FirstOrDefaultAsync(c => c.CPF.Equals(clientUpdateDTO.CPF));
            if (client == null)
            {
                return NotFound("Cliente não encontrado.");
            }


            client.Salary = clientUpdateDTO.Salary;
            client.Phone = clientUpdateDTO.Phone;
            client.Email = clientUpdateDTO.Email;
            client.Restriction = clientUpdateDTO.Restriction;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Não foi possível atualizar o cliente. Por favor, tente novamente.");
            }

            return await GetClient(client.CPF);
        }


        [HttpDelete("{cpf}")]
        public async Task<IActionResult> DeleteClient(string cpf)
        {
            var accounts = await GetAllAccounts();

            if (accounts != null)
            {
                foreach (var account in accounts)
                {
                    if (account.Client.Find(c => c.CPF.Equals(cpf)) != null)
                    {
                        return BadRequest("Não é possível excluir um cliente com conta ativa.");
                    }
                }
            }


            var list = await _context.Clients.ToListAsync();

            var clientToBeCancelled = list.FirstOrDefault(c => c.CPF.Equals(cpf));


            if (_context.Clients == null || _context.DeletedClient == null)
            {
                return NotFound("Contexto de clientes não encontrado");
            }

            if (clientToBeCancelled == null)
            {
                return NotFound("Cliente não encontrado.");
            }


            clientToBeCancelled.Restriction = true;
            _context.Entry(clientToBeCancelled).State = EntityState.Modified;

            // Cria nova entrada para a tabela de clientes cancelados
            var clientCancelled = new ClientCancelled
            {
                Name = clientToBeCancelled.Name,
                CPF = clientToBeCancelled.CPF,
                BirthDt = clientToBeCancelled.BirthDt,
                Sex = clientToBeCancelled.Sex,
                AddressId = clientToBeCancelled.AddressId,
                Salary = clientToBeCancelled.Salary,
                Phone = clientToBeCancelled.Phone,
                Email = clientToBeCancelled.Email,
                Restriction = clientToBeCancelled.Restriction
            };

            _context.DeletedClient.Add(clientCancelled);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest("Não foi possível cancelar o cliente. Por favor, tente novamente.");
            }

            return Ok("Cliente movido para a tabela de clientes cancelados e Restricao mudado para true.");
        }


        private bool ClientExists(string cpf)
        {
            return _context.Clients.Any(e => e.CPF.Equals(cpf));
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

        public async Task<List<Account>> GetAllAccounts()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync("https://localhost:7011/api/Accounts");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Account>>(content);
                }
                return null;
            }
        }
    }
}