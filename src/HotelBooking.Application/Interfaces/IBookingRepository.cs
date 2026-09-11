using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Interfaces;

public interface IBookingRepository
{
    Task<bool> HasOverlappingBookingAsync(int roomId, DateOnly checkIn, DateOnly checkOut);
    Task AddAsync(Booking booking);
    Task<Booking?> GetByReferenceAsync(string reference);
}