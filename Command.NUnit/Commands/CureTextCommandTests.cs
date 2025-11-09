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
        document.InsertText(0, "Hello:World");
        var command = new CureTextCommand(document);

        // Act
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Hello@World"));
    }

    [Test]
    public void Execute_ShouldReplaceSpaces()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Hello World");
        var command = new CureTextCommand(document);

        // Act
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Hello_World"));
    }

    [Test]
    public void Execute_ShouldReplaceColonsAndSpaces()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Hello: World Test");
        var command = new CureTextCommand(document);

        // Act
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Hello@_World_Test"));
    }

    [Test]
    public void Execute_WithMultipleColons_MayHaveIndexShiftingIssue()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "a:b:c");
        var command = new CureTextCommand(document);

        // Act
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("a@b@c"));
    }

    [Test]
    public void Execute_WithNoSpecialCharacters_ShouldReturnUnchanged()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "HelloWorld");
        var command = new CureTextCommand(document);

        // Act
        command.Execute();

        // Assert
        Assert.That(document.Content, Is.EqualTo("HelloWorld"));
    }

    [Test]
    public void Undo_ShouldReturnOriginalState()
    {
        // Arrange
        var document = new Document();
        document.InsertText(0, "Hello: World");
        var command = new CureTextCommand(document);
        command.Execute();

        // Act
        command.Undo();

        // Assert
        Assert.That(document.Content, Is.EqualTo("Hello: World"));
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
