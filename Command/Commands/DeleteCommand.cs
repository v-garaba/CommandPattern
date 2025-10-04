using Command.Validation;

namespace Command.Commands;

internal sealed class DeleteCommand(Document document, int position, int length) : ICommand
{
    private readonly Document _document = document.AssertNotNull();
    private readonly int _position = position.AssertPositive();
    private readonly int _length = length.AssertPositive();
    private string _undoDeletedText = string.Empty;

    /// <inheritdoc/>
    public void Execute()
    {
        _undoDeletedText = _document.GetText(_position, _length);
        _document.DeleteText(_position, _length);
    }

    /// <inheritdoc/>
    public void Undo()
    {
        _document.InsertText(_position, _undoDeletedText);
    }

    /// <inheritdoc/>
    public string Description =>
        $"Delete {_length} characters at position {_position} (was: '{_undoDeletedText}')";
}
