using Command.Memento;
using Command.Validation;

namespace Command.Commands;

internal sealed class MacroCommand(
    Document document,
    IEnumerable<Func<Document, ICommand<Document>>> commandFactories
) : ICommand<Document>
{
    private readonly Document _document = document.AssertNotNull();
    private readonly ISnapshot _snapshot = document.CreateSnapshot();
    private readonly IEnumerable<Func<Document, ICommand<Document>>> _commandFactories =
        commandFactories;
    private readonly List<ICommand<Document>> _executedCommands = new();

    /// <inheritdoc/>
    public void Execute()
    {
        _executedCommands.Clear();
        foreach (var factory in _commandFactories)
        {
            var command = factory(_document);
            command.Execute();
            _executedCommands.Add(command);
        }
    }

    /// <inheritdoc/>
    public void Undo()
    {
        _document.RestoreSnapshot(_snapshot);
    }

    /// <inheritdoc/>
    public string Description =>
        _executedCommands.Count > 0
            ? $"Macro Command: {string.Join(", ", _executedCommands.Select(c => c.Description))}"
            : "Macro Command: (not executed)";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
