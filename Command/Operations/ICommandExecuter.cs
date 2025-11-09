using Command.Commands;

namespace Command.Operations;

/// <summary>
/// Interface for executing commands and managing their history.
/// </summary>
public interface ICommandExecuter<TTarget>
    where TTarget : notnull
{
    /// <summary>
    /// Executes a command and adds it to the history.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    void ExecuteCommand(ICommand<TTarget> command);

    /// <summary>
    /// Undoes the last executed command.
    /// </summary>
    /// <returns>The command that was undone, or null if there is no command to undo.</returns>
    ICommand<TTarget>? UndoLastCommand();

    /// <summary>
    /// Redoes the last undone command.
    /// </summary>
    /// <returns>The command that was redone, or null if there is no command to redo.</returns>
    ICommand<TTarget>? RedoLastCommand();

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
