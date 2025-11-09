using System.Threading.Tasks;
using Command.Commands;

namespace Command.NUnit.Commands;

[TestFixture]
public class CommandSequenceTests
{
    [Test]
    public async Task MultipleCommands_WithUndo_ShouldRestoreCorrectStates()
    {
        // Arrange
        var doc1 = new Document();
        var cmd1 = new InsertCommand(doc1, 0, "Hello");

        await cmd1.ExecuteAsync(); // "Hello"
        var cmd2 = new InsertCommand(doc1, 5, " World");

        await cmd2.ExecuteAsync(); // "Hello World"
        var cmd3 = new InsertCommand(doc1, 11, "!");

        await cmd3.ExecuteAsync(); // "Hello World!"

        // Act & Assert
        var result = await doc1.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hello World!"));

        await cmd3.UndoAsync();
        result = await doc1.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hello World"));

        await cmd2.UndoAsync();
        result = await doc1.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hello"));

        await cmd1.UndoAsync();
        result = await doc1.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(""));
    }

    [Test]
    public async Task MixedCommands_ExecuteAndUndo_ShouldMaintainCorrectState()
    {
        // Arrange
        var doc = new Document();

        var insertCmd = new InsertCommand(doc, 0, "Hello World");
        await insertCmd.ExecuteAsync();

        var replaceCmd = new ReplaceCommand(doc, 0, 5, "Hi");
        await replaceCmd.ExecuteAsync();

        var deleteCmd = new DeleteCommand(doc, 2, 6);
        await deleteCmd.ExecuteAsync();

        // Act & Assert
        var result = await doc.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hi"));

        // Undo delete
        await deleteCmd.UndoAsync();
        result = await doc.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hi World"));

        // Undo replace
        await replaceCmd.UndoAsync();
        result = await doc.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hello World"));

        // Undo insert
        await insertCmd.UndoAsync();
        result = await doc.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(""));
    }
}
