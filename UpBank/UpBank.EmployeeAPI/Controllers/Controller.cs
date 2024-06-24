using Dapper;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.People;
using Services.People;

namespace UpBank.EmployeeAPI.Controllers
{
    public class Controller
    {
        private readonly EmployeeService _employeeService;

        public EmployeesController()
        {
            _employeeService = new EmployeeService("connectionString");
        }

        [HttpPost]
        public Employee PostEmployee(EmployeeDTO employeeDTO)
        {
            return null;
        }
        // GET: api/Employees
        [HttpGet]
        public Employee GetAllEmployee()
        {
            return _employeeService.GetAllEmployee();
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
