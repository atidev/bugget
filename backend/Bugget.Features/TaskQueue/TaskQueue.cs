using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bugget.Features.TaskQueue;

internal class TaskQueue : BackgroundService, ITaskQueue
{
    private readonly ILogger<TaskQueue> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Channel<TaskContext> _queue;

    private record TaskContext(Func<IServiceProvider, CancellationToken, Task> WorkItem, Activity Activity);

    public TaskQueue(ILogger<TaskQueue> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _queue = Channel.CreateUnbounded<TaskContext>();
    }

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
                    using var serviceScope = _serviceProvider.CreateScope();
                    await context.WorkItem(serviceScope.ServiceProvider, ct);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "При выполнении фоновой задачи произошла ошибка");
                }
            });
}
