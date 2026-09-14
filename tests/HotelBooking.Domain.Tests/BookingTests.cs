using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Xunit;

namespace HotelBooking.Domain.Tests;

public class BookingTests
{
    private static readonly DateOnly CheckIn = DateOnly.FromDateTime(DateTime.Today).AddDays(30);
    private static readonly DateOnly CheckOut = CheckIn.AddDays(5);

    [Fact]
    public void Constructor_ValidInput_CreatesBooking()
    {
        var booking = new Booking(1, RoomType.Double, CheckIn, CheckOut, 2, "ABC123");
        Assert.Equal(1, booking.RoomId);
        Assert.Equal("ABC123", booking.Reference);
    }

    [Fact]
    public void Constructor_CheckInInPast_Throws()
    {
        var pastDate = new DateOnly(2020, 1, 1);
        Assert.Throws<ArgumentException>(() => new Booking(1, RoomType.Single, pastDate, pastDate.AddDays(3), 1, "REF"));
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
    public void OverlapsWith_ReturnsExpected(int otherInOffset, int otherOutOffset, bool expected)
    {
        var booking = new Booking(1, RoomType.Double, CheckIn, CheckOut, 2, "REF");
        var otherIn = CheckIn.AddDays(otherInOffset);
        var otherOut = CheckIn.AddDays(otherOutOffset);

        Assert.Equal(expected, booking.OverlapsWith(otherIn, otherOut));
    }

    public static IEnumerable<object[]> OverlapCases()
    {
        // offsets are in days relative to the booking's own CheckIn (day 0); booking spans day 0 - day 5
        yield return new object[] { 5, 10, false };  // starts on checkout day (turnover)
        yield return new object[] { -5, 0, false };  // ends on check-in day
        yield return new object[] { 2, 8, true };    // overlaps start
        yield return new object[] { -2, 3, true };   // overlaps end
        yield return new object[] { 1, 4, true };    // fully contained
        yield return new object[] { 0, 5, true };    // identical
    }
}