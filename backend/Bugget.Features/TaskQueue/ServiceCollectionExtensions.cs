using Microsoft.Extensions.DependencyInjection;

namespace Bugget.Features.TaskQueue;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaskQueueHostedService(this IServiceCollection services) => services
        .AddSingleton<ITaskQueue, TaskQueue>()
        .AddHostedService(provider => (TaskQueue)provider.GetRequiredService<ITaskQueue>());
}