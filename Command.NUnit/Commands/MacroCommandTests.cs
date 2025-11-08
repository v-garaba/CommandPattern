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
        document = document.InsertText(0, "Test");
        var command = new MacroCommand(document, new List<Func<Document, ICommand<Document>>>());

        // Act
        var result = command.Execute();

        // Assert
        Assert.That(result.Content, Is.EqualTo("Test"));
    }

    [Test]
    public void Execute_DemonstratesBrokenChaining()
    {
        // Arrange - This test demonstrates the bug
        var document = new Document();

        // All commands created with the same initial empty document
        var commands = new List<Func<Document, ICommand<Document>>>
        {
            doc => new InsertCommand(doc, 0, "A"), // Should insert at 0
            doc => new InsertCommand(doc, 1, "B"), // Expects position 1 exists
            doc => new InsertCommand(doc, 2, "C"), // Expects position 2 exists
        };

        var macroCommand = new MacroCommand(document, commands);

        // Act & Assert
        // This will throw ArgumentOutOfRangeException because all commands
        // operate on the same empty document, not on the result of previous commands
        Assert.Throws<ArgumentOutOfRangeException>(() => macroCommand.Execute());
    }

    [Test]
    public void Execute_WithSameDocumentState_ThrowsOnInvalidPositions()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "Start");

        // Commands that would work if chained, but won't work because
        // they all reference the same "Start" document state
        var commands = new List<Func<Document, ICommand<Document>>>
        {
            doc => new InsertCommand(doc, 5, " A"), // OK: "Start A"
            doc => new InsertCommand(doc, 7, "B"), // FAIL: position 7 doesn't exist in "Start"
            doc => new InsertCommand(doc, 8, "C"), // FAIL: position 8 doesn't exist in "Start"
        };

        var macroCommand = new MacroCommand(document, commands);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => macroCommand.Execute());
    }

    [Test]
    public void Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "Test");

        var commands = new List<Func<Document, ICommand<Document>>>
        {
            doc => new DeleteCommand(doc, 0, 2),
        };

        var macroCommand = new MacroCommand(document, commands);
        macroCommand.Execute();

        // Act
        var result = macroCommand.Undo();

        // Assert
        Assert.That(result.Content, Is.EqualTo("Test"));
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
        var description = macroCommand.Description;

        // Assert
        Assert.That(description, Does.Contain("Macro"));
        Assert.That(description, Does.Contain("Insert"));
    }
}
