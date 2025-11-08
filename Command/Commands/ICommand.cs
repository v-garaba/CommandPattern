namespace Command.Commands;

/// <summary>
/// Represents a command that can be executed and undone.
/// Implements the Command design pattern for encapsulating operations.
/// </summary>
public interface ICommand<TTarget>
    where TTarget : notnull
{
    /// <summary>
    /// Gets a human-readable description of the command.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executes the command, performing the operation on the target.
    /// </summary>
    TTarget Execute();

    /// <summary>
    /// Undoes the command, reverting the operation performed by Execute.
    /// </summary>
    TTarget Undo();
}
