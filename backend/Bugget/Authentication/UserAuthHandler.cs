using System.Security.Claims;
using System.Text.Encodings.Web;
using Bugget.DA.Interfaces;
using Bugget.Entities.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Bugget.Authentication;

public class UserAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IEmployeesClient employeesClient,
    IOptionsMonitor<AuthHeadersOptions> authHeadersOptions)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private const string DefaultUserId = "default-user";
    private readonly string? UserIdHeader = authHeadersOptions.CurrentValue.UserIdHeaderName;
    private readonly string? TeamIdHeader = authHeadersOptions.CurrentValue.TeamIdHeaderName;
    private readonly string? OrganizationIdHeader = authHeadersOptions.CurrentValue.OrganizationIdHeaderName;

    private const string SignalRConnectionIdHeader = "X-Signal-R-Conntection-Id";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var headers = Request.Headers;

        var userId = GetHeaderOrDefault(headers, UserIdHeader, DefaultUserId);
        if (string.IsNullOrWhiteSpace(userId))
            return Fail("User ID not found");

        var teamId = await ResolveTeamIdAsync(userId, headers);
        if (string.IsNullOrEmpty(teamId) && !string.IsNullOrEmpty(TeamIdHeader))
            return Fail("Team ID not found");

        var organizationId = GetHeaderOrDefault(headers, OrganizationIdHeader);
        if (!string.IsNullOrEmpty(OrganizationIdHeader) && string.IsNullOrWhiteSpace(organizationId))
            return Fail("Organization ID not found");

        var signalRId = GetHeaderOrDefault(headers, SignalRConnectionIdHeader);

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        if (!string.IsNullOrEmpty(teamId))
        {
            claims.Add(new Claim("team_id", teamId));
        }

        if (!string.IsNullOrEmpty(organizationId))
        {
            claims.Add(new Claim("organization_id", organizationId));
        }

        if (!string.IsNullOrEmpty(signalRId))
        {
            claims.Add(new Claim("signalr_connection_id", signalRId));
        }

        return AuthenticateResult.Success(CreateTicket(claims));
    }

    private async Task<string?> ResolveTeamIdAsync(string userId, IHeaderDictionary headers)
    {
        if (!string.IsNullOrEmpty(TeamIdHeader))
        {
            var teamId = headers[TeamIdHeader].ToString();
            if (!string.IsNullOrWhiteSpace(teamId))
                return teamId;
        }

        var employee = await employeesClient.GetEmployeeAsync(userId);
        return employee?.TeamId;
    }

    private static string? GetHeaderOrDefault(IHeaderDictionary headers, string? headerName, string? defaultValue = null)
    {
        if (!string.IsNullOrEmpty(headerName))
        {
            if (headers.TryGetValue(headerName, out var values))
                return values.ToString();
            return null;
        }

        return defaultValue;
    }

    private static AuthenticateResult Fail(string reason) =>
        AuthenticateResult.Fail(reason);

    private AuthenticationTicket CreateTicket(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, Scheme.Name, ClaimTypes.NameIdentifier, ClaimsIdentity.DefaultRoleClaimType);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return ticket;
    }
}