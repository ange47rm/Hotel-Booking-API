using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Interfaces;

public interface IBookingRepository
{
    Task AddAsync(Booking booking);
    Task<Booking?> GetByReferenceAsync(string reference);
}