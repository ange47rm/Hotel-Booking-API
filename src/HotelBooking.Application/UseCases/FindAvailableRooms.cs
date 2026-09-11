using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.UseCases;

public record FindAvailableRoomsRequest(int HotelId, DateOnly CheckIn, DateOnly CheckOut, int Guests);
public record AvailableRoomResponse(int RoomId, int RoomNumber, RoomType Type, int MaxOccupancy);

public class FindAvailableRooms
{
    private readonly IRoomRepository _roomRepository;

    public FindAvailableRooms(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<List<AvailableRoomResponse>> ExecuteAsync(FindAvailableRoomsRequest request)
    {
        if (request.CheckOut <= request.CheckIn)
            throw new ArgumentException("Check-out must be after check-in.");

        if (request.Guests <= 0)
            throw new ArgumentException("Number of guests must be greater than zero.");

        // find suitable room types based on number of guest, using room type/max guest mapping
        var suitableRoomTypes = Enum.GetValues<RoomType>().Where(roomType => roomType.MaxOccupancy() >= request.Guests);

        var rooms = await _roomRepository.FindAvailableAsync(request.HotelId, request.CheckIn, request.CheckOut, suitableRoomTypes);

        return rooms.Select(r => new AvailableRoomResponse(r.Id, r.RoomNumber, r.Type, r.Type.MaxOccupancy())).ToList();
    }
}