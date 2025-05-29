using Bugget.ExternalClients.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Bugget.ExternalClients;

public static class ExternalClientsExtensions
{
    public static IServiceCollection AddExternalClients(this IServiceCollection services)
    {
        services.AddSingleton<ExternalClientsActionService>();
        
        services.AddNotifications();
        return services;
    }
}