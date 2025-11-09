using Command.Commands;

namespace Command.NUnit.Commands;

[TestFixture]
public class MacroCommandTests
{
    [Test]
    public async Task Execute_WithEmptyCommands_ShouldReturnClonedDocument()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Test");
        var command = new MacroCommand(document, new List<Func<Document, ICommandAsync>>());

        // Act
        await command.ExecuteAsync();

        // Assert
        string content = await document.GetTextAsync();
        Assert.That(content, Is.EqualTo("Test"));
    }

    [Test]
    public async Task Execute_WithSameDocumentState_ThrowsOnInvalidPositions()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Start");

        // Commands that would work if chained
        var commands = new List<Func<Document, ICommandAsync>>
        {
            doc => new InsertCommand(doc, 5, " A"),
            doc => new InsertCommand(doc, 7, "B"),
            doc => new InsertCommand(doc, 8, "C"),
        };

        var macroCommand = new MacroCommand(document, commands);

        // Act & Assert
        await macroCommand.ExecuteAsync();

        string content = await document.GetTextAsync();
        Assert.That(content, Is.EqualTo("Start ABC"));
    }

    [Test]
    public async Task Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Test");

        var commands = new List<Func<Document, ICommandAsync>>
        {
            doc => new DeleteCommand(doc, 0, 2),
        };

        var macroCommand = new MacroCommand(document, commands);
        await macroCommand.ExecuteAsync();

        // Act
        await macroCommand.UndoAsync();

        // Assert
        string content = await document.GetTextAsync();
        Assert.That(content, Is.EqualTo("Test"));
    }

    [Test]
    public async Task Description_ShouldIncludeAllSubCommands()
    {
        // Arrange
        var document = new Document();
        var commands = new List<Func<Document, ICommandAsync>>
        {
            doc => new InsertCommand(doc, 0, "A"),
            doc => new InsertCommand(doc, 0, "B"),
        };

        var macroCommand = new MacroCommand(document, commands);

        // Act
        await macroCommand.ExecuteAsync();
        var description = macroCommand.Description;

        // Assert
        Assert.That(description, Does.Contain("Macro"));
        Assert.That(description, Does.Contain("Insert"));
    }
}
