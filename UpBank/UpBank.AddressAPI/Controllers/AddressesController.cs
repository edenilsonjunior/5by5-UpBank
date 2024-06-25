using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.People;
using Newtonsoft.Json;
using Services.People;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace UpBank.AddressAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AddressesController : ControllerBase
{
    private AddressService _service;

    public AddressesController(AddressService service)
    {
        _service = service;
    }


    [HttpPost]
    public async Task<ActionResult<Address>> Post(AddressDTO dto)
    {
        
        var address = await _service.GetAddressByZip(dto);


        if(address == null)
            return NotFound("Nao foi possivel consultar a api de viacep");

        address = await _service.CreateAddress(address);

        return address == null ? BadRequest("Nao foi possivel inserir o endereco") : Ok(address);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Address>> Get(string id)
    {
        var address = await _service.GetAddress(id);

        return address == null ? NotFound("Endereco nao encontrado") : Ok(address);
    }
}