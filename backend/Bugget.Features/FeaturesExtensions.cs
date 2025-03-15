using Bugget.Features.Interfaces;
using Bugget.Features.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Bugget.Features;

public static class FeaturesExtensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddNotifications();
        
        // services.AddSingleton<WithoutFeaturesService>();
        //
        // services.AddSingleton<IReportCreatePostAction, WithoutFeaturesService>(sp => sp.GetRequiredService<WithoutFeaturesService>());
        // services.AddSingleton<IReportUpdatePostAction, WithoutFeaturesService>(sp => sp.GetRequiredService<WithoutFeaturesService>());
        
        services.AddSingleton<FeaturesService>();
        
        return services;
    }
}