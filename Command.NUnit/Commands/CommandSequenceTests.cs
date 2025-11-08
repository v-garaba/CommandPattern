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

        var doc2 = cmd1.Execute(); // "Hello"
        var cmd2 = new InsertCommand(doc2, 5, " World");

        var doc3 = cmd2.Execute(); // "Hello World"
        var cmd3 = new InsertCommand(doc3, 11, "!");

        var doc4 = cmd3.Execute(); // "Hello World!"

        // Act & Assert
        Assert.That(doc4.Content, Is.EqualTo("Hello World!"));

        var undo1 = cmd3.Undo();
        Assert.That(undo1.Content, Is.EqualTo("Hello World"));

        var undo2 = cmd2.Undo();
        Assert.That(undo2.Content, Is.EqualTo("Hello"));

        var undo3 = cmd1.Undo();
        Assert.That(undo3.Content, Is.EqualTo(""));
    }

    [Test]
    public void MixedCommands_ExecuteAndUndo_ShouldMaintainCorrectState()
    {
        // Arrange
        var doc = new Document();

        var insertCmd = new InsertCommand(doc, 0, "Hello World");
        doc = insertCmd.Execute();

        var replaceCmd = new ReplaceCommand(doc, 0, 5, "Hi");
        doc = replaceCmd.Execute();

        var deleteCmd = new DeleteCommand(doc, 2, 6);
        doc = deleteCmd.Execute();

        // Act & Assert
        Assert.That(doc.Content, Is.EqualTo("Hi"));

        // Undo delete
        doc = deleteCmd.Undo();
        Assert.That(doc.Content, Is.EqualTo("Hi World"));

        // Undo replace
        doc = replaceCmd.Undo();
        Assert.That(doc.Content, Is.EqualTo("Hello World"));

        // Undo insert
        doc = insertCmd.Undo();
        Assert.That(doc.Content, Is.EqualTo(""));
    }

    [Test]
    public void CommandChain_EachCommandCapturesItsPreExecutionState()
    {
        // Arrange & Act
        var doc1 = new Document();
        var cmd1 = new InsertCommand(doc1, 0, "A");
        var doc2 = cmd1.Execute(); // doc2 = "A"

        var cmd2 = new InsertCommand(doc2, 1, "B");
        var doc3 = cmd2.Execute(); // doc3 = "AB"

        var cmd3 = new InsertCommand(doc3, 2, "C");
        var doc4 = cmd3.Execute(); // doc4 = "ABC"

        // Assert - Each undo should restore to its captured state
        Assert.That(
            cmd3.Undo().Content,
            Is.EqualTo("AB"),
            "cmd3 should restore to 'AB' (state before cmd3 executed)"
        );

        Assert.That(
            cmd2.Undo().Content,
            Is.EqualTo("A"),
            "cmd2 should restore to 'A' (state before cmd2 executed)"
        );

        Assert.That(
            cmd1.Undo().Content,
            Is.EqualTo(""),
            "cmd1 should restore to '' (state before cmd1 executed)"
        );
    }
}
