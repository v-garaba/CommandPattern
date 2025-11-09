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
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Hello"));
    }

    [Test]
    public void Execute_ShouldInsertTextInMiddle()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "HelloWorld");
        var command = new InsertCommand(document, 5, " - ");

        // Act
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Hello - World"));
    }

    [Test]
    public void Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        var command = new InsertCommand(document, 0, "Hello");
        command.Execute();

        // Act
        command.Undo();

        // Assert
        Assert.That(document.Content, Is.EqualTo(""));
    }

    [Test]
    public void Undo_AfterMultipleEdits_ShouldReturnCapturedState()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "First");
        var command = new InsertCommand(document, 5, " Second");
        command.Execute();

        // Act
        command.Undo();

        // Assert
        Assert.That(document.Content, Is.EqualTo("First"));
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
