using Microsoft.AspNetCore.Http;
using SunsetBooking.Domain.Shared.Consts;

namespace SunsetBooking.Domain.Base;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId 
    { 
        get 
        {
            var claimValue = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(
                c => c.Type == CustomClaimTypes.UserId)?.Value;
            if (string.IsNullOrEmpty(claimValue))
            {
                throw new ArgumentNullException(nameof(CustomClaimTypes.UserId), $"Claim type {CustomClaimTypes.UserId} is missing a value.");
            }
            
            return claimValue;  
        } 
    }
    
    public string? UserFullname
    {
        get 
        {
            var claimValue = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(
                c => c.Type == CustomClaimTypes.UserFullname)?.Value;
            if (string.IsNullOrEmpty(claimValue))
            {
                throw new ArgumentNullException(nameof(CustomClaimTypes.UserId), $"Claim type {CustomClaimTypes.UserFullname} is missing a value.");
            }
            return claimValue;  
        }
    }
}