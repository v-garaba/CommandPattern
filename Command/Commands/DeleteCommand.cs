using Command.Validation;

namespace Command.Commands;

internal sealed class DeleteCommand(Document document, int position, int length)
    : ICommand<Document>
{
    private readonly Document _document = document;
    private readonly int _position = position.AssertPositive();
    private readonly int _length = length.AssertPositive();
    private string _undoDeletedText = string.Empty;

    /// <inheritdoc/>
    public Document Execute()
    {
        var doc = _document.Clone();
        _undoDeletedText = doc.GetText(_position, _length);
        doc = doc.DeleteText(_position, _length);
        return doc;
    }

    /// <inheritdoc/>
    public Document Undo()
    {
        return _document.Clone();
    }

    /// <inheritdoc/>
    public string Description =>
        $"Delete {_length} characters at position {_position} (was: '{_undoDeletedText}')";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
