namespace SunsetBooking.Domain.Base;

public interface IUserContext
{
    string UserId { get; }
    string UserFullname { get; }
}