namespace SunsetBooking.Domain.Base;

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }
}