using System.Collections.Immutable;
using Command.Commands;

namespace Command.History;

internal sealed class HistoryManager : IManagesHistory
{
    private const int MaxHistorySize = 20;
    private readonly Stack<ICommand> _commandStack = new();
    private readonly Stack<ICommand> _redoStack = new();
    private ImmutableArray<(CommandStatus Status, ICommand Command)> _historicalLog = [];

    /// <inheritdoc/>
    public void AddCommand(ICommand command)
    {
        _commandStack.Push(command);
        _historicalLog = _historicalLog.Add((CommandStatus.Executed, command));

        // Maintain history size
        if (_historicalLog.Length > MaxHistorySize)
        {
            _historicalLog = _historicalLog.RemoveAt(0);
        }
    }

    /// <inheritdoc/>
    public ICommand? Undo()
    {
        if (_commandStack.TryPop(out var command))
        {
            command.Undo();
            _redoStack.Push(command);
            _historicalLog = _historicalLog.Add((CommandStatus.Undone, command));
            return command;
        }

        return null;
    }

    /// <inheritdoc/>
    public ICommand? Redo()
    {
        if (_redoStack.TryPop(out var command))
        {
            command.Execute();
            _commandStack.Push(command);
            _historicalLog = _historicalLog.Add((CommandStatus.Redone, command));
            return command;
        }
        return null;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _commandStack.Clear();
        _historicalLog = [];
    }

    public override string ToString()
    {
        if (_historicalLog.Length == 0)
        {
            return "No commands executed.";
        }

        string[] logEntries =
        [
            .. _historicalLog.Select(
                (entry, index) => $"{index + 1}. {entry.Status}: {entry.Command.Description}"
            ),
        ];

        return string.Join(Environment.NewLine, logEntries);
    }
}
