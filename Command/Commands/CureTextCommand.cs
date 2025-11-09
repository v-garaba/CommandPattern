using Command.Memento;
using Command.Validation;

namespace Command.Commands;

internal sealed class CureTextCommand(Document document) : ICommand<Document>
{
    private readonly Document _document = document.AssertNotNull();
    private readonly ISnapshot _snapshot = document.CreateSnapshot();

    /// <inheritdoc/>
    public void Execute()
    {
        int[] foundColumns =
        [
            .. _document
                .Content.Select((c, i) => (Char: c, Index: i))
                .Where(t => t.Char == ':')
                .Select(t => t.Index),
        ];

        if (foundColumns.Length != 0)
        {
            foreach (var columnIndex in foundColumns)
            {
                _document.ReplaceText(columnIndex, 1, "@");
            }
        }

        int[] foundSpaces =
        [
            .. _document
                .Content.Select((c, i) => (Char: c, Index: i))
                .Where(t => t.Char == ' ')
                .Select(t => t.Index),
        ];

        if (foundSpaces.Length != 0)
        {
            foreach (var spaceIndex in foundSpaces)
            {
                _document.ReplaceText(spaceIndex, 1, "_");
            }
        }
    }

    /// <inheritdoc/>
    public void Undo()
    {
        _document.RestoreSnapshot(_snapshot);
    }

    /// <inheritdoc/>
    public string Description => $"Cure text by replacing ':' with '@' and ' ' with '_'";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
