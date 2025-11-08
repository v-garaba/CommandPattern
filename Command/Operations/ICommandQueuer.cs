using Command.Commands;

namespace Command.Operations;

/// <summary>
/// Interface for queuing and executing queues of commands.
/// </summary>
public interface ICommandQueuer<TTarget>
    where TTarget : notnull
{
    /// <summary>
    /// Queues a command for later execution.
    /// </summary>
    /// <param name="command">The command to queue.</param>
    void QueueCommand(ICommand<TTarget> command);

    /// <summary>
    /// Executes all queued commands.
    /// </summary>
    TTarget? ExecuteQueuedCommands();

    /// <summary>
    /// Clears all queued commands without executing them.
    /// </summary>
    void ClearQueue();
}
