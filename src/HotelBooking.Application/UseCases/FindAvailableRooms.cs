using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.UseCases;

public record FindAvailableRoomsRequest(int? HotelId, DateOnly CheckIn, DateOnly CheckOut, int Guests);
public record AvailableRoomResponse(int RoomId, int HotelId, int RoomNumber, RoomType Type, int MaxOccupancy);

public class FindAvailableRooms
{
    private readonly IRoomRepository _roomRepository;

    public FindAvailableRooms(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<List<AvailableRoomResponse>> ExecuteAsync(FindAvailableRoomsRequest request)
    {
        if (request.CheckIn < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Check-in cannot be in the past.");

        if (request.CheckOut <= request.CheckIn)
            throw new ArgumentException("Check-out must be after check-in.");

        if (request.Guests <= 0)
            throw new ArgumentException("Number of guests must be greater than zero.");

        var suitableTypes = Enum.GetValues<RoomType>().Where(t => t.MaxOccupancy() >= request.Guests);

        var rooms = await _roomRepository.FindAvailableAsync(request.HotelId, request.CheckIn, request.CheckOut, suitableTypes);

        return rooms.Select(r => new AvailableRoomResponse(r.Id, r.HotelId, r.RoomNumber, r.Type, r.Type.MaxOccupancy())).ToList();
    }
}