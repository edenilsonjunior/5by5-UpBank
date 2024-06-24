using Dapper;
using Models.Bank;
using Models.DTO;
using Models.People;
using System.Net.Http.Json;
using Repositories;
using System.Data.SqlClient;

namespace Services.People
{
    public class EmployeeService
    {
        private readonly string _connString;
        public EmployeeService(string connString)
        {
            _connString = connString;
        }

        //POST: api/Employees
        public Employee PostEmployee(EmployeeDTO employeeDTO)
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
                var employee = connection.QueryFirstOrDefault<Employee>(query, new { Id = 1 });

                return employee;
            }
        }
        //Get: api/Employees
        public List<Employee> GetAllEmployee(EmployeeDTO employeeDTO)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                var query = "SELECT * FROM Employees";
                var employees = connection.Query<Employee>(query, new { Registry = employeeDTO.Registry }).ToList();
                return employees;
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

                connection.Execute(updateQuery, new
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
        private bool EmployeeExists(int registry)
        {
            return registry.Employee.Any(e => e.Registry == registry);
        }
        public Employee CreateAccount(string cpf, string registry)
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
        public Account DefineAccountProfile(Client client)
        {
            //perfil de conta
        }
        public Account ApproveAccount(string registry, string number)
        {
            //buscar pelo Id
            var account = Account.FirstOrDefault(a => a.Number == number);
            if (account == null)
            {
                throw new Exception("Conta não encontrada.");
            }

            //verificar se o funcionário tem permissão de aprovar a conta
            var employee = Employee.FirstOrDefault(e => e.Registry == registry);
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

