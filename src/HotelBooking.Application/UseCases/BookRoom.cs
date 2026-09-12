using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.ValueObjects;

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

        var bookingReference = BookingReference.Generate();

        var booking = new Booking(request.RoomId, room.Type, request.CheckIn, request.CheckOut, request.NumberOfGuests, bookingReference);

        await _bookingRepository.AddAsync(booking);

        return new BookingResponse(booking.Reference, booking.RoomId, booking.CheckIn, booking.CheckOut, booking.NumberOfGuests);
    }
}