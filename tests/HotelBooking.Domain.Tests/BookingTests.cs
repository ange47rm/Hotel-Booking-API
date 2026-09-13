using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Xunit;

namespace HotelBooking.Domain.Tests;

public class BookingTests
{
    private static readonly DateOnly CheckIn = new(2026, 1, 10);
    private static readonly DateOnly CheckOut = new(2026, 1, 15);

    [Fact]
    public void Constructor_ValidInput_CreatesBooking()
    {
        var booking = new Booking(1, RoomType.Double, CheckIn, CheckOut, 2, "ABC123");
        Assert.Equal(1, booking.RoomId);
        Assert.Equal("ABC123", booking.Reference);
    }

    [Fact]
    public void Constructor_CheckOutNotAfterCheckIn_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Booking(1, RoomType.Single, CheckIn, CheckIn, 1, "REF"));
    }

    [Theory]
    [InlineData(RoomType.Single, 2)]
    [InlineData(RoomType.Double, 3)]
    [InlineData(RoomType.Deluxe, 5)]
    public void Constructor_GuestsExceedCapacity_Throws(RoomType type, int guests)
    {
        Assert.Throws<ArgumentException>(() => new Booking(1, type, CheckIn, CheckOut, guests, "REF"));
    }

    [Fact]
    public void Constructor_ZeroGuests_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Booking(1, RoomType.Single, CheckIn, CheckOut, 0, "REF"));
    }

    [Theory]
    [MemberData(nameof(OverlapCases))]
    public void OverlapsWith_ReturnsExpected(DateOnly otherIn, DateOnly otherOut, bool expected)
    {
        var booking = new Booking(1, RoomType.Double, CheckIn, CheckOut, 2, "REF");
        Assert.Equal(expected, booking.OverlapsWith(otherIn, otherOut));
    }

    public static IEnumerable<object[]> OverlapCases()
    {
        // existing booking: Jan 10 - Jan 15
        yield return new object[] { new DateOnly(2026, 1, 15), new DateOnly(2026, 1, 20), false }; // starts on checkout day (turnover)
        yield return new object[] { new DateOnly(2026, 1, 5), new DateOnly(2026, 1, 10), false };  // ends on check-in day
        yield return new object[] { new DateOnly(2026, 1, 12), new DateOnly(2026, 1, 18), true };  // overlaps start
        yield return new object[] { new DateOnly(2026, 1, 8), new DateOnly(2026, 1, 13), true };   // overlaps end
        yield return new object[] { new DateOnly(2026, 1, 11), new DateOnly(2026, 1, 14), true };  // fully contained
        yield return new object[] { new DateOnly(2026, 1, 10), new DateOnly(2026, 1, 15), true };  // identical
    }
}