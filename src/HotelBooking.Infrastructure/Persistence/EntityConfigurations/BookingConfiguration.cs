using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(b => b.Reference)
            .HasMaxLength(8)
            .IsRequired();

        builder.HasIndex(b => b.Reference).IsUnique();
        builder.HasOne<Room>().WithMany().HasForeignKey(b => b.RoomId);
    }
}