namespace Command.Commands;

internal sealed class MacroCommand(
    Document document,
    IEnumerable<Func<Document, ICommand<Document>>> commandFactories
) : ICommand<Document>
{
    private readonly Document _document = document;
    private readonly IEnumerable<Func<Document, ICommand<Document>>> _commandFactories =
        commandFactories;
    private readonly List<ICommand<Document>> _executedCommands = new();

    /// <inheritdoc/>
    public Document Execute()
    {
        _executedCommands.Clear();
        var currentDoc = _document;

        foreach (var factory in _commandFactories)
        {
            var command = factory(currentDoc);
            currentDoc = command.Execute();
            _executedCommands.Add(command);
        }

        return currentDoc;
    }

    /// <inheritdoc/>
    public Document Undo()
    {
        return _document.Clone();
    }

    /// <inheritdoc/>
    public string Description =>
        _executedCommands.Count > 0
            ? $"Macro Command: {string.Join(", ", _executedCommands.Select(c => c.Description))}"
            : "Macro Command: (not executed)";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
