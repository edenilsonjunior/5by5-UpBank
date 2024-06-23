using Microsoft.AspNetCore.Mvc;
using Models.Bank;
using Models.People;
using System.Text.Json;

namespace UpBank.Mock.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MocksController : ControllerBase
{
    private List<Agency> _agencies;
    private List<Client> _clients;

    public MocksController()
    {
        LoadData();
    }

    [HttpGet("/GetAgencies")]
    public List<Agency> GetAgencies() => _agencies;


    [HttpGet("/GetClients")]
    public List<Client> GetClients() => _clients;


    private void LoadData()
    {
        string agenciesJson = System.IO.File.ReadAllText(@"Mock/agencies.json");
        string clientsJson = System.IO.File.ReadAllText(@"Mock/clients.json");

        _agencies = JsonSerializer.Deserialize<List<Agency>>(agenciesJson);
        _clients = JsonSerializer.Deserialize<List<Client>>(clientsJson);
    }

}
