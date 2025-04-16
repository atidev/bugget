using System.Security.Claims;
using System.Text.Encodings.Web;
using Bugget.Entities.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Bugget.Authentication;

public class UserAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private static readonly string? LdapUserIdKey = Environment.GetEnvironmentVariable(EnvironmentConstants.LdapUserIdKeyName);
    private static readonly string? LdapTeamIdKey = Environment.GetEnvironmentVariable(EnvironmentConstants.LdapTeamIdKeyName);
    private static readonly string? LdapOrganizationIdKey = Environment.GetEnvironmentVariable(EnvironmentConstants.LdapOrganizationIdKeyName);

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var headers = Request.Headers;

        var userId = string.Empty;
        if (!string.IsNullOrEmpty(LdapUserIdKey))
            userId = headers[LdapUserIdKey].ToString();

        if (string.IsNullOrEmpty(userId))
        {
            userId = "default-user";
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, userId)
        };

        if (!string.IsNullOrEmpty(LdapTeamIdKey))
        {
            var teamId = headers[LdapTeamIdKey].ToString();
            if (!string.IsNullOrEmpty(teamId))
            {
                claims.Add(new Claim("team_id", teamId));
            }
        }

        if (!string.IsNullOrEmpty(LdapOrganizationIdKey))
        {
            var organizationId = headers[LdapOrganizationIdKey].ToString();
            if (!string.IsNullOrEmpty(organizationId))
            {
                claims.Add(new Claim("organization_id", organizationId));
            }
        }

        return Task.FromResult(AuthenticateResult.Success(CreateTicket(claims)));
    }

    private AuthenticationTicket CreateTicket(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, Scheme.Name, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return ticket;
    }
}