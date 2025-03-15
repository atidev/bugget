using System.Security.Claims;

namespace Bugget.Authentication;

public class UserIdentity(ClaimsPrincipal principal)
{
    public string Id { get; init; } = principal.Identity?.Name ?? "undefined_id";
}