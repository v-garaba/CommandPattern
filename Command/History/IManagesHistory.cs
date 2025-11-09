using Command.Commands;

namespace Command.History;

public interface IManagesHistory<TTarget>
    where TTarget : notnull
{
    void AddCommand(ICommand<TTarget> command);
    ICommand<TTarget>? Undo();
    ICommand<TTarget>? Redo();
    void Clear();
}
