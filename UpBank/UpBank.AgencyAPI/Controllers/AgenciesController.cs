using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Bank;
using Models.DTO;
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
            var agencies = await _context.Agency.ToListAsync();
            if (agencies == null || agencies.Count == 0) return NotFound("Nenhuma agência encontrada.");
            foreach (var agency in agencies) agency.Address = await _agenciesService.GetAddressById(agency.AddressId);
            return Ok(agencies);
        }

        // GET: api/Agencies/{number}
        [HttpGet("{number}")]
        public async Task<ActionResult<Agency>> GetAgency(string number)
        {
            var agency = await _context.Agency.FindAsync(number);
            if (agency == null) return NotFound("Agência não existe ou o número informado não corresponde á agência desejada.");
            agency.Address = await _agenciesService.GetAddressById(agency.AddressId);
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
        [HttpGet("/lending")]
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

            if (!CnpjValidator.IsValid(agency.CNPJ)) return BadRequest("CNPJ inválido.");

            var address = await _agenciesService.GetAddressById(agency.AddressId);
            if (address == null) return NotFound("Id de endereço fornecido não foi encontrado no banco de dados.");

            
            if (agency.Restriction == agencyFromBank.Restriction)
            {
                string status = agencyFromBank.Restriction ? "RESTRINGIDA" : "LIBERADA";
                return BadRequest($"A agência já está {status}.");
            }
            else agencyFromBank.Restriction = agency.Restriction.GetValueOrDefault();

            var employees = await _agenciesService.GetAllEmployees();
            
            foreach (var CPF in agency.Employees)
            {
                var employee = employees.FirstOrDefault(e => e.CPF == CPF);
                if (employee == null) return BadRequest($"CPF {CPF} não foi encontrado no banco de dados.");
                if (agencyFromBank.Employees.Any(e => e.CPF == CPF)) return BadRequest($"CPF {CPF} já está registrado na agência.");
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

            if (agencyFromBank.AddressId != agency.AddressId)
            {
                agencyFromBank.Address = address;
                agencyFromBank.AddressId = agency.AddressId;
            }

            _context.Entry(agencyFromBank).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (Exception) { throw; }

            return Ok(agencyFromBank);
        }

        // POST: api/Agencies
        [HttpPost]
        public async Task<ActionResult<Agency>> PostAgency(AgencyDTO agency)
        {
            if (!CnpjValidator.IsValid(agency.CNPJ)) return BadRequest("CNPJ inválido.");

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

            Agency newAgency = null;
            try { newAgency = await _agenciesService.AgencyBuilder(agency, newAgencyNumber); }
            catch (Exception e) { return BadRequest(e.Message); }

            var result = _context.Agency.Add(newAgency);
            await _context.SaveChangesAsync();
            return Ok(newAgency);
        }

        // DELETE: api/Agencies/5
        [HttpDelete("{number}")]
        public async Task<ActionResult<Agency>> DeleteAgency(string number)
        {
            var agency = await _context.Agency.FindAsync(number);
            if (agency == null) return NotFound("Número da agência informado não corresponde a uma agência cadastrada.");

            agency.Restriction = true;
            _context.Agency.Update(agency);
            await _context.SaveChangesAsync();

            return Ok(agency);
        }

        private async Task<bool> AgencyExistsAsync(string number) => await Task.Run(() => (_context.Agency?.Any(e => e.Number == number)).GetValueOrDefault());
    }
}
