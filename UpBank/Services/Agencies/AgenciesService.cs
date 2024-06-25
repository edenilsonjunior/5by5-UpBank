using Bogus;
using Models.Bank;
using Models.DTO;
using Models.People;
using Newtonsoft.Json;
using ZstdSharp;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace Services.Agencies
{
    public class AgenciesService
    {
        public async Task<List<Account>> GetAllAccounts()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync("https://localhost:7011/Accounts");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Account>>(content);
                }
                return null;
            }
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            //using (HttpClient client = new HttpClient())
            //{
            //    var response = await client.GetAsync("https://localhost:7042/Employees");
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var content = await response.Content.ReadAsStringAsync();
            //        return JsonConvert.DeserializeObject<List<Employee>>(content);
            //    }
            //    return null;
            //}

            return await Task.FromResult(new List<Employee>
                {
                    new Employee
                    {
                        Name = "John Doe",
                        CPF = "48451260870",
                        BirthDt = new DateTime(1990, 1, 1),
                        Sex = 'M',
                        Address = new Address
                        {
                            Id = "667abbb8d46955ec14c317e8",
                            Street = "Avenida Guanabara",
                            City = "João Pessoa",
                            State = "PB",
                            ZipCode = "58030-280"
                        },
                        Salary = 5000,
                        Phone = "1234567890",
                        Email = "john.doe@example.com",
                        Manager = false,
                        Registry = 123
                    },
                    new Employee
                    {
                        Name = "Jane Smith",
                        CPF = "10738170089",
                        BirthDt = new DateTime(1995, 5, 5),
                        Sex = 'F',
                        Address = new Address
                        {
                            Id = "667abc89d46955ec14c317e9",
                            Street = "Rua Professor Balbino de Lima Pitta",
                            City = "Vitória",
                            State = "ES",
                            ZipCode = "29070-280"
                        },
                        Salary = 6000,
                        Phone = "9876543210",
                        Email = "jane.smith@example.com",
                        Manager = true,
                        Registry = 456
                    }
                });
        }

        public async Task<Address> GetAddressById(string id)
        {
            using (HttpClient client = new())
            {
                var response = await client.GetAsync($"https://localhost:7084/api/Addresses/{id}");
                return JsonConvert.DeserializeObject<Address>(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<Agency> AgencyBuilder(AgencyDTO agency, string newAgencyNumber)
        {
            List<Employee> employees = await GetAllEmployees();
            List<EmployeeDTOEntity> newAgencyEmployees = new();
            int managerQuantity = 0;

            foreach (var employee in employees)
            {
                if (agency.Employees.Contains(employee.CPF))
                {
                    if (employee.Manager == true) managerQuantity++;

                    newAgencyEmployees.Add(new EmployeeDTOEntity
                    {
                        Name = employee.Name,
                        CPF = employee.CPF,
                        BirthDt = employee.BirthDt,
                        Sex = employee.Sex,
                        AddressId = employee.Address.Id,
                        Salary = employee.Salary,
                        Phone = employee.Phone,
                        Email = employee.Email,
                        Manager = employee.Manager,
                        Registry = employee.Registry,
                        AgencyNumber = newAgencyNumber
                    });
                }
                else { throw new Exception("CPF de funcionário não encontrado no banco de dados.");}
            }

            if (managerQuantity == 0) throw new Exception("A agência deve ter pelo menos um gerente.");

            return new Agency
            {
                Number = newAgencyNumber,
                Address = await GetAddressById(agency.AddressId),
                AddressId = agency.AddressId,
                CNPJ = agency.CNPJ,
                Employees = newAgencyEmployees,
                Restriction = false
            };
        }
    }
}