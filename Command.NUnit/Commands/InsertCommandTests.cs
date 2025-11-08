using Command.Commands;

namespace Command.NUnit.Commands;

[TestFixture]
public class InsertCommandTests
{
    [Test]
    public void Execute_ShouldInsertTextAtPosition()
    {
        // Arrange
        var document = new Document();
        var command = new InsertCommand(document, 0, "Hello");

        // Act
        var result = command.Execute();

        // Assert
        Assert.That(result.Content, Is.EqualTo("Hello"));
        Assert.That(result.Length, Is.EqualTo(5));
    }

    [Test]
    public void Execute_ShouldInsertTextInMiddle()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "HelloWorld");
        var command = new InsertCommand(document, 5, " - ");

        // Act
        var result = command.Execute();

        // Assert
        Assert.That(result.Content, Is.EqualTo("Hello - World"));
    }

    [Test]
    public void Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        var command = new InsertCommand(document, 0, "Hello");
        command.Execute();

        // Act
        var undoResult = command.Undo();

        // Assert
        Assert.That(undoResult.Content, Is.EqualTo(""));
        Assert.That(undoResult.Length, Is.EqualTo(0));
    }

    [Test]
    public void Undo_AfterMultipleEdits_ShouldReturnCapturedState()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "First");
        var command = new InsertCommand(document, 5, " Second");
        command.Execute();

        // Act
        var undoResult = command.Undo();

        // Assert
        Assert.That(undoResult.Content, Is.EqualTo("First"));
    }

    [Test]
    public void Description_ShouldBeReadable()
    {
        // Arrange
        var document = new Document();
        var command = new InsertCommand(document, 0, "Test");

        // Act
        var description = command.Description;

        // Assert
        Assert.That(description, Does.Contain("Insert"));
        Assert.That(description, Does.Contain("Test"));
        Assert.That(description, Does.Contain("0"));
    }
}
