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
        document.InsertText(0, "Hello World");
        var command = new DeleteCommand(document, 5, 6);

        // Act
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Hello"));
    }

    [Test]
    public void Execute_ShouldDeleteFromBeginning()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Hello World");
        var command = new DeleteCommand(document, 0, 6);

        // Act
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("World"));
    }

    [Test]
    public void Execute_ShouldDeleteFromEnd()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Hello World");
        var command = new DeleteCommand(document, 5, 6);

        // Act
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Hello"));
    }

    [Test]
    public void Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Hello World");
        var command = new DeleteCommand(document, 5, 6);
        command.Execute();

        // Act
        command.Undo();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Hello World"));
    }

    [Test]
    public void Description_ShouldIncludeDeletedText()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Hello World");
        var command = new DeleteCommand(document, 0, 5);

        // Act
        command.Execute();
        var description = command.Description;

        // Assert
        Assert.That(description, Does.Contain("Delete"));
    }
}
