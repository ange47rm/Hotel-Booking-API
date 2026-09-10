namespace HotelBooking.Domain;

public class Booking
{
    public int Id { get; private set; }
    public string Reference { get; private set; } = string.Empty;
    public int RoomId { get; private set; }
    public DateOnly CheckIn { get; private set; }
    public DateOnly CheckOut { get; private set; }
    public int NumberOfGuests { get; private set; }

    private Booking() { } // parameterless constructor required by EF Core

    public Booking(int roomId, RoomType roomType, DateOnly checkIn, DateOnly checkOut, int numberOfGuests, string reference)
    {
        if (checkOut <= checkIn)
            throw new ArgumentException("Check-out must be after check-in.");

        if (numberOfGuests <= 0 || numberOfGuests > roomType.MaxOccupancy())
            throw new ArgumentException($"{roomType} rooms cannot accommodate {numberOfGuests} guest(s).");

        RoomId = roomId;
        CheckIn = checkIn;
        CheckOut = checkOut;
        NumberOfGuests = numberOfGuests;
        Reference = reference;
    }

    public bool OverlapsWith(DateOnly otherCheckIn, DateOnly otherCheckOut)
        => CheckIn < otherCheckOut && otherCheckIn < CheckOut;
}