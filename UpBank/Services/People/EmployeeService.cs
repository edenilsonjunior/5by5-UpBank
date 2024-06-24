using Models.Bank;
using Models.DTO;
using Models.People;
using System.Net.Http.Json;
using Repositories;

namespace Services.People
{
    public class EmployeeService
    {
        private EmployeeRepository _employeeRepository;
        private readonly string _conn;
        private readonly string _context;
        public EmployeeService()
        {
            _employeeRepository = new();
        }
        //POST: api/Employees
        public async Task<Employee> PostEmployee(EmployeeDTO employeeDTO)
        {
            Address address = new Address();
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.PostAsJsonAsync("https://localhost:7084/api/Addresses", employeeDTO.Address);
                if (response.IsSuccessStatusCode)
                {
                    address = await response.Content.ReadFromJsonAsync<Address>();
                }
                else
                {
                    throw new Exception("Falha ao consumir endpoint");
                }
            }
            catch (Exception)
            {
                throw;
            }
            Employee employee = new Employee
            {
                Name = employeeDTO.Name,
                CPF = employeeDTO.CPF,
                BirthDt = employeeDTO.BirthDt,
                Sex = employeeDTO.Sex,
                IdAddress = employeeDTO.Address,
                Salary = employeeDTO.Salary,
                Phone = employeeDTO.Phone,
                Email = employeeDTO.Email,
                Manager = employeeDTO.Manager,
                Registry = employeeDTO.Registry
            }.Single();

            return employee;
        }
        //Get: api/Employees
        public async Task<List<Employee>> GetAllEmployee(EmployeeDTO employeeDTO)
        {
            var employees = await _context.Employee.ToListAsync();
            return employees;
        }
        //GetRegistry: api/Employees
        public async Task<Employee> GetEmployee(int registry)
        {
            var employee = await _context.Employee.FindAsync(registry);
            if (employee == null)
            {
                return NotFound();
            }
            return employee;
        }
        // PUT: api/Employees
        public async Task<Employee> PutEmployee(int registry, Employee employee)
        {
            if (registry != employee.Registry)
            {
                return BadRequest();
            }
            _context.(employee).State = Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch ()
            {
                if (!EmployeeExists(registry))
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
        // DELETE: api/Employees/5
        public async Task<Employee> RemoveEmployee(int registry)
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(_conn))
            {
                connection.Open();

                var employee = GetEmployee(registry).Result;
                if (employee == null)
                {
                    Console.WriteLine("Funcionário não encontrado.");
                    return null;
                }
                if (employee != null)
                {
                    try //cria um registro na tabela de funcionário excluído
                    {
                        string insertCanceledEmployee = @"INSERT INTO EmployeeCanceled (Name, CPF, BirthDt, Sex, IdAddress, Salary, Phone, Email, Manager, Registry) 
                                                   VALUES (@Name, @CPF, @BirthDt, @Sex, @IdAddress, @Salary, @Phone, @Email, @Manager, @Registry)";
                        connection.Execute(insertCanceledEmployee, new
                        {
                            Name = employee.Name,
                            CPF = employee.CPF,
                            BirthDt = employee.BirthDt,
                            Sex = employee.Sex,
                            IdAddress = employee.Address,
                            Salary = employee.Salary,
                            Phone = employee.Phone,
                            Email = employee.Email,
                            Manager = employee.Manager,
                            Registry = employee.Registry
                        });

                        string updateEmployee = "UPDATE Employee SET Name=0, CPF=0, BirthDt=0, Sex=0, IdAddress=0, Salary=0, Phone=0, Email=0, Manager=0, Registry=0 WHERE Registry = @Registry";
                        connection.Execute(updateEmployee, new
                        {
                            Name = employee.Name,
                            CPF = employee.CPF,
                            BirthDt = employee.BirthDt,
                            Sex = employee.Sex,
                            IdAddress = employee.Address,
                            Salary = employee.Salary,
                            Phone = employee.Phone,
                            Email = employee.Email,
                            Manager = employee.Manager,
                            Registry = employee.Registry
                        });

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro ao cancelar funcionário: " + ex.Message);
                    }
                    return employee;
                }
            }
        }
        private bool EmployeeExists(int id)
        {
            return (_context.Employee?.Any(e => e.Registry == id)).GetValueOrDefault();
        }
        public async Task<Employee> CreateAccount(string cpf, string registry)
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
            var conta = DefineAccountProfile(cliente);

            //criar conta e inserir no banco
            _context.Account.Add(conta);
            await _context.SaveChangesAsync();
            return Ok(conta);
            //return CreatedAtAction("GetAccount", new { id = conta.Id }, conta);
        }
        private Account DefineAccountProfile(Client cliente)
        {
            var conta = new Account();
            conta.ClientId = cliente.Cpf;
            conta.Type = AccountType.Type;
            return conta;
        }
        public async Task<Account> ApproveAccount(string registry, string number)
        {
            //buscar pelo Id
            var conta = _context.Account.SingleOrDefault(a => a.Number == number);
            if (conta == null)
            {
                return NotFound("Conta não encontrada.");
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
    }
}

