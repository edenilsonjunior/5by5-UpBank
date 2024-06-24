using Dapper;
using Models.Bank;
using Models.DTO;
using Models.People;
using System.Net.Http.Json;
using Repositories;
using System.Data.SqlClient;
using System.Net;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace Services.People
{
    public class EmployeeService
    {
        private readonly string _connString;

        private readonly EmployeeRepository _employeeRepository;
        
        public EmployeeService(string connString)
        {
            _connString = connString;
            _employeeRepository = new EmployeeRepository();
        }

        //POST: api/Employees
        public async Task<Employee> PostEmployee(EmployeeDTO employeeDTO)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
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
                    Address = address,
                    Salary = employeeDTO.Salary,
                    Phone = employeeDTO.Phone,
                    Email = employeeDTO.Email,
                    Manager = employeeDTO.Manager,
                    Registry = employeeDTO.Registry
                };

               bool result = _employeeRepository.Post(employee);
                if (result)
                {
                    return employee;
                }
                else
                {
                    return null;
                }
            }
        }
        //Get: api/Employees
        public async Task<List<Employee>> GetAllEmployee()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                var query = "SELECT * FROM Employees";
                var employees = connection.Query<dynamic>(query).ToList();
                List<Employee> list = new List<Employee>();
                foreach (var row in employees)
                {
                    Employee employee = new Employee()
                    {
                        Name = row.Name,
                        CPF = row.CPF,
                        BirthDt = row.BirthDt,
                        Sex = row.Sex,
                        Salary = row.Salary,
                        Phone = row.Phone,
                        Email = row.Email,
                        Manager = row.Manager,
                        Registry = row.Registry
                    };
                   
                    using HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("https://localhost:7084");
                    HttpResponseMessage response = await client.GetAsync($"/api/addresses/{row.IdAddress}");
                    Address address = null;
                    if (response.IsSuccessStatusCode)
                    {
                       address = await response.Content.ReadFromJsonAsync<Address>();
                    }
                    else
                    {
                        throw new Exception("Erro ao consumir o endpoint de GET");
                    }
                    employee.Address = address;
                    list.Add(employee);
                }
                return list;
            }
        }
        //GetRegistry: api/Employees
        public Employee GetEmployee(int registry)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                var query = "SELECT * FROM Employee WHERE Registry = @Registry";
                var employee = connection.Query<Employee>(query, new { Registry = registry }).FirstOrDefault();
                return employee;
            }
        }
        // PUT: api/Employees
        public void UpdateEmployee(int registry, Employee employee)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                if (registry != employee.Registry)
                {
                    if (registry != employee.Registry)
                    {
                        throw new ArgumentException("O número de registro não corresponde ao registro do funcionário.");
                    }
                }
                var query = @"UPDATE Employee 
                                    SET Name = @Name, CPF = @CPF, BirthDt = @BirthDt, 
                                        Sex = @Sex, IdAddress = @IdAddress, Salary = @Salary, 
                                        Phone = @Phone, Email = @Email, Manager = @Manager 
                                    WHERE Registry = @Registry";

                connection.Execute(query, new
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
                    Registry = registry
                });
            }
        }

        // DELETE: api/Employees/5
        public Employee RemoveEmployee(int registry)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                var employee = GetEmployee(registry);
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
                        Console.WriteLine("Erro ao excluir funcionário: " + ex.Message);
                    }
                    return employee;
                }
                return employee;
            }
        }
        public async Task<Employee> CreateAccount(string cpf, string registry)
        {
            //buscar cliente pelo CPF
            var client = Client.FirstOrDefault(c => c.CPF == cpf);
            if (client == null)
            {
                throw new Exception("Cliente não encontrado.");
            }
            //verificar se o CPF já possui uma conta
            var account = Account.FirstOrDefault(a => a.Client.CPF == cpf);
            if (account != null)
            {
                throw new Exception("Cliente já possui uma conta.");
            }
            else
            {
                account = new Account();
                account.Client = client;
                account.Number = Guid.NewGuid().ToString();
            }
            var perfilaccount = DefineAccountProfile(client);
            Account.Add(account);
            account.SaveChanges();
            return account;

        }
        //public Account DefineAccountProfile(Client client)
        //{
        //    //perfil de conta
        //}
        public Account ApproveAccount(string registry, string number)
        {
            //buscar pelo Id

            using (var httpAccount = new HttpClient())
            {
                httpAccount.BaseAddress = new Uri("    ");

                HttpResponseMessage response = await client.GetAsync("/api/accounts/");

                if (response.IsSuccessStatusCode)
                {
                    var account = await response.Content.ReadAsStringAsync<Account>();
                    
                }
                else
                {
                    throw new Exception("Erro ao consumir o endpoint de GET");
                }

            }

            var account = GetAccount().FirstOrDefault(a => a.Number == number);
            if (account == null)
            {
                throw new Exception("Conta não encontrada.");
            }

            //verificar se o funcionário tem permissão de aprovar a conta
            var employee = GetAllEmployee().FirstOrDefault(e => e.Registry == registry);
            if (employee == null)
            {
                throw new Exception("Funcionário não encontrado.");
            }
            if (!employee.Manager)
            {
                throw new Exception("Funcionário não tem permissão para aprovar contas.");
            }
            //aprovar conta
            account.Approve();
            account.SaveChanges();
            return account;
        }

    }
}

