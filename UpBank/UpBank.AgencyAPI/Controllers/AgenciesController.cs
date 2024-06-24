using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            if (_context.Agency == null) return NotFound();
            return await _context.Agency.ToListAsync();
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

        // GET: api/Agencies/{number}
        [HttpGet("{number}")]
        public async Task<ActionResult<Agency>> GetAgency(string number)
        {
            var agency = await _context.Agency.FindAsync(number);

            if (agency == null) return NotFound();

            return agency;
        }

        // PUT: api/Agencies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAgency(string id, Agency agency)
        {
            if (id != agency.Number) return BadRequest($"O numero informado {id} não condiz com o número da agência {agency.Number}.");

            _context.Entry(agency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception) { throw; }

            return NoContent();
        }

        // POST: api/Agencies
        [HttpPost]
        public async Task<ActionResult<Agency>> PostAgency(AgencyDTO agency)
        {
            if (CnpjValidator.IsValid(agency.CNPJ)) return BadRequest("CNPJ inválido.");

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
            
            return Ok(newAgency);
        }

        // DELETE: api/Agencies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgency(string id)
        {
            if (_context.Agency == null)
            {
                return NotFound();
            }
            var agency = await _context.Agency.FindAsync(id);
            if (agency == null)
            {
                return NotFound();
            }

            _context.Agency.Remove(agency);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> AgencyExistsAsync(string number) => await Task.Run(() => (_context.Agency?.Any(e => e.Number == number)).GetValueOrDefault());
    }
}
