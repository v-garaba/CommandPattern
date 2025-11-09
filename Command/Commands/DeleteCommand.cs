using Command.Memento;
using Command.Validation;

namespace Command.Commands;

internal sealed class DeleteCommand(Document document, int position, int length)
    : ICommand<Document>
{
    private readonly Document _document = document.AssertNotNull();
    private readonly ISnapshot _snapshot = document.CreateSnapshot();
    private readonly int _position = position.AssertPositive();
    private readonly int _length = length.AssertPositive();

    /// <inheritdoc/>
    public void Execute()
    {
        _document.DeleteText(_position, _length);
    }

    /// <inheritdoc/>
    public void Undo()
    {
        _document.RestoreSnapshot(_snapshot);
    }

    /// <inheritdoc/>
    public string Description =>
        $"Delete {_length} characters at position {_position}";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
