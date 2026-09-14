using HotelBooking.Application.Interfaces;
using HotelBooking.Application.UseCases;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Moq;
using Xunit;

namespace HotelBooking.Application.Tests;

public class BookRoomTests
{
    private static readonly DateOnly CheckIn = DateOnly.FromDateTime(DateTime.Today).AddDays(30);
    private static readonly DateOnly CheckOut = CheckIn.AddDays(5);

    [Fact]
    public async Task ExecuteAsync_RoomNotFound_ThrowsKeyNotFound()
    {
        var roomRepository = new Mock<IRoomRepository>();
        roomRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Room?)null);

        var useCase = new BookRoom(roomRepository.Object, new Mock<IBookingRepository>().Object);
        var request = new BookRoomRequest(99, CheckIn, CheckOut, 2);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_GuestsExceedCapacity_Throws()
    {
        var roomRepository = new Mock<IRoomRepository>();
        roomRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Room { Id = 1, HotelId = 1, RoomNumber = 101, Type = RoomType.Single });

        var useCase = new BookRoom(roomRepository.Object, new Mock<IBookingRepository>().Object);
        var request = new BookRoomRequest(1, CheckIn, CheckOut, 5);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_CheckInInPast_Throws()
    {
        var roomRepository = new Mock<IRoomRepository>();
        roomRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Room { Id = 1, HotelId = 1, RoomNumber = 101, Type = RoomType.Double });

        var useCase = new BookRoom(roomRepository.Object, new Mock<IBookingRepository>().Object);
        var request = new BookRoomRequest(1, new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 5), 2);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_CallsAddAsyncAndReturnsResponse()
    {
        var roomRepository = new Mock<IRoomRepository>();
        roomRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Room { Id = 1, HotelId = 1, RoomNumber = 101, Type = RoomType.Double });

        var bookingRepository = new Mock<IBookingRepository>();

        var useCase = new BookRoom(roomRepository.Object, bookingRepository.Object);
        var request = new BookRoomRequest(1, CheckIn, CheckOut, 2);

        var result = await useCase.ExecuteAsync(request);

        Assert.Equal(1, result.RoomId);
        Assert.Equal(8, result.Reference.Length);
        bookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Once);
    }
}