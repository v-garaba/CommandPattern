using Command.Commands;

namespace Command.NUnit.Commands;

[TestFixture]
public class InsertCommandTests
{
    [Test]
    public async Task Execute_ShouldInsertTextAtPosition()
    {
        // Arrange
        var document = new Document();
        var command = new InsertCommand(document, 0, "Hello");

        // Act
        await command.ExecuteAsync();

        // Assert
        var result = await document.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hello"));
    }

    [Test]
    public async Task Execute_ShouldInsertTextInMiddle()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "HelloWorld");
        var command = new InsertCommand(document, 5, " - ");

        // Act
        await command.ExecuteAsync();

        // Assert
        var result = await document.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hello - World"));
    }

    [Test]
    public async Task Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        var command = new InsertCommand(document, 0, "Hello");
        await command.ExecuteAsync();

        // Act
        await command.UndoAsync();

        // Assert
        var result = await document.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(""));
    }

    [Test]
    public async Task Undo_AfterMultipleEdits_ShouldReturnCapturedState()
    {
        // Arrange
        var document = new Document();
        await document.InsertTextAsync(0, "First");
        var command = new InsertCommand(document, 5, " Second");
        await command.ExecuteAsync();

        // Act
        await command.UndoAsync();

        // Assert
        var result = await document.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("First"));
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
