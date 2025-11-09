using Command.Commands;

namespace Command.NUnit.Commands;

[TestFixture]
public class ReplaceCommandTests
{
    [Test]
    public async Task Execute_ShouldReplaceText()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hello World");
        var command = new ReplaceCommand(document, 0, 5, "Hi");

        // Act
        await command.ExecuteAsync();

        // Assert
        var result = await document.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hi World"));
    }

    [Test]
    public async Task Execute_ShouldReplaceWithLongerText()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hi");
        var command = new ReplaceCommand(document, 0, 2, "Hello");

        // Act
        await command.ExecuteAsync();

        // Assert
        var result = await document.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hello"));
    }

    [Test]
    public async Task Execute_ShouldReplaceInMiddle()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hello Big World");
        var command = new ReplaceCommand(document, 6, 3, "Small");

        // Act
        await command.ExecuteAsync();

        // Assert
        var result = await document.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hello Small World"));
    }

    [Test]
    public async Task Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hello World");
        var command = new ReplaceCommand(document, 0, 5, "Hi");
        await command.ExecuteAsync();

        // Act
        await command.UndoAsync();

        // Assert
        var result = await document.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hello World"));
    }

    [Test]
    public async Task Description_ShouldIncludeReplacedText()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hello World");
        var command = new ReplaceCommand(document, 0, 5, "Hi");

        // Act
        await command.ExecuteAsync();
        var description = command.Description;

        // Assert
        Assert.That(description, Does.Contain("Replace"));
    }
}
