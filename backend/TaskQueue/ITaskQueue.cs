namespace TaskQueue;

public interface ITaskQueue
{

    ValueTask Enqueue(Func<IServiceProvider, CancellationToken, Task> workItem);


    ValueTask Enqueue(Func<CancellationToken, Task> workItem);


    ValueTask Enqueue(Func<Task> workItem);
}