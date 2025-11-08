using Command.Commands;

namespace Command.Queues;

internal sealed class QueueManager<TTarget> : ICommandQueue<TTarget>
    where TTarget : notnull
{
    private readonly Queue<ICommand<TTarget>> _commandQueue = new();

    public void Enqueue(ICommand<TTarget> command)
    {
        _commandQueue.Enqueue(command);
    }

    public ICommand<TTarget>? Dequeue()
    {
        _commandQueue.TryDequeue(out var command);
        return command;
    }
}
