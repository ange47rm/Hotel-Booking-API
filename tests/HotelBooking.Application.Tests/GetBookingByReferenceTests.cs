using HotelBooking.Application.Interfaces;
using HotelBooking.Application.UseCases;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Moq;
using Xunit;

namespace HotelBooking.Application.Tests;

public class GetBookingByReferenceTests
{
    [Fact]
    public async Task ExecuteAsync_NotFound_ReturnsNull()
    {
        var repository = new Mock<IBookingRepository>();
        repository.Setup(r => r.GetByReferenceAsync("MISSING")).ReturnsAsync((Booking?)null);

        var result = await new GetBookingByReference(repository.Object).ExecuteAsync("MISSING");

        Assert.Null(result);
    }

    [Fact]
    public async Task ExecuteAsync_Found_ReturnsMappedResponse()
    {
        var checkIn = DateOnly.FromDateTime(DateTime.Today).AddDays(30);
        var checkOut = checkIn.AddDays(5);
        var booking = new Booking(1, RoomType.Double, checkIn, checkOut, 2, "ABC12345");

        var repository = new Mock<IBookingRepository>();
        repository.Setup(r => r.GetByReferenceAsync("ABC12345")).ReturnsAsync(booking);

        var result = await new GetBookingByReference(repository.Object).ExecuteAsync("ABC12345");

        Assert.Equal("ABC12345", result!.Reference);
    }
}