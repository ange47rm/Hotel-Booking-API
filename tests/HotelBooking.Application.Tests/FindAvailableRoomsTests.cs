using HotelBooking.Application.Interfaces;
using HotelBooking.Application.UseCases;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Moq;
using Xunit;

namespace HotelBooking.Application.Tests;

public class FindAvailableRoomsTests
{
    private static readonly DateOnly CheckIn = DateOnly.FromDateTime(DateTime.Today).AddDays(30);
    private static readonly DateOnly CheckOut = CheckIn.AddDays(5);

    [Fact]
    public async Task ExecuteAsync_CheckInInPast_Throws()
    {
        var useCase = new FindAvailableRooms(new Mock<IRoomRepository>().Object);
        var request = new FindAvailableRoomsRequest(1, new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 5), 2);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_CheckOutNotAfterCheckIn_Throws()
    {
        var useCase = new FindAvailableRooms(new Mock<IRoomRepository>().Object);
        var request = new FindAvailableRoomsRequest(1, CheckIn, CheckIn, 2);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_ZeroGuests_Throws()
    {
        var useCase = new FindAvailableRooms(new Mock<IRoomRepository>().Object);
        var request = new FindAvailableRoomsRequest(1, CheckIn, CheckOut, 0);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ReturnsMappedRooms()
    {
        var repository = new Mock<IRoomRepository>();
        repository.Setup(r => r.FindAvailableAsync(1, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<IEnumerable<RoomType>>()))
            .ReturnsAsync(new List<Room> { new() { Id = 5, HotelId = 1, RoomNumber = 101, Type = RoomType.Double } });

        var request = new FindAvailableRoomsRequest(1, CheckIn, CheckOut, 2);
        var result = await new FindAvailableRooms(repository.Object).ExecuteAsync(request);

        Assert.Single(result);
        Assert.Equal(5, result[0].RoomId);
        Assert.Equal(2, result[0].MaxOccupancy);
    }
}