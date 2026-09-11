using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Interfaces;

public interface IHotelRepository
{
    Task<List<Hotel>> SearchByNameAsync(string name);
}