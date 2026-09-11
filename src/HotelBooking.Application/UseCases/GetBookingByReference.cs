using HotelBooking.Application.Interfaces;

namespace HotelBooking.Application.UseCases;

public record BookingDetailsResponse(string Reference, int RoomId, DateOnly CheckIn, DateOnly CheckOut, int NumberOfGuests);

public class GetBookingByReference
{
    private readonly IBookingRepository _bookingRepository;

    public GetBookingByReference(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<BookingDetailsResponse?> ExecuteAsync(string reference)
    {
        var booking = await _bookingRepository.GetByReferenceAsync(reference);

        return booking is null
            ? null
            : new BookingDetailsResponse(booking.Reference, booking.RoomId, booking.CheckIn, booking.CheckOut, booking.NumberOfGuests);
    }
}