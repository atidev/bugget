using Microsoft.AspNetCore.Authentication;

namespace Bugget.Authentication;

public static class ServiceCollectionExtensions
{
    public static AuthenticationBuilder AddAuthHeaders(this IServiceCollection services) =>
        services.AddAuthentication(options =>
    {
        options.DefaultScheme = AuthSchemeNames.Headers;
    })
            .AddScheme<AuthenticationSchemeOptions, UserAuthHandler>(
                AuthSchemeNames.Headers, o => { });
}