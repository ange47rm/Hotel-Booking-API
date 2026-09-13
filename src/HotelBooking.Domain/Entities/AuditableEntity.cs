namespace HotelBooking.Domain.Entities;

public abstract class AuditableEntity
{
    //public string CreatedBy { get; private set; }
    //public string ModifiedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    //public DateTime ModifiedAt { get; private set; }
}