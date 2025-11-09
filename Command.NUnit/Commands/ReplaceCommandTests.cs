using Command.Commands;

namespace Command.NUnit.Commands;

[TestFixture]
public class ReplaceCommandTests
{
    [Test]
    public void Execute_ShouldReplaceText()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Hello World");
        var command = new ReplaceCommand(document, 0, 5, "Hi");

        // Act
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Hi World"));
    }

    [Test]
    public void Execute_ShouldReplaceWithLongerText()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Hi");
        var command = new ReplaceCommand(document, 0, 2, "Hello");

        // Act
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Hello"));
    }

    [Test]
    public void Execute_ShouldReplaceInMiddle()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Hello Big World");
        var command = new ReplaceCommand(document, 6, 3, "Small");

        // Act
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Hello Small World"));
    }

    [Test]
    public void Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Hello World");
        var command = new ReplaceCommand(document, 0, 5, "Hi");
        command.Execute();

        // Act
        command.Undo();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Hello World"));
    }

    [Test]
    public void Description_ShouldIncludeReplacedText()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Hello World");
        var command = new ReplaceCommand(document, 0, 5, "Hi");

        // Act
        command.Execute();
        var description = command.Description;

        // Assert
        Assert.That(description, Does.Contain("Replace"));
    }
}
