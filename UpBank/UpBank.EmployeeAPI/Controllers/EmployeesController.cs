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
        [HttpPost]
        public async Task <Employee> PostEmployee(EmployeeDTO employeeDTO)
        {
            return await _employeeService.PostEmployee(employeeDTO);        
        }
        // GET: api/Employees
        [HttpGet]
        public async Task<List<Employee>> GetAllEmployee()
        {
            return  await _employeeService.GetAllEmployee();
        }
        // GETALL: api/Employees
        [HttpGet("{registry}")]
        public Employee GetEmployee(int registry)
        {
            return _employeeService.GetEmployee(registry);
        }
        // PATCH: api/Employees
        [HttpPatch("{registry}")]
        public void UpdateEmployee(int registry, Employee employee)
        {
            _employeeService.UpdateEmployee(registry, employee);
        }
        // DELETE: api/Employees
        [HttpDelete("{registry}")]
        public void RemoveEmployee(int registry)
        {
            _employeeService.RemoveEmployee(registry);
        }
        [HttpPost("CreateAccount")]
        public async Task<Account> CreateAccount(AccountCreateDTO accountCreateDTO)
        {
            return await _employeeService.CreateAccount(accountCreateDTO);
        }
        [HttpPatch("ApproveAccount/{registry}/{number}")]
        public async Task<Account> ApproveAccount(int registry, string number)
        {
            return await _employeeService.ApproveAccount(registry,number);
        }
    }
}


