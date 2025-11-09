using System.Threading.Tasks;
using Command.Common;
using Command.Memento;
using Command.Validation;

namespace Command.Commands;

internal sealed class InsertCommand(Document document, int position, string text)
    : ICommandAsync
{
    private readonly Document _document = document.AssertNotNull();
    private readonly ISnapshot _snapshot = document.CreateSnapshot();
    private readonly int _position = position.AssertPositive();
    private readonly string _text = text.AssertNotEmpty();

    /// <inheritdoc/>
    public async Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return await _document.InsertTextAsync(_position, _text, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Result> UndoAsync(CancellationToken cancellationToken = default)
    {
        await _document.RestoreSnapshot(_snapshot, cancellationToken);
        return Result.Success();
    }

    /// <inheritdoc/>
    public string Description => $"Insert '{_text}' at position {_position}";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
