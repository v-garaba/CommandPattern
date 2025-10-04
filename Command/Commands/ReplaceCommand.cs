using Command.Validation;

namespace Command.Commands;

internal sealed class ReplaceCommand(
    Document document,
    int position,
    int length,
    string replaceText
) : ICommand
{
    private readonly Document _document = document.AssertNotNull();
    private readonly int _position = position.AssertPositive();
    private readonly int _length = length.AssertPositive();
    private readonly string _replaceText = replaceText.AssertNotEmpty();
    private string _undoReplaceText = string.Empty;

    /// <inheritdoc/>
    public void Execute()
    {
        _undoReplaceText = _document.GetText(_position, _length);
        _document.ReplaceText(_position, _length, _replaceText);
    }

    /// <inheritdoc/>
    public void Undo()
    {
        _document.ReplaceText(_position, _replaceText.Length, _undoReplaceText);
    }

    /// <inheritdoc/>
    public string Description =>
        $"Replace text at position {_position} (was: '{_undoReplaceText}')";
}
