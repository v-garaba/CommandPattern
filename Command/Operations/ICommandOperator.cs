using Command.Commands;

namespace Command.Operations;

/// <summary>
/// Interface for executing commands or managing command queues.
/// </summary>
public interface ICommandOperator : ICommandQueuer, ICommandExecuter { }
