using Newtonsoft.Json;

namespace Services.Utils;

public class ApiConsume<T>
{
    public static async Task<T?> Get(string uri, string requestUri)
    {
        T? generics;

        try
        {
            using HttpClient client = new();
            client.BaseAddress = new Uri(uri);

            HttpResponseMessage response = await client.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();
            //generics = await response.Content.ReadFromJsonAsync<T>();
            string responseBody = await response.Content.ReadAsStringAsync();
            generics = JsonConvert.DeserializeObject<T>(responseBody);
        }
        catch (Exception)
        {
            return default;
        }

        if (generics == null)
            return default;

        return generics;
    }
}
