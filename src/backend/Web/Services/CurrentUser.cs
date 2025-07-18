using System.Security.Claims;
using FiTrack.Application.Interfaces.Auth;
using Shared.Constants;

namespace Web.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public int? Id => GetIntClaim(StaticClaims.UserId);
    public int? RoleId => GetIntClaim(StaticClaims.RoleId);

    private int? GetIntClaim(string claimType)
    {
        var claimValue = httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType);
        return int.TryParse(claimValue, out var result) ? result : null;
    }
}
