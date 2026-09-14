using HotelBooking.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Route("rooms")]
public class RoomsController : ControllerBase
{
    private readonly FindAvailableRooms _findAvailableRooms;

    public RoomsController(FindAvailableRooms findAvailableRooms)
    {
        _findAvailableRooms = findAvailableRooms;
    }

    [HttpGet("available")]
    public async Task<ActionResult<List<AvailableRoomResponse>>> GetAvailableRooms(int? hotelId, DateOnly checkIn, DateOnly checkOut, int guests)
    {
        var request = new FindAvailableRoomsRequest(hotelId, checkIn, checkOut, guests);

        var result = await _findAvailableRooms.ExecuteAsync(request);

        return Ok(result);
    }
}