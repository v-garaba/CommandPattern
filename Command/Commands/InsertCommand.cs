using Command.Validation;

namespace Command.Commands;

internal sealed class InsertCommand(Document document, int position, string text)
    : ICommand<Document>
{
    private readonly Document _document = document;
    private readonly int _position = position.AssertPositive();
    private readonly string _text = text.AssertNotEmpty();

    /// <inheritdoc/>
    public Document Execute()
    {
        var doc = _document.Clone();
        doc = doc.InsertText(_position, _text);

        return doc;
    }

    /// <inheritdoc/>
    public Document Undo()
    {
        return _document.Clone();
    }

    /// <inheritdoc/>
    public string Description => $"Insert '{_text}' at position {_position}";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
