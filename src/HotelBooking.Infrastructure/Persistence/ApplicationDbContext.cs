using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>()
            .HasOne<Hotel>()
            .WithMany()
            .HasForeignKey(r => r.HotelId);

        modelBuilder.Entity<Booking>()
            .HasOne<Room>()
            .WithMany()
            .HasForeignKey(b => b.RoomId);

        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.Reference)
            .IsUnique();
    }
}