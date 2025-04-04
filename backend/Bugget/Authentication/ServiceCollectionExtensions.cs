using Microsoft.AspNetCore.Authentication;

namespace Bugget.Authentication;

public static class ServiceCollectionExtensions
{
    public static AuthenticationBuilder AddLdapAuth(this IServiceCollection services) =>
        services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, UserAuthHandler>(
                AuthSchemeNames.Ldap, o => { });
}