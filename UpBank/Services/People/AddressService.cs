using Models.DTO;
using Models.People;
using MongoDB.Driver;
using Newtonsoft.Json;
using Services.Utils;


namespace Services.People;

public class AddressService
{
    private readonly IMongoCollection<Address> _collection;


    public AddressService(IMongoDataBaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        _collection = database.GetCollection<Address>(settings.AddressCollectionName);
    }

    public async Task<Address> CreateAddress(Address address)
    {
        _collection.InsertOne(address);
        return address;
    }

    public async Task<Address> GetAddress(string id)
    {
        var address = await _collection.Find<Address>(a => a.Id == id).FirstOrDefaultAsync();
        return address;
    }


    public async Task<Address?> GetAddressByZip(AddressDTO dto)
    {
        dynamic? responseAsDynamic = null;

        try
        {
            using var client = new HttpClient();
            string viacep = $"https://viacep.com.br/ws/";
            string url = $"{viacep}{dto.ZipCode}/json";

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                responseAsDynamic = JsonConvert.DeserializeObject<dynamic>(json);
            }
            else
            {
                return null;
            }
        }
        catch { return null; }

        if (responseAsDynamic == null)
            return null;

        Address address = new()
        {
            Street = responseAsDynamic.logradouro,
            Number = dto.Number,
            Complement = dto.Complement,
            ZipCode = responseAsDynamic.cep,
            City = responseAsDynamic.localidade,
            State = responseAsDynamic.uf,
            District = responseAsDynamic.bairro
        };

        return address;
    }
}
