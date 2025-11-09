using System.Collections.Immutable;
using Command.Commands;

namespace Command.History;

internal sealed class HistoryManager : IManagesHistory
{
    private const int MaxHistorySize = 20;
    private readonly Stack<ICommandAsync> _commandStack = new();
    private readonly Stack<ICommandAsync> _redoStack = new();
    private Dictionary<ICommandAsync, CommandStatus> _historicalLog = [];

    public IReadOnlyDictionary<ICommandAsync, CommandStatus> HistoricalLog =>
        _historicalLog.ToImmutableDictionary();

    /// <inheritdoc/>
    public void AddCommand(ICommandAsync command)
    {
        _commandStack.Push(command);
        _redoStack.Clear();
        _historicalLog[command] = CommandStatus.Executed;

        // Maintain history size
        if (_historicalLog.Count > MaxHistorySize)
        {
            var oldestCommand = _historicalLog.Keys.First();
            _historicalLog.Remove(oldestCommand);
        }
    }

    /// <inheritdoc/>
    public async Task<bool?> UndoAsync(CancellationToken cancellationToken = default)
    {
        if (_commandStack.TryPop(out var command))
        {
            _redoStack.Push(command);
            _historicalLog[command] = CommandStatus.Undone;
            return await command.UndoAsync(cancellationToken);
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task<bool?> RedoAsync(CancellationToken cancellationToken = default)
    {
        if (_redoStack.TryPop(out var command))
        {
            _commandStack.Push(command);
            _historicalLog[command] = CommandStatus.Redone;
            return await command.ExecuteAsync(cancellationToken);
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
        if (_historicalLog.Count == 0)
        {
            return "No commands executed.";
        }

        string[] logEntries =
        [
            .. _historicalLog.Select(
                (entry, index) => $"{index + 1}. {entry.Key.Description}: {entry.Value}"
            ),
        ];

        return string.Join(Environment.NewLine, logEntries);
    }
}
