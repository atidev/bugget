using System.Security.Claims;

namespace Authentication;

public class UserIdentity(ClaimsPrincipal principal)
{
    public string Id { get; init; } = principal.Identity?.Name ?? "undefined_id";
    public string? TeamId { get; init; } = principal.FindFirst("team_id")?.Value;
    public string? OrganizationId { get; init; } = principal.FindFirst("organization_id")?.Value;
    public string? SignalRConnectionId { get; init; } = principal.FindFirst("signalr_connection_id")?.Value;
}