using Command.Commands;

namespace Command.NUnit.Commands;

[TestFixture]
public class DeleteCommandTests
{
    [Test]
    public void Execute_ShouldDeleteText()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "Hello World");
        var command = new DeleteCommand(document, 5, 6);

        // Act
        var result = command.Execute();

        // Assert
        Assert.That(result.Content, Is.EqualTo("Hello"));
        Assert.That(result.Length, Is.EqualTo(5));
    }

    [Test]
    public void Execute_ShouldDeleteFromBeginning()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "Hello World");
        var command = new DeleteCommand(document, 0, 6);

        // Act
        var result = command.Execute();

        // Assert
        Assert.That(result.Content, Is.EqualTo("World"));
    }

    [Test]
    public void Execute_ShouldDeleteFromEnd()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "Hello World");
        var command = new DeleteCommand(document, 5, 6);

        // Act
        var result = command.Execute();

        // Assert
        Assert.That(result.Content, Is.EqualTo("Hello"));
    }

    [Test]
    public void Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "Hello World");
        var command = new DeleteCommand(document, 5, 6);
        command.Execute();

        // Act
        var undoResult = command.Undo();

        // Assert
        Assert.That(undoResult.Content, Is.EqualTo("Hello World"));
    }

    [Test]
    public void Description_ShouldIncludeDeletedText()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "Hello World");
        var command = new DeleteCommand(document, 0, 5);

        // Act
        command.Execute();
        var description = command.Description;

        // Assert
        Assert.That(description, Does.Contain("Delete"));
        Assert.That(description, Does.Contain("Hello"));
    }
}
