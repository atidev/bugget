using System.Security.Claims;

namespace Bugget.Authentication;

public static class UserIdentityExtensions
{
    public static UserIdentity GetIdentity(this ClaimsPrincipal principal) => new(principal);
}