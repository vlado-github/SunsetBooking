using System.Security.Claims;

namespace SunsetBooking.Domain.Shared.Consts;

public class CustomClaimTypes
{
    public const string UserId = ClaimTypes.NameIdentifier;
    public const string UserFullname = "name";
}