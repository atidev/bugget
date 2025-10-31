using Bugget.ExternalClients.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Bugget.ExternalClients.Notifications.Mattermost;

public static class MattermostExtensions
{
    private static readonly string? MattermostBaseUrl = Environment.GetEnvironmentVariable(MattermostConstants.MattermostBaseUrlKey);
    private static readonly string? MattermostBotAccessTokenName = Environment.GetEnvironmentVariable(MattermostConstants.MattermostBotAccessTokenKey);
    public static IServiceCollection AddMattermostNotifications(this IServiceCollection services)
    {
        if (string.IsNullOrEmpty(MattermostBaseUrl) || string.IsNullOrEmpty(MattermostBotAccessTokenName))
            return services;
        
        services.AddHttpClient(MattermostConstants.MattermostHttpClientKey, client =>
        {
            client.BaseAddress = new Uri(MattermostBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(5);
        });
        
        services.AddSingleton<MattermostClient>();
        services.AddSingleton<MattermostService>();
        
        services.AddSingleton<IReportPatchPostAction, MattermostService>(sp => sp.GetRequiredService<MattermostService>());
        return services;
    }
}