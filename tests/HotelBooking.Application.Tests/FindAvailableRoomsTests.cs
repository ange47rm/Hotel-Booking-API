using HotelBooking.Application.Interfaces;
using HotelBooking.Application.UseCases;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Moq;
using Xunit;

namespace HotelBooking.Application.Tests;

public class FindAvailableRoomsTests
{
    [Fact]
    public async Task ExecuteAsync_CheckOutNotAfterCheckIn_Throws()
    {
        var useCase = new FindAvailableRooms(new Mock<IRoomRepository>().Object);
        var request = new FindAvailableRoomsRequest(1, new DateOnly(2026, 1, 10), new DateOnly(2026, 1, 10), 2);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_ZeroGuests_Throws()
    {
        var useCase = new FindAvailableRooms(new Mock<IRoomRepository>().Object);
        var request = new FindAvailableRoomsRequest(1, new DateOnly(2026, 1, 10), new DateOnly(2026, 1, 15), 0);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ReturnsMappedRooms()
    {
        var repository = new Mock<IRoomRepository>();
        repository.Setup(r => r.FindAvailableAsync(1, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<IEnumerable<RoomType>>()))
            .ReturnsAsync(new List<Room> { new() { Id = 5, HotelId = 1, RoomNumber = 101, Type = RoomType.Double } });

        var request = new FindAvailableRoomsRequest(1, new DateOnly(2026, 1, 10), new DateOnly(2026, 1, 15), 2);
        var result = await new FindAvailableRooms(repository.Object).ExecuteAsync(request);

        Assert.Single(result);
        Assert.Equal(5, result[0].RoomId);
        Assert.Equal(2, result[0].MaxOccupancy);
    }
}