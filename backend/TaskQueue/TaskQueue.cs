using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TaskQueue;

public class TaskQueue(ILogger<TaskQueue> logger, IServiceProvider serviceProvider) 
    : BackgroundService, ITaskQueue
{
    private readonly Channel<TaskContext> _queue = Channel.CreateUnbounded<TaskContext>();

    private record TaskContext(Func<IServiceProvider, CancellationToken, Task> WorkItem, Activity Activity);

    public ValueTask Enqueue(Func<IServiceProvider, CancellationToken, Task> workItem)
    {
        var nestedActivity = new Activity($"{nameof(TaskQueue)}.{nameof(ExecuteAsync)}").Start();

        return _queue.Writer.WriteAsync(new TaskContext(workItem, nestedActivity));
    }

    public ValueTask Enqueue(Func<CancellationToken, Task> workItem) => Enqueue((_, token) => workItem(token));

    public ValueTask Enqueue(Func<Task> workItem) => Enqueue((_, _) => workItem());

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Parallel
        .ForEachAsync(_queue.Reader.ReadAllAsync(stoppingToken), new ParallelOptions
            {
                CancellationToken = stoppingToken,
                MaxDegreeOfParallelism = TaskScheduler.Current.MaximumConcurrencyLevel,
            },
            async (context, ct) =>
            {
                using var act = context.Activity;
                Activity.Current = act;

                try
                {
                    using var serviceScope = serviceProvider.CreateScope();
                    await context.WorkItem(serviceScope.ServiceProvider, ct);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "При выполнении фоновой задачи произошла ошибка");
                }
            });
}
