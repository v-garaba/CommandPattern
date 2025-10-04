using Command.Commands;

namespace Command.History;

public interface IManagesHistory
{
    void AddCommand(ICommand command);
    ICommand? Undo();
    ICommand? Redo();
    void Clear();
}
