using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(int roomId);
    Task<List<Room>> FindAvailableAsync(int? hotelId, DateOnly checkIn, DateOnly checkOut, IEnumerable<RoomType> suitableTypes);
}