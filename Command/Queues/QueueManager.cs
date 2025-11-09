using Command.Commands;

namespace Command.Queues;

internal sealed class QueueManager : ICommandQueue
{
    private readonly Queue<ICommandAsync> _commandQueue = new();

    public void Enqueue(ICommandAsync command)
    {
        _commandQueue.Enqueue(command);
    }

    public ICommandAsync? Dequeue()
    {
        _commandQueue.TryDequeue(out var command);
        return command;
    }
}
