using Command.Commands;

namespace Command.History;

public interface IManagesHistory<TTarget>
    where TTarget : notnull
{
    void AddCommand(ICommand<TTarget> command);
    (TTarget Target, ICommand<TTarget> Command)? Undo();
    (TTarget Target, ICommand<TTarget> Command)? Redo();
    void Clear();
}
