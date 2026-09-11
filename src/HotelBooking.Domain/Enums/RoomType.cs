namespace HotelBooking.Domain.Enums;

public enum RoomType
{
    Single,
    Double,
    Deluxe
}

public static class RoomTypeExtensions
{
    private static readonly Dictionary<RoomType, int> MaxOccupancyByType = new()
    {
        [RoomType.Single] = 1,
        [RoomType.Double] = 2,
        [RoomType.Deluxe] = 4
    };

    public static int MaxOccupancy(this RoomType type) => MaxOccupancyByType[type];
}