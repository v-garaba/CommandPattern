using Command.Commands;

namespace Command.NUnit.Commands;

[TestFixture]
public class CommandSequenceTests
{
    [Test]
    public void MultipleCommands_WithUndo_ShouldRestoreCorrectStates()
    {
        // Arrange
        var doc1 = new Document();
        var cmd1 = new InsertCommand(doc1, 0, "Hello");

        cmd1.Execute(); // "Hello"
        var cmd2 = new InsertCommand(doc1, 5, " World");

        cmd2.Execute(); // "Hello World"
        var cmd3 = new InsertCommand(doc1, 11, "!");

        cmd3.Execute(); // "Hello World!"

        // Act & Assert
        Assert.That(doc1.Content, Is.EqualTo("Hello World!"));

        cmd3.Undo();
        Assert.That(doc1.Content, Is.EqualTo("Hello World"));

        cmd2.Undo();
        Assert.That(doc1.Content, Is.EqualTo("Hello"));

        cmd1.Undo();
        Assert.That(doc1.Content, Is.EqualTo(""));
    }

    [Test]
    public void MixedCommands_ExecuteAndUndo_ShouldMaintainCorrectState()
    {
        // Arrange
        var doc = new Document();

        var insertCmd = new InsertCommand(doc, 0, "Hello World");
        insertCmd.Execute();

        var replaceCmd = new ReplaceCommand(doc, 0, 5, "Hi");
        replaceCmd.Execute();

        var deleteCmd = new DeleteCommand(doc, 2, 6);
        deleteCmd.Execute();

        // Act & Assert
        Assert.That(doc.Content, Is.EqualTo("Hi"));

        // Undo delete
        deleteCmd.Undo();
        Assert.That(doc.Content, Is.EqualTo("Hi World"));

        // Undo replace
        replaceCmd.Undo();
        Assert.That(doc.Content, Is.EqualTo("Hello World"));

        // Undo insert
        insertCmd.Undo();
        Assert.That(doc.Content, Is.EqualTo(""));
    }
}
