using Microsoft.AspNetCore.Authorization;

namespace Authentication;

/// <summary>
/// Аутентификация по LDAP
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class LdapAuthAttribute : AuthorizeAttribute
{
    public LdapAuthAttribute()
    {
        AuthenticationSchemes = AuthSchemeNames.Ldap;
    }
}