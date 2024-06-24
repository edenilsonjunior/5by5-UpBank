using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Bank;
using Services.Agency;
using UpBank.AgencyAPI.Data;

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
            if (_context.Agency == null)
            {
                return NotFound();
            }
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

        // GET: api/Agencies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Agency>> GetAgency(string id)
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

            return agency;
        }

        // PUT: api/Agencies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAgency(string id, Agency agency)
        {
            if (id != agency.Number)
            {
                return BadRequest();
            }

            _context.Entry(agency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgencyExists(id))
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

        // POST: api/Agencies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Agency>> PostAgency(Agency agency)
        {
            if (_context.Agency == null)
            {
                return Problem("Entity set 'UpBankAgencyAPIContext.Agency'  is null.");
            }
            _context.Agency.Add(agency);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AgencyExists(agency.Number))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAgency", new { id = agency.Number }, agency);
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

        private bool AgencyExists(string id)
        {
            return (_context.Agency?.Any(e => e.Number == id)).GetValueOrDefault();
        }
    }
}
