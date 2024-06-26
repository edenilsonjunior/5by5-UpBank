using Models.DTO;
using Models.People;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.People
{
    public class ClientService
    {
        public async Task<Address> GetAddressById(string id)
        {
            using (var client = new HttpClient())
            {
                var Response = await client.GetAsync($"https://localhost:7084/api/Addresses/{id}");
                return JsonConvert.DeserializeObject<Address>(await Response.Content.ReadAsStringAsync());
            }

        }
        public async Task<Address> PostAddress(AddressDTO addressDTO)
        {
            using (var client = new HttpClient())
            {
                var AddressJson = JsonConvert.SerializeObject(addressDTO);
                var Content = new StringContent(AddressJson, Encoding.UTF8, "application/json");
                var Response = await client.PostAsync($"https://localhost:7084/api/Addresses/", Content);
                return JsonConvert.DeserializeObject<Address>(await Response.Content.ReadAsStringAsync());
            }
        }
    }
}
