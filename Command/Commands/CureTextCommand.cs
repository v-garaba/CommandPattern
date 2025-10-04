namespace Command.Commands;

internal sealed class CureTextCommand(Document document) : ICommand
{
    private Document _document = document ?? throw new ArgumentNullException(nameof(document));
    private List<int> _foundColumns = [];
    private List<int> _foundSpaces = [];

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

        int[] foundSpaces =
        [
            .. _document
                .Content.Select((c, i) => (Char: c, Index: i))
                .Where(t => t.Char == ' ')
                .Select(t => t.Index),
        ];

        if (foundColumns.Any())
        {
            foreach (var columnIndex in foundColumns)
            {
                _document.ReplaceText(columnIndex, 1, "@");
                _foundColumns.Add(columnIndex);
            }
        }

        if (foundSpaces.Any())
        {
            foreach (var spaceIndex in foundSpaces)
            {
                _document.ReplaceText(spaceIndex, 1, "_");
                _foundSpaces.Add(spaceIndex);
            }
        }
    }

    /// <inheritdoc/>
    public void Undo()
    {
        if (_foundColumns.Any())
        {
            foreach (var columnIndex in _foundColumns)
            {
                _document.ReplaceText(columnIndex, 1, ":");
            }
            _foundColumns.Clear();
        }

        if (_foundSpaces.Any())
        {
            foreach (var spaceIndex in _foundSpaces)
            {
                _document.ReplaceText(spaceIndex, 1, " ");
            }
            _foundSpaces.Clear();
        }
    }

    /// <inheritdoc/>
    public string Description =>
        $"Cure text (replaced {_foundColumns.Count} columns and {_foundSpaces.Count} spaces)";
}
