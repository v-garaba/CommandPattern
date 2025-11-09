using Command.Common;
using Command.Memento;
using Command.Validation;

namespace Command.Commands;

internal sealed class DeleteCommand(Document document, int position, int length)
    : ICommandAsync
{
    private readonly Document _document = document.AssertNotNull();
    private readonly ISnapshot _snapshot = document.CreateSnapshot();
    private readonly int _position = position.AssertPositive();
    private readonly int _length = length.AssertPositive();

    /// <inheritdoc/>
    public async Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return await _document.DeleteTextAsync(_position, _length, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Result> UndoAsync(CancellationToken cancellationToken = default)
    {
        await _document.RestoreSnapshot(_snapshot, cancellationToken);
        return Result.Success();
    }

    /// <inheritdoc/>
    public string Description =>
        $"Delete {_length} characters at position {_position}";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
