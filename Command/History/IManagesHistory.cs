using Command.Commands;

namespace Command.History;

public interface IManagesHistory
{
    IReadOnlyDictionary<ICommandAsync, CommandStatus> HistoricalLog { get; }

    void AddCommand(ICommandAsync command);

    Task<bool?> UndoAsync(CancellationToken cancellationToken = default);

    Task<bool?> RedoAsync(CancellationToken cancellationToken = default);

    void Clear();
}
