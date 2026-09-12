using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.UseCases;

public record BookRoomRequest(int RoomId, DateOnly CheckIn, DateOnly CheckOut, int NumberOfGuests);
public record BookingResponse(string Reference, int RoomId, DateOnly CheckIn, DateOnly CheckOut, int NumberOfGuests);

public class BookRoom
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public BookRoom(IRoomRepository roomRepository, IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<BookingResponse> ExecuteAsync(BookRoomRequest request)
    {
        var room = await _roomRepository.GetByIdAsync(request.RoomId);

        if (room == null)
        {
            throw new KeyNotFoundException($"Room {request.RoomId} not found.");
        }

        var bookingReference = GenerateBookingReference();

        var booking = new Booking(request.RoomId, room.Type, request.CheckIn, request.CheckOut, request.NumberOfGuests, bookingReference);

        await _bookingRepository.AddAsync(booking);

        return new BookingResponse(booking.Reference, booking.RoomId, booking.CheckIn, booking.CheckOut, booking.NumberOfGuests);
    }

    private static string GenerateBookingReference()
    {
        const string allowedCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // excludes confusing characters O/0, I/1
        
        const int referenceLength = 8;

        // build a sequence of random characters by picking one at a time from the allowed set
        var randomCharacters = Enumerable.Range(0, referenceLength)
            .Select(_ => allowedCharacters[Random.Shared.Next(allowedCharacters.Length)]);

        return new string(randomCharacters.ToArray());
    }
}