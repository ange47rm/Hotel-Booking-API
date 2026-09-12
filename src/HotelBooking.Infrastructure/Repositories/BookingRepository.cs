using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Booking booking)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var lockResource = $"Room_{booking.RoomId}";

            // Blocks other requests for the same room until this transaction commits or rolls back.
            // 5-second timeout bounds the worst-case wait rather than blocking forever.
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC sp_getapplock @Resource = {lockResource}, @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 5000");

            var hasOverlap = await HasOverlappingBookingAsync(booking.RoomId, booking.CheckIn, booking.CheckOut);

            if (hasOverlap)
            {
                throw new InvalidOperationException("Room is already booked for the requested dates.");
            }

            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (SqlException)
        {
            throw new InvalidOperationException("Could not secure the room for booking — please try again.");
        }
    }

    public async Task<Booking?> GetByReferenceAsync(string reference)
    {
        return await _context.Bookings.FirstOrDefaultAsync(b => b.Reference == reference);
    }

    private async Task<bool> HasOverlappingBookingAsync(int roomId, DateOnly checkIn, DateOnly checkOut)
    {
        return await _context.Bookings.AnyAsync(b =>
            b.RoomId == roomId && b.CheckIn < checkOut && checkIn < b.CheckOut);
    }
}