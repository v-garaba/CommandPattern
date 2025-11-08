using System.Collections.Immutable;
using Command.Commands;

namespace Command.History;

internal sealed class HistoryManager<TTarget> : IManagesHistory<TTarget>
    where TTarget : notnull
{
    private const int MaxHistorySize = 20;
    private readonly Stack<ICommand<TTarget>> _commandStack = new();
    private readonly Stack<ICommand<TTarget>> _redoStack = new();
    private ImmutableArray<(CommandStatus Status, ICommand<TTarget> Command)> _historicalLog = [];

    /// <inheritdoc/>
    public void AddCommand(ICommand<TTarget> command)
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
    public (TTarget Target, ICommand<TTarget> Command)? Undo()
    {
        if (_commandStack.TryPop(out var command))
        {
            var target = command.Undo();
            _redoStack.Push(command);
            _historicalLog = _historicalLog.Add((CommandStatus.Undone, command));
            return (target, command);
        }

        return null;
    }

    /// <inheritdoc/>
    public (TTarget Target, ICommand<TTarget> Command)? Redo()
    {
        if (_redoStack.TryPop(out var command))
        {
            var target = command.Execute();
            _commandStack.Push(command);
            _historicalLog = _historicalLog.Add((CommandStatus.Redone, command));
            return (target, command);
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
