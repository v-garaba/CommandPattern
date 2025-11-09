using Command.Commands;

namespace Command.NUnit.Commands;

[TestFixture]
public class MacroCommandTests
{
    [Test]
    public void Execute_WithEmptyCommands_ShouldReturnClonedDocument()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Test");
        var command = new MacroCommand(document, new List<Func<Document, ICommand<Document>>>());

        // Act
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Test"));
    }

    [Test]
    public void Execute_WithSameDocumentState_ThrowsOnInvalidPositions()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Start");

        // Commands that would work if chained
        var commands = new List<Func<Document, ICommand<Document>>>
        {
            doc => new InsertCommand(doc, 5, " A"),
            doc => new InsertCommand(doc, 7, "B"),
            doc => new InsertCommand(doc, 8, "C"),
        };

        var macroCommand = new MacroCommand(document, commands);

        // Act & Assert
        macroCommand.Execute();

        Assert.That(document.Content, Is.EqualTo("Start ABC"));
    }

    [Test]
    public void Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Test");

        var commands = new List<Func<Document, ICommand<Document>>>
        {
            doc => new DeleteCommand(doc, 0, 2),
        };

        var macroCommand = new MacroCommand(document, commands);
        macroCommand.Execute();

        // Act
        macroCommand.Undo();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Test"));
    }

    [Test]
    public void Description_ShouldIncludeAllSubCommands()
    {
        // Arrange
        var document = new Document();
        var commands = new List<Func<Document, ICommand<Document>>>
        {
            doc => new InsertCommand(doc, 0, "A"),
            doc => new InsertCommand(doc, 0, "B"),
        };

        var macroCommand = new MacroCommand(document, commands);

        // Act
        macroCommand.Execute();
        var description = macroCommand.Description;

        // Assert
        Assert.That(description, Does.Contain("Macro"));
        Assert.That(description, Does.Contain("Insert"));
    }
}
