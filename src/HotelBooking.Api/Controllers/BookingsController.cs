using HotelBooking.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Route("bookings")]
public class BookingsController : ControllerBase
{
    private readonly BookRoom _bookRoom;
    private readonly GetBookingByReference _getBookingByReference;

    public BookingsController(BookRoom bookRoom, GetBookingByReference getBookingByReference)
    {
        _bookRoom = bookRoom;
        _getBookingByReference = getBookingByReference;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> CreateNewBooking(BookRoomRequest request)
    {
        var result = await _bookRoom.ExecuteAsync(request);

        return CreatedAtAction(nameof(GetBookingByReference), new { reference = result.Reference }, result);
    }

    [HttpGet("{reference}")]
    public async Task<ActionResult<BookingDetailsResponse>> GetBookingByReference(string reference)
    {
        var result = await _getBookingByReference.ExecuteAsync(reference);
        return result is null ? NotFound() : Ok(result);
    }
}