using Command.Commands;

namespace Command.NUnit.Commands;

[TestFixture]
public class CureTextCommandTests
{
    [Test]
    public void Execute_ShouldReplaceColons()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "Hello:World");
        var command = new CureTextCommand(document);

        // Act
        var result = command.Execute();

        // Assert
        Assert.That(result.Content, Is.EqualTo("Hello@World"));
    }

    [Test]
    public void Execute_ShouldReplaceSpaces()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "Hello World");
        var command = new CureTextCommand(document);

        // Act
        var result = command.Execute();

        // Assert
        Assert.That(result.Content, Is.EqualTo("Hello_World"));
    }

    [Test]
    public void Execute_ShouldReplaceColonsAndSpaces()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "Hello: World Test");
        var command = new CureTextCommand(document);

        // Act
        var result = command.Execute();

        // Assert
        Assert.That(result.Content, Is.EqualTo("Hello@_World_Test"));
    }

    [Test]
    public void Execute_WithMultipleColons_MayHaveIndexShiftingIssue()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "a:b:c");
        var command = new CureTextCommand(document);

        // Act
        var result = command.Execute();

        // Assert
        // This test documents potential index shifting behavior
        // Expected: "a@b@c"
        // Note: Current implementation works for this case but algorithm
        // could fail if replacement text length differs from original
        Assert.That(result.Content, Is.EqualTo("a@b@c"));
    }

    [Test]
    public void Execute_WithNoSpecialCharacters_ShouldReturnUnchanged()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "HelloWorld");
        var command = new CureTextCommand(document);

        // Act
        var result = command.Execute();

        // Assert
        Assert.That(result.Content, Is.EqualTo("HelloWorld"));
    }

    [Test]
    public void Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        document = document.InsertText(0, "Hello: World");
        var command = new CureTextCommand(document);
        command.Execute();

        // Act
        var undoResult = command.Undo();

        // Assert
        Assert.That(undoResult.Content, Is.EqualTo("Hello: World"));
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
