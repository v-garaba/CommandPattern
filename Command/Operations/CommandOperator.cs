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
internal sealed class CommandOperator<TTarget>(
    IManagesHistory<TTarget> historyManager,
    ICommandQueue<TTarget> commandQueue,
    ILogger<CommandOperator<TTarget>> logger
) : ICommandOperator<TTarget>
    where TTarget : notnull
{
    private readonly IManagesHistory<TTarget> _historyManager = historyManager.AssertNotNull();
    private readonly ICommandQueue<TTarget> _commandQueue = commandQueue.AssertNotNull();
    private readonly ILogger<CommandOperator<TTarget>> _logger = logger.AssertNotNull();

    /// <inheritdoc/>
    public void ExecuteCommand(ICommand<TTarget> command)
    {
        try
        {
            command.Execute();
            _historyManager.AddCommand(command.AssertNotNull());
        }
        catch (Exception ex)
        {
            // Tell user that command execution failed but do not throw. Outside of the scope.
            _logger.LogError(ex, "Failed to execute command: {Command}", command);
        }
    }

    /// <inheritdoc/>
    public void QueueCommand(ICommand<TTarget> command)
    {
        _commandQueue.Enqueue(command.AssertNotNull());
    }

    /// <inheritdoc/>
    public void ExecuteQueuedCommands()
    {
        ICommand<TTarget>? command;
        while ((command = _commandQueue.Dequeue()) != null)
        {
            ExecuteCommand(command);
        }
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
    public ICommand<TTarget>? UndoLastCommand() => _historyManager.Undo();

    /// <inheritdoc/>
    public ICommand<TTarget>? RedoLastCommand() => _historyManager.Redo();
}
