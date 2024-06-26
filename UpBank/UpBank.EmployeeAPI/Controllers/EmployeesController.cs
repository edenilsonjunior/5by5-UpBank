using Microsoft.AspNetCore.Mvc;
using Models.Bank;
using Models.DTO;
using Models.People;
using Services.People;
namespace UpBank.EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _employeeService;
        public EmployeesController()
        {
            _employeeService = new EmployeeService("Data Source=127.0.0.1; Initial Catalog=DBEmployee; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes");
        }

        // Gets

        [HttpGet]
        public async Task<List<Employee>> GetAllEmployee()
        {
            return await _employeeService.GetAllEmployee();
        }


        [HttpGet("{registry}")]
        public ActionResult<Employee> GetEmployee(int registry)
        {
            try
            {
                return _employeeService.GetEmployee(registry);
            }
            catch (Exception)
            {
                return NotFound("Cliente nao encontrado.");
            }
        }


        // Posts

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(EmployeeDTO employeeDTO)
        {
            try
            {
                return await _employeeService.PostEmployee(employeeDTO);
            }
            catch (Exception e)
            {
                return BadRequest("Erro: " + e.Message);
            }
        }


        [HttpPost("CreateAccount")]
        public async Task<Account> CreateAccount(AccountCreateDTO accountCreateDTO)
        {
            return await _employeeService.CreateAccount(accountCreateDTO);
        }


        // Patches

        [HttpPatch]
        public ActionResult UpdateEmployee(EmployeeUpdateDTO employee)
        {
            try
            {
                _employeeService.UpdateEmployee(employee);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest("Erro: " + e.Message);
            }
        }


        [HttpPatch("ApproveAccount/{registry}/{number}")]
        public async Task<Account> ApproveAccount(int registry, string number)
        {
            return await _employeeService.ApproveAccount(registry, number);
        }



        // Deletes

        [HttpDelete("{registry}")]
        public ActionResult RemoveEmployee(int registry)
        {
            try
            {
                _employeeService.RemoveEmployee(registry);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest("Erro: " + e.Message);
            }
        }
    }
}


