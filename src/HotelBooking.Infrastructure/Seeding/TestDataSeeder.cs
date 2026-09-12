using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Seeding;

public class TestDataSeeder
{
    private readonly ApplicationDbContext _context;

    public TestDataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        var hotelNames = new[] { "Hotel Edinburgh", "Hotel Dundee", "Hotel Glasgow" };

        var today = DateOnly.FromDateTime(DateTime.Today);

        foreach (var hotelName in hotelNames)
        {
            var hotel = new Hotel { Name = hotelName };
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync(); // triggers hotel.Id creation, needed before creating its rooms

            var rooms = new List<Room>
            {
                new() { HotelId = hotel.Id, RoomNumber = 1, Type = RoomType.Single },
                new() { HotelId = hotel.Id, RoomNumber = 2, Type = RoomType.Single },
                new() { HotelId = hotel.Id, RoomNumber = 3, Type = RoomType.Double },
                new() { HotelId = hotel.Id, RoomNumber = 4, Type = RoomType.Double },
                new() { HotelId = hotel.Id, RoomNumber = 5, Type = RoomType.Deluxe },
                new() { HotelId = hotel.Id, RoomNumber = 6, Type = RoomType.Deluxe },
            };

            _context.Rooms.AddRange(rooms);
            await _context.SaveChangesAsync(); // as above, room Ids are needed before creating bookings against them

            var singleRoom = rooms.First(r => r.Type == RoomType.Single);
            var doubleRoom = rooms.First(r => r.Type == RoomType.Double);
            var deluxeRoom = rooms.First(r => r.Type == RoomType.Deluxe);

            var bookings = new[]
            {
                new Booking(singleRoom.Id, RoomType.Single, today.AddDays(7), today.AddDays(10), 1, $"SEED{hotel.Id}S1"),
                new Booking(doubleRoom.Id, RoomType.Double, today.AddDays(7), today.AddDays(10), 2, $"SEED{hotel.Id}D1"),
                new Booking(deluxeRoom.Id, RoomType.Deluxe, today.AddDays(7), today.AddDays(10), 4, $"SEED{hotel.Id}X1"),
            };

            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ResetAsync()
    {
        // children before parents, to satisfy FK constraints.
        await _context.Bookings.ExecuteDeleteAsync();
        await _context.Rooms.ExecuteDeleteAsync();
        await _context.Hotels.ExecuteDeleteAsync();
    }
}