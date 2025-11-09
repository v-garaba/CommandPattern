using Command.Memento;
using Command.Validation;

namespace Command.Commands;

internal sealed class ReplaceCommand(
    Document document,
    int position,
    int length,
    string replaceText
) : ICommandAsync
{
    private readonly Document _document = document.AssertNotNull();
    private readonly ISnapshot _snapshot = document.CreateSnapshot();
    private readonly int _position = position.AssertPositive();
    private readonly int _length = length.AssertPositive();
    private readonly string _replaceText = replaceText.AssertNotEmpty();

    /// <inheritdoc/>
    public async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return await _document.ReplaceTextAsync(_position, _length, _replaceText, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> UndoAsync(CancellationToken cancellationToken = default)
    {
        await _document.RestoreSnapshot(_snapshot, cancellationToken);
        return true;
    }

    /// <inheritdoc/>
    public string Description =>
        $"Replace text at position {_position}";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
