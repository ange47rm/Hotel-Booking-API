using HotelBooking.Application.Interfaces;
using HotelBooking.Application.UseCases;
using HotelBooking.Domain.Entities;
using Moq;
using Xunit;

namespace HotelBooking.Application.Tests;

public class FindHotelsByNameTests
{
    [Fact]
    public async Task ExecuteAsync_MapsRepositoryResultsToResponses()
    {
        var repository = new Mock<IHotelRepository>();
        repository.Setup(r => r.SearchByNameAsync("Test"))
            .ReturnsAsync(new List<Hotel> { new() { Id = 1, Name = "Hotel Test" } });

        var result = await new FindHotelsByName(repository.Object).ExecuteAsync("Test");

        Assert.Single(result);
        Assert.Equal("Hotel Test", result[0].Name);
    }
}