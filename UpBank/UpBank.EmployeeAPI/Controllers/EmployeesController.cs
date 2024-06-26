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
        public Employee GetEmployee(int registry)
        {
            return _employeeService.GetEmployee(registry);
        }


        // Posts

        [HttpPost]
        public async Task<Employee> PostEmployee(EmployeeDTO employeeDTO)
        {
            return await _employeeService.PostEmployee(employeeDTO);
        }


        [HttpPost("CreateAccount")]
        public async Task<Account> CreateAccount(AccountCreateDTO accountCreateDTO)
        {
            return await _employeeService.CreateAccount(accountCreateDTO);
        }


        // Patches

        [HttpPatch("{registry}")]
        public void UpdateEmployee(int registry, EmployeeUpdateDTO employee)
        {
            _employeeService.UpdateEmployee(registry, employee);
        }


        [HttpPatch("ApproveAccount/{registry}/{number}")]
        public async Task<Account> ApproveAccount(int registry, string number)
        {
            return await _employeeService.ApproveAccount(registry, number);
        }



        // Deletes

        [HttpDelete("{registry}")]
        public void RemoveEmployee(int registry)
        {
            _employeeService.RemoveEmployee(registry);
        }
    }
}


