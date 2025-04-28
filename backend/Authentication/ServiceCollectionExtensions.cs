using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication;

public static class ServiceCollectionExtensions
{
    public static AuthenticationBuilder AddAuthHeaders(this IServiceCollection services) =>
        services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, UserAuthHandler>(
                AuthSchemeNames.Headers, o => { });
}