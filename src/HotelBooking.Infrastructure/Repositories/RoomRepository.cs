using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _context;

    public RoomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(int roomId)
    {
        return await _context.Rooms.FindAsync(roomId);
    }

    public async Task<List<Room>> FindAvailableAsync(int hotelId, DateOnly checkIn, DateOnly checkOut, IEnumerable<RoomType> suitableRoomTypes)
    {
        return await _context.Rooms
            .Where(room => room.HotelId == hotelId && suitableRoomTypes.Contains(room.Type))
            .Where(room => !_context.Bookings.Any(booking =>
                booking.RoomId == room.Id && booking.CheckIn < checkOut && checkIn < booking.CheckOut))
            .ToListAsync();
    }
}