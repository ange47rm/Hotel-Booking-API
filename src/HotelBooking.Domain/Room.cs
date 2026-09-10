namespace HotelBooking.Domain
{
    public class Room
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int RoomNumber { get; set; }
        public RoomType Type { get; set; }
    }
}
