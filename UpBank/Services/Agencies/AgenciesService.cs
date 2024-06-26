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
                var response = await client.GetAsync("https://localhost:7011/api/Accounts");
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
                var response = await client.GetAsync("https://localhost:7042/api/Employees");
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
                var response = await client.GetAsync($"https://localhost:7084/api/Addresses/{id}");
                return JsonConvert.DeserializeObject<Address>(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<AgencyDTOEntity> AgencyDTOEntityBuilder(AgencyDTO agency, string newAgencyNumber)
        {
            List<Employee> employees = await GetAllEmployees();
            List<EmployeeDTOEntity> newAgencyEmployees = new();
            int managerQuantity = 0;


            foreach (var employeeDto in agency.Employees)
            {
                Employee? employee = employees.Find(e => e.Registry == employeeDto);

                if (employee != null)
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
                        Registry = employee.Registry
                    });
                }
                else { throw new Exception("Registro de funcionário não encontrado no banco de dados."); }
            }

            if (managerQuantity == 0) throw new Exception("A agência deve ter pelo menos um gerente.");

            return new AgencyDTOEntity
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