using Command.Commands;
using Command.History;

namespace Command.NUnit;

[TestFixture]
public class HistoryManagerTests
{
    [Test]
    public async Task HistoryManager_UndoRedo_WorksCorrectlyWithSimpleCommands()
    {
        // Arrange
        var historyManager = new HistoryManager();
        var doc1 = new Document();

        var cmd1 = new InsertCommand(doc1, 0, "Hello");
        await cmd1.ExecuteAsync();
        historyManager.AddCommand(cmd1);

        var cmd2 = new InsertCommand(doc1, 5, " World");
        await cmd2.ExecuteAsync();
        historyManager.AddCommand(cmd2);

        // Act - Undo last command
        await historyManager.UndoAsync();

        // Assert
        var result = await doc1.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hello"));

        // Act - Redo
        await historyManager.RedoAsync();

        // Assert
        result = await doc1.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hello World"));
    }

    [Test]
    public async Task HistoryManager_MultipleUndos_RestoresCorrectStates()
    {
        // Arrange
        var historyManager = new HistoryManager();
        var doc1 = new Document();

        var cmd1 = new InsertCommand(doc1, 0, "A");
        await cmd1.ExecuteAsync();
        historyManager.AddCommand(cmd1);

        var cmd2 = new InsertCommand(doc1, 1, "B");
        await cmd2.ExecuteAsync();
        historyManager.AddCommand(cmd2);

        var cmd3 = new InsertCommand(doc1, 2, "C");
        await cmd3.ExecuteAsync();
        historyManager.AddCommand(cmd3);

        // Act & Assert
        var result = await doc1.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("ABC"));

        await historyManager.UndoAsync();
        result = await doc1.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("AB"));

        await historyManager.UndoAsync();
        result = await doc1.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("A"));

        await historyManager.UndoAsync();
        result = await doc1.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(""));
    }

    [Test]
    public async Task HistoryManager_UndoThenNewCommand_ClearsRedoStack()
    {
        // Arrange
        var historyManager = new HistoryManager();
        var doc1 = new Document();

        var cmd1 = new InsertCommand(doc1, 0, "Hello");
        await cmd1.ExecuteAsync();
        historyManager.AddCommand(cmd1);

        var cmd2 = new InsertCommand(doc1, 5, " World");
        await cmd2.ExecuteAsync();
        historyManager.AddCommand(cmd2);

        // Undo
        await historyManager.UndoAsync();

        // Act - Add new command (should clear redo stack in proper implementation)
        var cmd3 = new InsertCommand(doc1, 5, " There");
        historyManager.AddCommand(cmd3);

        // Assert - Redo should not work
        await historyManager.RedoAsync();

        var result = await doc1.GetTextAsync();
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("Hello"), "Redo should not change document after new command added post-undo.");
    }

    [Test]
    public void HistoryManager_LogsCommandHistory()
    {
        // Arrange
        var historyManager = new HistoryManager();
        var doc1 = new Document();

        var cmd1 = new InsertCommand(doc1, 0, "Test");
        historyManager.AddCommand(cmd1);

        // Act
        var history = historyManager.ToString();

        // Assert
        Assert.That(history, Does.Contain("Executed"));
        Assert.That(history, Does.Contain("Insert 'Test' at position 0"));
    }

    [Test]
    public void HistoryManager_MaintainsMaxHistorySize()
    {
        // Arrange
        var historyManager = new HistoryManager();
        var doc = new Document();

        // Act - Add 25 commands (max is 20)
        for (int i = 0; i < 25; i++)
        {
            var cmd = new InsertCommand(doc, 0, $"{i}");
            _ = cmd.ExecuteAsync(); // Fire and forget for this test
            historyManager.AddCommand(cmd);
        }

        var history = historyManager.ToString();
        var lines = history.Split(Environment.NewLine);

        // Assert - Should only have 20 entries
        Assert.That(lines, Has.Length.LessThanOrEqualTo(20));
    }
}
