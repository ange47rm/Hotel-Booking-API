using HotelBooking.Application.Interfaces;

namespace HotelBooking.Application.UseCases;

public record HotelResponse(int Id, string Name);

public class FindHotelsByName
{
    private readonly IHotelRepository _hotelRepository;

    public FindHotelsByName(IHotelRepository hotelRepository)
    {
        _hotelRepository = hotelRepository;
    }

    public async Task<List<HotelResponse>> ExecuteAsync(string name)
    {
        var hotels = await _hotelRepository.SearchByNameAsync(name);

        return hotels.Select(h => new HotelResponse(h.Id, h.Name)).ToList();
    }
}