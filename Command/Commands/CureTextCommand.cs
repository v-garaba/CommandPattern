using Command.Common;
using Command.Memento;
using Command.Validation;

namespace Command.Commands;

internal sealed class CureTextCommand(Document document) : ICommandAsync
{
    private readonly Document _document = document.AssertNotNull();
    private readonly ISnapshot _snapshot = document.CreateSnapshot();

    /// <inheritdoc/>
    public async Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Result result = Result.Success();
        Result<string> content = await _document.GetTextAsync(cancellationToken);
        int[] foundColumns =
        [
            .. content.Value
                .Select((c, i) => (Char: c, Index: i))
                .Where(t => t.Char == ':')
                .Select(t => t.Index),
        ];

        if (foundColumns.Length != 0)
        {
            foreach (var columnIndex in foundColumns)
            {
                result = await _document.ReplaceTextAsync(columnIndex, 1, "@", cancellationToken);
            }
        }

        if (!result.IsSuccess)
            return result;

        content = await _document.GetTextAsync(cancellationToken);
        int[] foundSpaces =
        [
            .. content.Value
                .Select((c, i) => (Char: c, Index: i))
                .Where(t => t.Char == ' ')
                .Select(t => t.Index),
        ];

        if (foundSpaces.Length != 0)
        {
            foreach (var spaceIndex in foundSpaces)
            {
                result = await _document.ReplaceTextAsync(spaceIndex, 1, "_", cancellationToken);
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<Result> UndoAsync(CancellationToken cancellationToken = default)
    {
        await _document.RestoreSnapshot(_snapshot, cancellationToken);
        return Result.Success();
    }

    /// <inheritdoc/>
    public string Description => $"Cure text by replacing ':' with '@' and ' ' with '_'";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
