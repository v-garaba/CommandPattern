namespace Command.Commands;

internal sealed class CureTextCommand(Document document) : ICommand<Document>
{
    private readonly Document _document = document;

    /// <inheritdoc/>
    public Document Execute()
    {
        var doc = _document.Clone();
        int[] foundColumns =
        [
            .. doc
                .Content.Select((c, i) => (Char: c, Index: i))
                .Where(t => t.Char == ':')
                .Select(t => t.Index),
        ];

        if (foundColumns.Length != 0)
        {
            foreach (var columnIndex in foundColumns)
            {
                doc = doc.ReplaceText(columnIndex, 1, "@");
            }
        }

        int[] foundSpaces =
        [
            .. doc
                .Content.Select((c, i) => (Char: c, Index: i))
                .Where(t => t.Char == ' ')
                .Select(t => t.Index),
        ];

        if (foundSpaces.Length != 0)
        {
            foreach (var spaceIndex in foundSpaces)
            {
                doc = doc.ReplaceText(spaceIndex, 1, "_");
            }
        }

        return doc;
    }

    /// <inheritdoc/>
    public Document Undo()
    {
        return _document.Clone();
    }

    /// <inheritdoc/>
    public string Description => $"Cure text by replacing ':' with '@' and ' ' with '_'";

    /// <inheritdoc/>
    public override string? ToString() => Description;
}
