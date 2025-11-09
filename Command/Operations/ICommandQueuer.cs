using Command.Commands;

namespace Command.Operations;

/// <summary>
/// Interface for queuing and executing queues of commands.
/// </summary>
public interface ICommandQueuer
{
    /// <summary>
    /// Queues a command for later execution.
    /// </summary>
    /// <param name="command">The command to queue.</param>
    void QueueCommand(ICommandAsync command);

    /// <summary>
    /// Executes all queued commands.
    /// </summary>
    Task<bool> ExecuteQueuedCommandsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all queued commands without executing them.
    /// </summary>
    void ClearQueue();
}
