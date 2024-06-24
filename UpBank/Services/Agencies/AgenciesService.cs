using Models.Bank;
using Models.DTO;
using Models.People;
using Newtonsoft.Json;

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
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync("https://localhost:7042/Employees");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Employee>>(content);
                }
                return null;
            }
        }

        public async Task<Address> GetAddressById(string id)
        {
            using (HttpClient client = new())
            {
                var response = await client.GetAsync($"https://localhost:7084/{id}");
                return JsonConvert.DeserializeObject<Address>(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<Agency> AgencyBuilder(AgencyDTO agency, string newAgencyNumber)
        {
            List<Employee> employees = await GetAllEmployees();
            List<EmployeeDTOEntity> newAgencyEmployees = new();

            foreach (var employee in employees)
            {
                if (agency.Employees.Contains(employee.CPF))
                {
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
            }

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