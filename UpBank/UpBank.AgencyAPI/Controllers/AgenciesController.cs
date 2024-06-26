using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models.Bank;
using Models.DTO;
using Models.People;
using Services.Agencies;
using UpBank.AgencyAPI.Data;
using UpBank.AgencyAPI.Utils;

namespace UpBank.AgencyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgenciesController : ControllerBase
    {
        private readonly UpBankAgencyAPIContext _context;
        private readonly AgenciesService _agenciesService;

        public AgenciesController(UpBankAgencyAPIContext context, AgenciesService agenciesService)
        {
            _context = context;
            _agenciesService = agenciesService;
        }

        // GET: api/Agencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Agency>>> GetAgency()
        {
            List<Agency> agencies = new();
            var agenciesDto = await _context.Agency.ToListAsync();
            if (agenciesDto == null || agenciesDto.Count == 0) return NotFound("Nenhuma agência encontrada.");
            foreach (var agencyDto in agenciesDto)
            {
                List<Employee> employees = new();
                foreach (var employee in agencyDto.Employees)
                {
                    employees.Add(new Employee
                    {
                        Registry = employee.Registry,
                        CPF = employee.CPF,
                        Name = employee.Name,
                        BirthDt = employee.BirthDt,
                        Sex = employee.Sex,
                        Address = await _agenciesService.GetAddressById(employee.AddressId),
                        Salary = employee.Salary,
                        Phone = employee.Phone,
                        Email = employee.Email,
                        Manager = employee.Manager
                    });
                }

                agencies.Add(new Agency
                {
                    Number = agencyDto.Number,
                    Address = await _agenciesService.GetAddressById(agencyDto.AddressId),
                    CNPJ = agencyDto.CNPJ,
                    Employees = employees,
                    Restriction = agencyDto.Restriction
                });
            }

            return Ok(agencies);
        }


        // GET: api/Agencies/{number}
        [HttpGet("{number}")]
        public async Task<ActionResult<Agency>> GetAgency(string number)
        {
            Agency agency = null;
            List<Employee> employees = new();
            var agencyDto = await _context.Agency.FindAsync(number);
            if (agencyDto == null) return NotFound("Agência não existe ou o número informado não corresponde á agência desejada.");

            foreach (var employee in agencyDto.Employees) employees.Add(new Employee
            {
                Registry = employee.Registry,
                CPF = employee.CPF,
                Name = employee.Name,
                BirthDt = employee.BirthDt,
                Sex = employee.Sex,
                Address = await _agenciesService.GetAddressById(employee.AddressId),
                Salary = employee.Salary,
                Phone = employee.Phone,
                Email = employee.Email,
                Manager = employee.Manager
            });

            agency = new Agency
            {
                Number = agencyDto.Number,
                Address = await _agenciesService.GetAddressById(agencyDto.AddressId),
                CNPJ = agencyDto.CNPJ,
                Employees = employees,
                Restriction = agencyDto.Restriction
            };

            return Ok(agency);
        }

        // GET: api/Agencies/restrict
        [HttpGet("restrict/{agencyNumber}")]
        public async Task<ActionResult<List<Account>>> GetRestrictAccounts(string? agencyNumber)
        {
            List<Account> accounts = null;
            if (agencyNumber == null)
            {
                accounts = _agenciesService.GetAllAccounts().Result.Where(a => a.Restriction == true).ToList();
                if (accounts == null) return NotFound("Nenhuma conta restrita encontrada.");
                return Ok(accounts);
            }

            accounts = _agenciesService.GetAllAccounts().Result.Where(a => a.Restriction == true && a.Agency.Number == agencyNumber).ToList();
            if (accounts == null) return NotFound("Nenhuma conta restrita encontrada.");
            return Ok(accounts);
        }

        // GET: api/Agencies/profile
        [HttpGet("profile/{profile}/{agencyNumber}")]
        public async Task<ActionResult<List<Account>>> GetAccountsByProfile(string profile, string? agencyNumber)
        {
            List<Account> accounts = null;

            if (profile == null) return BadRequest("O perfil da conta é obrigatório.");

            if (agencyNumber == null)
            {
                accounts = _agenciesService.GetAllAccounts().Result.Where(a => a.Profile.ToString() == profile).ToList();
                if (accounts == null) return NotFound("Nenhuma conta encontrada.");
                return Ok(accounts);
            }

            accounts = _agenciesService.GetAllAccounts().Result.Where(a => a.Profile.ToString() == profile && a.Agency.Number == agencyNumber).ToList();
            if (accounts == null) return NotFound("Nenhuma conta encontrada.");
            return Ok(accounts);
        }



        // GET: api/Agencies/lending
        [HttpGet("lending")]
        public async Task<ActionResult<List<Account>>> GetLendingAccounts()
        {
            List<Account> accounts = _agenciesService.GetAllAccounts().Result.Where(a => a.Extract.Any(t => t.Type == ETransactionType.Lending)).ToList();
            if (accounts == null || accounts.Count == 0) return NotFound("Nenhuma conta com empréstimo ativo encontrada.");
            return Ok(accounts);
        }


        // PUT: api/Agencies/5
        [HttpPut("{number}")]
        public async Task<ActionResult<Agency>> PutAgency(string number, AgencyDTO agency)
        {
            var agencyFromBank = await _context.Agency.FindAsync(number);
            if (agencyFromBank == null) return NotFound("Número da agência informado não corresponde a uma agência cadastrada.");

            if (!agencyFromBank.CNPJ.Equals(agency.CNPJ))
                return BadRequest("O cnpj informado nao condiz com a agencia buscada");


            if (agency.Restriction == agencyFromBank.Restriction)
            {
                string status = agencyFromBank.Restriction ? "RESTRINGIDA" : "LIBERADA";
                return BadRequest($"A agência já está {status}.");
            }
            else agencyFromBank.Restriction = agency.Restriction.GetValueOrDefault();

            var employees = await _agenciesService.GetAllEmployees();

            if (agency.Employees != null || agency.Employees.Count > 0)
            {
                foreach (var registry in agency.Employees)
                {
                    var employee = employees.FirstOrDefault(e => e.Registry == registry);
                    if (employee == null) return BadRequest($"Registro {registry} não foi encontrado no banco de dados.");
                    if (agencyFromBank.Employees.Any(e => e.Registry == registry)) return BadRequest($"Registro {registry} já está registrado na agência.");
                    agencyFromBank.Employees.Add(new EmployeeDTOEntity
                    {
                        Name = employee.Name,
                        CPF = employee.CPF,
                        BirthDt = employee.BirthDt,
                        Sex = employee.Sex,
                        AddressId = employee.Address.Id,
                        Salary = employee.Salary,
                        Phone = employee.Phone,
                        Email = employee.Email,
                        Manager = employee.Manager,
                        Registry = employee.Registry,
                        AgencyNumber = agencyFromBank.Number
                    });
                }
            }

            if (agency.AddressId != null)
            {
                var address = await _agenciesService.GetAddressById(agency.AddressId);
                if (address == null) return NotFound("Id de endereço fornecido não foi encontrado no banco de dados.");

                if (agencyFromBank.AddressId != agency.AddressId)
                {
                    agencyFromBank.Address = address;
                    agencyFromBank.AddressId = agency.AddressId;
                }
            }


            _context.Entry(agencyFromBank).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (Exception) { throw; }


            return await GetAgency(agencyFromBank.Number);
        }

        // POST: api/Agencies
        [HttpPost]
        public async Task<ActionResult<EmployeeDTOEntity>> PostAgency(AgencyDTO agency)
        {
            if (!CnpjValidator.IsValid(agency.CNPJ)) return BadRequest("CNPJ inválido.");

            if (agency.Employees.Count == 0) return BadRequest("A agência deve ter ao menos um funcionário e um deles precisa ser um gerente.");


            string newAgencyNumber;
            bool agencyExists;
            do
            {
                newAgencyNumber = "";
                Random random = new Random();
                for (int i = 0; i < 4; i++) newAgencyNumber += random.Next(0, 10).ToString();
                agencyExists = await AgencyExistsAsync(newAgencyNumber);
            }
            while (agencyExists);

            AgencyDTOEntity newAgency = null;
            try { newAgency = await _agenciesService.AgencyDTOEntityBuilder(agency, newAgencyNumber); }
            catch (Exception e) { return BadRequest(e.Message); }

            var result = _context.Agency.Add(newAgency);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest("Nao foi possivel inserir a agencia. Tente novamente.");
            }

            return Ok(newAgency);
        }

        // DELETE: api/Agencies/5
        [HttpDelete("{number}")]
        public async Task<ActionResult<Agency>> DeleteAgency(string number)
        {
            var agency = await _context.Agency.FindAsync(number);
            if (agency == null) return NotFound("Número da agência informado não corresponde a uma agência cadastrada.");

            List<Account> accounts = await _agenciesService.GetAllAccounts();

            if(accounts.Find(accounts => accounts.Agency.Number == number) != null) 
                return BadRequest("Essa agência nao pode ser deletada pois possui contas vinculadas a ela.");


            agency.Restriction = true;
            _context.Agency.Update(agency);
            await _context.SaveChangesAsync();

            var parametersAgency = new[] {
                    new SqlParameter("@Number", agency.Number),
                    new SqlParameter("@AddressId", agency.AddressId),
                    new SqlParameter("@CNPJ", agency.CNPJ),
                    new SqlParameter("@Restriction", agency.Restriction)
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(Agency.INSERT, parametersAgency);
            }
            catch (Exception)
            {
                return BadRequest("Essa agencia ja consta como deletada.");
            }


            foreach (var employee in agency.Employees)
            {
                var parametersEmployee = new[] {
                    new SqlParameter("@CPF", employee.CPF),
                    new SqlParameter("@Name", employee.Name),
                    new SqlParameter("@BirthDt", employee.BirthDt),
                    new SqlParameter("@Sex", employee.Sex),
                    new SqlParameter("@AddressId", employee.AddressId),
                    new SqlParameter("@Salary", employee.Salary),
                    new SqlParameter("@Phone", employee.Phone),
                    new SqlParameter("@Email", employee.Email),
                    new SqlParameter("@Manager", employee.Manager),
                    new SqlParameter("@Registry", employee.Registry),
                    new SqlParameter("@AgencyNumber", employee.AgencyNumber)
                };

                try
                {
                    await _context.Database.ExecuteSqlRawAsync(Employee.INSERT, parametersEmployee);
                }
                catch (Exception)
                {
                    return BadRequest("Esse funcionario ja consta como deletado na agencia.");
                }
            }

            return await GetAgency(agency.Number);
        }

        private async Task<bool> AgencyExistsAsync(string number) => await Task.Run(() => (_context.Agency?.Any(e => e.Number == number)).GetValueOrDefault());
    }
}
