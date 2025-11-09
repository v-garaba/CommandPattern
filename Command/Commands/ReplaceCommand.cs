using Command.Memento;
using Command.Validation;

namespace Command.Commands;

internal sealed class ReplaceCommand(
    Document document,
    int position,
    int length,
    string replaceText
) : ICommand<Document>
{
    private readonly Document _document = document.AssertNotNull();
    private readonly ISnapshot _snapshot = document.CreateSnapshot();
    private readonly int _position = position.AssertPositive();
    private readonly int _length = length.AssertPositive();
    private readonly string _replaceText = replaceText.AssertNotEmpty();

    /// <inheritdoc/>
    public void Execute()
    {
        _document.ReplaceText(_position, _length, _replaceText);
    }

    /// <inheritdoc/>
    public void Undo()
    {
        _document.RestoreSnapshot(_snapshot);
    }

    /// <inheritdoc/>
    public string Description =>
        $"Replace text at position {_position}";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
