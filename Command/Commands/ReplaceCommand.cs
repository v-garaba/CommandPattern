using Command.Validation;

namespace Command.Commands;

internal sealed class ReplaceCommand(
    Document document,
    int position,
    int length,
    string replaceText
) : ICommand<Document>
{
    private readonly Document _document = document;
    private readonly int _position = position.AssertPositive();
    private readonly int _length = length.AssertPositive();
    private readonly string _replaceText = replaceText.AssertNotEmpty();
    private string _undoReplaceText = string.Empty;

    /// <inheritdoc/>
    public Document Execute()
    {
        var doc = _document.Clone();
        _undoReplaceText = doc.GetText(_position, _length);
        doc = doc.ReplaceText(_position, _length, _replaceText);
        return doc;
    }

    /// <inheritdoc/>
    public Document Undo()
    {
        return _document.Clone();
    }

    /// <inheritdoc/>
    public string Description =>
        $"Replace text at position {_position} (was: '{_undoReplaceText}')";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
