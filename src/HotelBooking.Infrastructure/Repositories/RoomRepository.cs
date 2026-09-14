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

    public async Task<List<Room>> FindAvailableAsync(int? hotelId, DateOnly checkIn, DateOnly checkOut, IEnumerable<RoomType> suitableTypes)
    {
        var query = _context.Rooms.AsQueryable();

        if (hotelId is not null)
            query = query.Where(r => r.HotelId == hotelId);

        // get suitable rooms
        var roomsOfSuitableType = query.Where(room => suitableTypes.Contains(room.Type));

        // ensure suitable rooms have no bookings based on input dates
        var availableRooms = roomsOfSuitableType.Where(room =>
            !_context.Bookings.Any(booking => booking.RoomId == room.Id && booking.CheckIn < checkOut && checkIn < booking.CheckOut));

        return await availableRooms.ToListAsync();
    }
}