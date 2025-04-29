using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Authentication;

public class UserAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    private static readonly string? UserIdKey = Environment.GetEnvironmentVariable("USER_ID_KEY");

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var headers = Request.Headers;

        var userId = string.Empty;
        if (!string.IsNullOrEmpty(UserIdKey))
            userId = headers[UserIdKey].ToString();

        if (string.IsNullOrEmpty(userId))
        {
            userId = "default-user";
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
        };

        return Task.FromResult(AuthenticateResult.Success(CreateTicket(claims)));
    }

    private AuthenticationTicket CreateTicket(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, Scheme.Name, ClaimTypes.NameIdentifier, ClaimsIdentity.DefaultRoleClaimType);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return ticket;
    }
}