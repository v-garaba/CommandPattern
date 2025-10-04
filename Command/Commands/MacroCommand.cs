namespace Command.Commands;

internal sealed class MacroCommand(IEnumerable<ICommand> commands) : ICommand
{
    private readonly IEnumerable<ICommand> _commands = commands;

    /// <inheritdoc/>
    public void Execute()
    {
        foreach (var command in _commands)
        {
            command.Execute();
        }
    }

    /// <inheritdoc/>
    public void Undo()
    {
        foreach (var command in _commands.Reverse())
        {
            command.Undo();
        }
    }

    /// <inheritdoc/>
    public string Description =>
        $"Macro Command: {string.Join(", ", _commands.Select(c => c.Description))}";
}
