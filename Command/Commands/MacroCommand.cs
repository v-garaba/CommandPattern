using Command.Memento;
using Command.Validation;

namespace Command.Commands;

internal sealed class MacroCommand(
    Document document,
    IEnumerable<Func<Document, ICommandAsync>> commandFactories
) : ICommandAsync
{
    private readonly Document _document = document.AssertNotNull();
    private readonly ISnapshot _snapshot = document.CreateSnapshot();
    private readonly IEnumerable<Func<Document, ICommandAsync>> _commandFactories =
        commandFactories;
    private readonly List<ICommandAsync> _executedCommands = [];

    /// <inheritdoc/>
    public async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        bool result = true;
        _executedCommands.Clear();
        foreach (var factory in _commandFactories)
        {
            var command = factory(_document);
            result &= await command.ExecuteAsync(cancellationToken);
            _executedCommands.Add(command);

            if (!result)
                break;
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<bool> UndoAsync(CancellationToken cancellationToken = default)
    {
        await _document.RestoreSnapshot(_snapshot, cancellationToken);
        return true;
    }

    /// <inheritdoc/>
    public string Description =>
        _executedCommands.Count > 0
            ? $"Macro Command: {string.Join(", ", _executedCommands.Select(c => c.Description))}"
            : "Macro Command: (not executed)";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
