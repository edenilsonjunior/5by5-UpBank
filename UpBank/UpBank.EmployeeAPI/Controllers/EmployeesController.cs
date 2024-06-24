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
            _employeeService = new();
        }
        [HttpPost]
        public async Task<Employee> PostEmployee(EmployeeDTO employeeDTO)
        {
            return null;
        }
        // GET: api/Employees
        [HttpGet]
        public IEnumerable<Employee> GetEmployee()
        {
            return _employeeService.GetEmployee();
        }
        // GETALL: api/Employees
        [HttpGet("{registry}")]
        public Employee GetAllEmployee(int registry)
        {
            return _employeeService.GetAllEmployee(registry);
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


