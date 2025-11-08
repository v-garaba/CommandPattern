namespace Command.Operations;

/// <summary>
/// Interface for executing commands or managing command queues.
/// </summary>
public interface ICommandOperator<TTarget> : ICommandQueuer<TTarget>, ICommandExecuter<TTarget>
    where TTarget : notnull { }
