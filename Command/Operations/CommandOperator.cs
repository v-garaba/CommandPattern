using Command.Commands;
using Command.Common;
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
    public Task<Result> ExecuteCommandAsync(ICommandAsync command, CancellationToken cancellationToken = default)
    {
        _historyManager.AddCommand(command.AssertNotNull());
        return command.ExecuteAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public void QueueCommand(ICommandAsync command)
    {
        _commandQueue.Enqueue(command.AssertNotNull());
    }

    /// <inheritdoc/>
    public async Task<Result> ExecuteQueuedCommandsAsync(CancellationToken cancellationToken = default)
    {
        Result result = Result.Success();
        ICommandAsync? command;
        while ((command = _commandQueue.Dequeue()) != null)
        {
            result = await ExecuteCommandAsync(command, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning($"Execution of {command.GetType().Name} failed: {result.ErrorMessage}");
                break;
            }
        }

        return result;
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
    public Task<Result<ICommandAsync?>> UndoLastCommandAsync(CancellationToken cancellationToken = default)
        => _historyManager.UndoAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<Result<ICommandAsync?>> RedoLastCommandAsync(CancellationToken cancellationToken = default)
        => _historyManager.RedoAsync(cancellationToken);
}
