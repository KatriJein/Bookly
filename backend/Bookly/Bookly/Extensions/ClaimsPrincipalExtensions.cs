using System.Security.Claims;

namespace Bookly.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid RetrieveUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userIdString = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        var parsed = Guid.TryParse(userIdString, out var userId);
        return parsed ? userId : Guid.Empty;
    } 
}