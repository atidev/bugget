using Bugget.Features.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Bugget.Features.Notifications.Mattermost;

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
        
        services.AddSingleton<MattermostService>();
        
        services.AddSingleton<IReportCreatePostAction, MattermostService>(sp => sp.GetRequiredService<MattermostService>());
        services.AddSingleton<IReportUpdatePostAction, MattermostService>(sp => sp.GetRequiredService<MattermostService>());

        return services;
    }
}