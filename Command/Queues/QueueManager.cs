using Command.Commands;

namespace Command.Queues;

internal sealed class QueueManager : ICommandQueue
{
    private readonly Queue<ICommand> _commandQueue = new();

    public void Enqueue(ICommand command)
    {
        _commandQueue.Enqueue(command);
    }

    public ICommand? Dequeue()
    {
        _commandQueue.TryDequeue(out var command);
        return command;
    }
}
