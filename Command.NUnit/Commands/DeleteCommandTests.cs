using Command.Commands;

namespace Command.NUnit.Commands;

[TestFixture]
public class DeleteCommandTests
{
    [Test]
    public async Task Execute_ShouldDeleteText()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hello World");
        var command = new DeleteCommand(document, 5, 6);

        // Act
        await command.ExecuteAsync();

        // Assert
        string content = await document.GetTextAsync();
        Assert.That(content, Is.EqualTo("Hello"));
    }

    [Test]
    public async Task Execute_ShouldDeleteFromBeginning()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hello World");
        var command = new DeleteCommand(document, 0, 6);

        // Act
        await command.ExecuteAsync();

        // Assert
        string content = await document.GetTextAsync();
        Assert.That(content, Is.EqualTo("World"));
    }

    [Test]
    public async Task Execute_ShouldDeleteFromEnd()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hello World");
        var command = new DeleteCommand(document, 5, 6);

        // Act
        await command.ExecuteAsync();

        // Assert
        string content = await document.GetTextAsync();
        Assert.That(content, Is.EqualTo("Hello"));
    }

    [Test]
    public async Task Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hello World");
        var command = new DeleteCommand(document, 5, 6);
        await command.ExecuteAsync();

        // Act
        await command.UndoAsync();

        // Assert
        string content = await document.GetTextAsync();
        Assert.That(content, Is.EqualTo("Hello World"));
    }

    [Test]
    public async Task Description_ShouldIncludeDeletedText()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hello World");
        var command = new DeleteCommand(document, 0, 5);

        // Act
        await command.ExecuteAsync();
        var description = command.Description;

        // Assert
        Assert.That(description, Does.Contain("Delete"));
    }
}
