using System.Security.Claims;

namespace Authentication;

public static class UserIdentityExtensions
{
    public static UserIdentity GetIdentity(this ClaimsPrincipal principal) => new(principal);
}