using Command.Commands;
using Command.History;
using Command.Queues;
using Command.Validation;
using Microsoft.Extensions.Logging;

namespace Command.Operations;

/// <summary>
/// Executes commands and manages their history.
/// </summary>
/// <param name="historyManager">The history manager to use.</param>
internal sealed class CommandOperator(
    IManagesHistory historyManager,
    ICommandQueue commandQueue,
    ILogger<CommandOperator> logger
) : ICommandOperator
{
    private readonly IManagesHistory _historyManager = historyManager.AssertNotNull();
    private readonly ICommandQueue _commandQueue = commandQueue.AssertNotNull();
    private readonly ILogger<CommandOperator> _logger = logger.AssertNotNull();

    /// <inheritdoc/>
    public Task<bool> ExecuteCommandAsync(ICommandAsync command, CancellationToken cancellationToken = default)
    {
        var result = command.ExecuteAsync(cancellationToken);
        _historyManager.AddCommand(command.AssertNotNull());
        return result;
    }

    /// <inheritdoc/>
    public void QueueCommand(ICommandAsync command)
    {
        _commandQueue.Enqueue(command.AssertNotNull());
    }

    /// <inheritdoc/>
    public async Task<bool> ExecuteQueuedCommandsAsync(CancellationToken cancellationToken = default)
    {
        bool allSuccessful = true;
        ICommandAsync? command;
        while ((command = _commandQueue.Dequeue()) != null)
        {
            allSuccessful &= await ExecuteCommandAsync(command, cancellationToken);

            if (!allSuccessful)
            {
                _logger.LogWarning($"Execution of queued commands stopped due to {command.GetType().Name} failure.");
                break;
            }
        }

        return allSuccessful;
    }

    /// <inheritdoc/>
    public void ClearQueue()
    {
        while (_commandQueue.Dequeue() != null)
        {
            // Just dequeue to clear.
        }
    }

    /// <inheritdoc/>
    public string RevealHistory()
    {
        return _historyManager.ToString() ?? string.Empty;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _historyManager.Clear();
        ClearQueue();
    }

    /// <inheritdoc/>
    public Task<bool?> UndoLastCommandAsync(CancellationToken cancellationToken = default)
        => _historyManager.UndoAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<bool?> RedoLastCommandAsync(CancellationToken cancellationToken = default)
        => _historyManager.RedoAsync(cancellationToken);
}
