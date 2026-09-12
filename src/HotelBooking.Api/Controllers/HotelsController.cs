using HotelBooking.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Route("hotels")]
public class HotelsController : ControllerBase
{
    private readonly FindHotelsByName _findHotelsByName;

    public HotelsController(FindHotelsByName findHotelsByName)
    {
        _findHotelsByName = findHotelsByName;
    }

    [HttpGet]
    public async Task<ActionResult<List<HotelResponse>>> GetHotelByName([FromQuery] string name)
    {
        var result = await _findHotelsByName.ExecuteAsync(name);
        return Ok(result);
    }
}