using Command.Commands;
using Command.Common;

namespace Command.Operations;

/// <summary>
/// Interface for executing commands and managing their history.
/// </summary>
public interface ICommandExecuter
{
    /// <summary>
    /// Executes a command and adds it to the history.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    Task<Result> ExecuteCommandAsync(ICommandAsync command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Undoes the last executed command.
    /// </summary>
    /// <returns>Whether the undo was successful, or null if there is no command to undo.</returns>
    Task<Result<ICommandAsync?>> UndoLastCommandAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Redoes the last undone command.
    /// </summary>
    /// <returns>Whether the redo was successful, or null if there is no command to redo.</returns>
    Task<Result<ICommandAsync?>> RedoLastCommandAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a string representation of the command history.
    /// </summary>
    /// <returns>The command history as a string.</returns>
    string RevealHistory();

    /// <summary>
    /// Clears the command history and queue.
    /// </summary>
    void Clear();
}
