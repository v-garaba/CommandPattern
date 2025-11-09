using Command.Commands;
using Command.Common;

namespace Command.History;

public interface IManagesHistory
{
    IReadOnlyDictionary<ICommandAsync, CommandStatus> HistoricalLog { get; }

    void AddCommand(ICommandAsync command);

    Task<Result<ICommandAsync?>> UndoAsync(CancellationToken cancellationToken = default);

    Task<Result<ICommandAsync?>> RedoAsync(CancellationToken cancellationToken = default);

    void Clear();
}
