using Command.Common;

namespace Command.Commands;

/// <summary>
/// Represents a command that can be executed and undone.
/// Implements the Command design pattern for encapsulating operations.
/// </summary>
public interface ICommandAsync
{
    /// <summary>
    /// Gets a human-readable description of the command.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executes the command, performing the operation on the target.
    /// </summary>
    Task<Result> ExecuteAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Undoes the command, reverting the operation performed by Execute.
    /// </summary>
    Task<Result> UndoAsync(CancellationToken cancellationToken = default);
}
