using Models.Bank;
using Newtonsoft.Json;

namespace Services.Agency
{
    public class AgenciesService
    {
        public async Task<List<Account>> GetAllAccounts()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync("https://localhost:7011/accounts");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Account>>(content);
                }
                return null;
            }
        }
    }
}