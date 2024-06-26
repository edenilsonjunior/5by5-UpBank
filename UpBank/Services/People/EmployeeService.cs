using Dapper;
using Microsoft.SqlServer.Server;
using Models.Bank;
using Models.DTO;
using Models.People;
using Newtonsoft.Json;
using Repositories;
using Services.Utils;
using System;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Drawing;
using System.Net;
using System.Net.Http.Json;
using System.Text;

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

                if (!ValidateCPF(employeeDTO.CPF))
                {
                    throw new Exception("CPF inválido");
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
                var query = "SELECT * FROM Table_Employee";
                var employees = connection.Query<dynamic>(query).ToList();
                List<Employee> list = new List<Employee>();
                foreach (var row in employees)
                {
                    Employee employee = new Employee()
                    {
                        Name = row.Name,
                        CPF = row.CPF,
                        BirthDt = row.BirthDt,
                        Sex = char.Parse(row.Sex),
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
                var query = "SELECT * FROM Table_Employee WHERE Registry = @Registry";
                var row = connection.Query<dynamic>(query, new { Registry = registry }).FirstOrDefault();
                Employee employee = new Employee()
                {
                    Name = row.Name,
                    CPF = row.CPF,
                    BirthDt = row.BirthDt,
                    Sex = char.Parse(row.Sex),
                    Salary = row.Salary,
                    Phone = row.Phone,
                    Email = row.Email,
                    Manager = row.Manager,
                    Registry = row.Registry
                };

                using HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7084");
                HttpResponseMessage response = client.GetAsync($"/api/addresses/{row.IdAddress}").Result;
                Address address = null;
                if (response.IsSuccessStatusCode)
                {
                    address = response.Content.ReadFromJsonAsync<Address>().Result;
                }
                else
                {
                    throw new Exception("Erro ao consumir o endpoint de GET");
                }
                employee.Address = address;
                return employee;
            }

        }
        //PATCH: api/Employees
        public void UpdateEmployee(int registry, Employee employee)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                var updateEmployee = GetEmployee(registry);
                if (registry != employee.Registry)
                {
                    throw new ArgumentException("O número de registro não corresponde ao registro do funcionário.");
                }
                var querySelect = @"SELECT * FROM Table_Employee WHERE Registry = @Registry";
                var result = connection.Query<dynamic>(querySelect, new { Registry = registry }).FirstOrDefault();
                if (result == null)
                {
                    throw new Exception("Funcionário não encontrado no banco de dados.");
                }
                var query = @"UPDATE Table_Employee
                                    SET Name = @Name, 
                                        
                                        BirthDt = @BirthDt, 
                                        Sex = @Sex, 
                                        IdAddress = @IdAddress, 
                                        Salary = @Salary, 
                                        Phone = @Phone, 
                                        Email = @Email, 
                                        Manager = @Manager 
                                    WHERE Registry = @Registry";

                connection.Execute(query, new
                {
                    Name = employee.Name,
                    //CPF = employee.CPF,
                    BirthDt = employee.BirthDt,
                    Sex = employee.Sex,
                    IdAddress = employee.Address.Id,
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
                        string insertCanceledEmployee = @"INSERT INTO Table_EmployeeCanceled (Name, CPF, BirthDt, Sex, IdAddress, Salary, Phone, Email, Manager, Registry) 
                                                   VALUES (@Name, @CPF, @BirthDt, @Sex, @IdAddress, @Salary, @Phone, @Email, @Manager, @Registry)";
                        connection.Execute(insertCanceledEmployee, new
                        {
                            Name = employee.Name,
                            CPF = employee.CPF,
                            BirthDt = employee.BirthDt,
                            Sex = employee.Sex,
                            IdAddress = employee.Address.Id,
                            Salary = employee.Salary,
                            Phone = employee.Phone,
                            Email = employee.Email,
                            Manager = employee.Manager,
                            Registry = employee.Registry
                        });
                        string deleteEmployee = "DELETE FROM Table_Employee WHERE Registry = @Registry";
                        connection.Execute(deleteEmployee, new
                        {
                            Name = employee.Name,
                            CPF = employee.CPF,
                            BirthDt = employee.BirthDt,
                            Sex = employee.Sex,
                            IdAddress = employee.Address.Id,
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
        public async Task<Account> CreateAccount(AccountCreateDTO accountCreateDTO)
        {
            List<Client> clients = new List<Client>();
            // Consumindo a api de dados mockados
            foreach (var cpf in accountCreateDTO.ClientCPF)
            {
                Client? client = await ApiConsume<Client>.Get("https://localhost:7166", $"/GetClients/{cpf}");//7142 Clientes
                if (client == null)
                    throw new Exception("Cliente não encontrado.");
                clients.Add(client);
            }

            // Consumindo a api de Account
            List<Account>? accounts = await ApiConsume<List<Account>>.Get("https://localhost:7011", $"api/Accounts");

            Account? account = null;
            foreach (var ac in accounts)
            {
                if (ac.Client[0].CPF.Equals(clients[0].CPF))
                {
                    account = ac;
                    break;
                }
            }
            if (account != null)
                throw new Exception("Cliente já possui uma conta.");

            //buscar funcionário pelo registro
            Employee? employee = GetEmployee(accountCreateDTO.EmployeeRegister);
            if (employee == null)
            {
                throw new Exception("Funcionário não encontrado.");
            }

            Account accountcreate = new Account
            {
                Client = clients,
                CreditCard = new CreditCard(),
                Agency = new Agency() { Number = accountCreateDTO.AgencyNumber },
            };
            //Definir perfil da conta        
            accountcreate = await DefineProfile(accountcreate);



            return await InsertAccount(accountcreate);
        }
        public async Task<Account> InsertAccount(Account account)
        {
            AccountDTO accountDTO = new AccountDTO
            {
                ClientCPF = account.Client.Select(c => c.CPF).ToList(),
                AccountNumber = account.Number,
                AgencyNumber = account.Agency.Number,
                AccountProfile = account.Profile.ToString(),
                Overdraft=account.Overdraft,
                CreditCardLimit = account.CreditCard.Limit,
                CreditCardHolder = account.Client[0].Name,
            };

            using var httpClient = new HttpClient();

            var json = JsonConvert.SerializeObject(accountDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://localhost:7011/api/Accounts", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var newAccount = JsonConvert.DeserializeObject<Account>(responseContent);
                return newAccount;
            }
            else
            {
                var problemDetails = await response.Content.ReadAsStringAsync();
                throw new Exception(problemDetails);
            }
        }
        public async Task<Account> ApproveAccount(int registry, string number)
        {
            // Consumindo a api de Account
            List<Account>? accounts = await ApiConsume<List<Account>>.Get("https://localhost:7011", $"api/Accounts");
            Account? account = null;
            foreach (var ac in accounts)
            {
                if (account != null)
                    throw new Exception("Conta não encontrada!");
                else if (ac.Number.Equals(number))
                {
                    account = ac;
                }
            }
            //verificar se o funcionário tem permissão de aprovar a conta
            var employees = await GetAllEmployee();
            Employee? employee = employees.Find(e => e.Registry == registry);
            if (employee == null)
            {
                throw new Exception("Funcionário não encontrado.");
            }
            if (!employee.Manager)
            {
                throw new Exception("Funcionário não tem permissão para aprovar contas.");
            }
            var httpClient = new HttpClient();
            var response = await httpClient.PatchAsync($"https://localhost:7011/api/Accounts/ApproveAccount/{account.Number}", null);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"Nao foi encontrada uma conta com o numero {account.Number}");
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new Exception("Conta ja esta aprovada");
                }
                else
                {
                    throw new Exception("Erro ao aprovar conta");
                }
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var newAccount = JsonConvert.DeserializeObject<Account>(responseContent);
                return newAccount;
            }
            return account;
        }
        public static bool ValidateCPF(string cpf)
        {
            // Removendo caracteres não numéricos do CPF
            cpf = new string(cpf.Where(char.IsDigit).ToArray());
            // Verificando se o CPF possui 11 dígitos
            if (cpf.Length != 11)
            {
                return false;
            }
            // Verificando se todos os dígitos são iguais
            if (cpf.Distinct().Count() == 1)
            {
                return false;
            }
            // Calculando o primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (10 - i);
            }
            int resto = soma % 11;
            int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;
            // Verificando o primeiro dígito verificador
            if (int.Parse(cpf[9].ToString()) != digitoVerificador1)
            {
                return false;
            }
            // Calculando o segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (11 - i);
            }
            resto = soma % 11;
            int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;
            // Verificando o segundo dígito verificador
            if (int.Parse(cpf[10].ToString()) != digitoVerificador2)
            {
                return false;
            }
            return true;
        }

        public async Task<Account> DefineProfile(Account account)
        {
            if (account.Client[0].Salary != null && account.Client[0].Salary >= 0)
            {
                double salary = account.Client[0].Salary;

                Random r = new();
                if (salary < 3000)
                {
                    account.Profile = EProfile.Academic;
                }
                else if (salary >= 3000 && salary < 10000)
                {
                    account.Profile = EProfile.Normal;
                }
                else
                {
                    account.Profile = EProfile.VIP;
                }
                switch (account.Profile)
                {
                    case EProfile.Academic:
                        account.CreditCard.Limit = r.Next(1000, 3001);  // Limite de 1000 a 3000
                        account.Overdraft = r.Next(500, 1501);  // Limite de 500 a 1500
                        break;

                    case EProfile.Normal:
                        account.CreditCard.Limit = r.Next(3000, 10001);  // Limite de 3000 a 10000
                        account.Overdraft = r.Next(1500, 5001);  // Limite de 1500 a 5000
                        break;

                    case EProfile.VIP:
                        account.CreditCard.Limit = r.Next(10000, 50001);  // Limite de 10000 a 50000
                        account.Overdraft = r.Next(5000, 20001);  // Limite de 5000 a 20000
                        break;
                }
            }
            else
            {
                throw new Exception("Salário inválido.");
            }
            return account;
        }
    }
}





