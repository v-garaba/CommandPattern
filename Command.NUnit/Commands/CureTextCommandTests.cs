using Command.Commands;

namespace Command.NUnit.Commands;

[TestFixture]
public class CureTextCommandTests
{
    [Test]
    public async Task Execute_ShouldReplaceColons()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hello:World");
        var command = new CureTextCommand(document);

        // Act
        await command.ExecuteAsync();

        // Assert
        string content = await document.GetTextAsync();
        Assert.That(content, Is.EqualTo("Hello@World"));
    }

    [Test]
    public async Task Execute_ShouldReplaceSpaces()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hello World");
        var command = new CureTextCommand(document);

        // Act
        await command.ExecuteAsync();

        // Assert
        string content = await document.GetTextAsync();
        Assert.That(content, Is.EqualTo("Hello_World"));
    }

    [Test]
    public async Task Execute_ShouldReplaceColonsAndSpaces()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hello: World Test");
        var command = new CureTextCommand(document);

        // Act
        await command.ExecuteAsync();

        // Assert
        string content = await document.GetTextAsync();
        Assert.That(content, Is.EqualTo("Hello@_World_Test"));
    }

    [Test]
    public async Task Execute_WithMultipleColons_MayHaveIndexShiftingIssue()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "a:b:c");
        var command = new CureTextCommand(document);

        // Act
        await command.ExecuteAsync();

        // Assert
        string content = await document.GetTextAsync();
        Assert.That(content, Is.EqualTo("a@b@c"));
    }

    [Test]
    public async Task Execute_WithNoSpecialCharacters_ShouldReturnUnchanged()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "HelloWorld");
        var command = new CureTextCommand(document);

        // Act
        await command.ExecuteAsync();

        // Assert
        string content = await document.GetTextAsync();
        Assert.That(content, Is.EqualTo("HelloWorld"));
    }

    [Test]
    public async Task Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "Hello: World");
        var command = new CureTextCommand(document);
        await command.ExecuteAsync();

        // Act
        await command.UndoAsync();

        // Assert
        string content = await document.GetTextAsync();
        Assert.That(content, Is.EqualTo("Hello: World"));
    }

    [Test]
    public void Description_ShouldBeReadable()
    {
        // Arrange
        var document = new Document();
        var command = new CureTextCommand(document);

        // Act
        var description = command.Description;

        // Assert
        Assert.That(description, Does.Contain("Cure"));
        Assert.That(description, Does.Contain("@"));
        Assert.That(description, Does.Contain("_"));
    }
}
