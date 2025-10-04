using Command.Validation;

namespace Command.Commands;

internal sealed class InsertCommand(Document document, int position, string text) : ICommand
{
    private readonly Document _document = document.AssertNotNull();
    private readonly int _position = position.AssertPositive();
    private readonly string _text = text.AssertNotEmpty();

    /// <inheritdoc/>
    public void Execute()
    {
        _document.InsertText(_position, _text);
    }

    /// <inheritdoc/>
    public void Undo()
    {
        _document.DeleteText(_position, _text.Length);
    }

    /// <inheritdoc/>
    public string Description => $"Insert '{_text}' at position {_position}";
}
