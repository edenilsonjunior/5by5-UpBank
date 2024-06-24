using Microsoft.AspNetCore.Mvc;
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
            _employeeService = new EmployeeService("connectionString");
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
        // PUT: api/Employees
        [HttpPut("{registry}")]
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
    }
}


