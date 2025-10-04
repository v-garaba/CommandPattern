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
    public void ExecuteCommand(ICommand command)
    {
        try
        {
            command.Execute();
            _historyManager.AddCommand(command.AssertNotNull());
        }
        catch
        {
            // Tell user that command execution failed but do not throw. Outside of the scope.
            _logger.LogError("Failed to execute command: {Command}", command);
        }
    }

    /// <inheritdoc/>
    public void QueueCommand(ICommand command)
    {
        _commandQueue.Enqueue(command.AssertNotNull());
    }

    /// <inheritdoc/>
    public void ExecuteQueuedCommands()
    {
        ICommand? command;
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
    public ICommand? UndoLastCommand() => _historyManager.Undo();

    /// <inheritdoc/>
    public ICommand? RedoLastCommand() => _historyManager.Redo();
}
