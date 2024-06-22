using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Models.People;
using UpBank.EmployeeAPI.Data;

namespace UpBank.EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly UpBankEmployeeAPIContext _context;

        public EmployeesController(UpBankEmployeeAPIContext context)
        {
            _context = context;
            //public AccountController accountController;
        }

        [HttpPost("CriarConta")]
        public async Task<ActionResult<Employee>> CriarConta([FromBody] string cpf, [FromBody] string registry) //DTO
        {
            //buscar cliente pelo CPF
            var cliente = _context.Client.SingleOrDefault(c => c.CPF == cpf);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado");
            }

            //verificar se o CPF já possui uma conta
            if (_context.Account.Any(a => a.ClientId == cliente.Id))
            {
                return Conflict("Cliente já possui uma conta");
            }

            //definir perfil de conta
            var conta = DefinirPerfilDeConta(cliente);

            //criar conta e inserir no banco
            _context.Account.Add(conta);
            await _context.SaveChangesAsync();
            return Ok(conta);
            //return CreatedAtAction("GetAccount", new { id = conta.Id }, conta);
        }

        private Account DefinirPerfilDeConta(Client cliente)
        {
            var conta = new Account();
            conta.ClientId = cliente.Cpf;
            conta.Type = AccountType.Type;
            return conta;
        }

        //**********************************************************************************************ACCOUNT Controller???
        [HttpPost("AprovarConta")] 
        public async Task<ActionResult<Account>> AprovarConta([FromBody] string registry, [FromBody] string IdConta)
        {
            //buscar pelo Id
            var conta = _context.Account.SingleOrDefault(c => c.IdConta == IdConta);
            if (conta == null)
            {
                return NotFound("Conta não encontrado");
            }

            //verificar se o funcionário tem permissão de aprovar a conta
            var employee = _context.Employee.SingleOrDefault(f => f.Registry == registry);
            if (employee == null)
            {
                return NotFound("Funcionário não encontrado.");
            }
            if (!employee.Manager)
            {
                return Unauthorized("Apenas o gerente pode aprovar conta.");
            }        
            //aprovar conta
            conta.Approved = true;
            //_context.Account.Update(conta);
            await _context.SaveChangesAsync();
            return Ok($"Conta aprovada {conta}");
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }
            return await _context.Employee.ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }
            var employee = await _context.Employee.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.Registry)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            if (_context.Employee == null)
            {
                return Problem("Entity set 'UpBankEmployeeAPIContext.Employee'  is null.");
            }
            _context.Employee.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployee", new { id = employee.Registry }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return (_context.Employee?.Any(e => e.Registry == id)).GetValueOrDefault();
        }
    }
}


