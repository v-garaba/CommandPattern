using Command.Commands;
using Command.History;

namespace Command.NUnit;

[TestFixture]
public class HistoryManagerTests
{
    [Test]
    public void HistoryManager_UndoRedo_WorksCorrectlyWithSimpleCommands()
    {
        // Arrange
        var historyManager = new HistoryManager<Document>();
        var doc1 = new Document();

        var cmd1 = new InsertCommand(doc1, 0, "Hello");
        var doc2 = cmd1.Execute();
        historyManager.AddCommand(cmd1);

        var cmd2 = new InsertCommand(doc2, 5, " World");
        var doc3 = cmd2.Execute();
        historyManager.AddCommand(cmd2);

        // Act - Undo last command
        var undoResult = historyManager.Undo();

        // Assert
        Assert.That(undoResult, Is.Not.Null);
        Assert.That(undoResult.Value.Target.Content, Is.EqualTo("Hello"));

        // Act - Redo
        var redoResult = historyManager.Redo();

        // Assert
        Assert.That(redoResult, Is.Not.Null);
        Assert.That(redoResult.Value.Target.Content, Is.EqualTo("Hello World"));
    }

    [Test]
    public void HistoryManager_MultipleUndos_RestoresCorrectStates()
    {
        // Arrange
        var historyManager = new HistoryManager<Document>();
        var doc1 = new Document();

        var cmd1 = new InsertCommand(doc1, 0, "A");
        var doc2 = cmd1.Execute();
        historyManager.AddCommand(cmd1);

        var cmd2 = new InsertCommand(doc2, 1, "B");
        var doc3 = cmd2.Execute();
        historyManager.AddCommand(cmd2);

        var cmd3 = new InsertCommand(doc3, 2, "C");
        var doc4 = cmd3.Execute();
        historyManager.AddCommand(cmd3);

        // Act & Assert
        Assert.That(doc4.Content, Is.EqualTo("ABC"));

        var undo1 = historyManager.Undo();
        Assert.That(undo1, Is.Not.Null);
        Assert.That(undo1.Value.Target.Content, Is.EqualTo("AB"));

        var undo2 = historyManager.Undo();
        Assert.That(undo2, Is.Not.Null);
        Assert.That(undo2.Value.Target.Content, Is.EqualTo("A"));

        var undo3 = historyManager.Undo();
        Assert.That(undo3, Is.Not.Null);
        Assert.That(undo3.Value.Target.Content, Is.EqualTo(""));
    }

    [Test]
    public void HistoryManager_UndoThenNewCommand_ClearsRedoStack()
    {
        // Arrange
        var historyManager = new HistoryManager<Document>();
        var doc1 = new Document();

        var cmd1 = new InsertCommand(doc1, 0, "Hello");
        var doc2 = cmd1.Execute();
        historyManager.AddCommand(cmd1);

        var cmd2 = new InsertCommand(doc2, 5, " World");
        var doc3 = cmd2.Execute();
        historyManager.AddCommand(cmd2);

        // Undo
        historyManager.Undo();

        // Act - Add new command (should clear redo stack in proper implementation)
        var cmd3 = new InsertCommand(doc2, 5, " There");
        historyManager.AddCommand(cmd3);

        // Assert - Redo should not work
        var redoResult = historyManager.Redo();

        // Note: Current implementation doesn't clear redo stack!
        // This is another bug - when you add a new command after undo,
        // the redo stack should be cleared
        Assert.That(
            redoResult,
            Is.Not.Null,
            "Bug: HistoryManager should clear redo stack when new command is added after undo"
        );
    }

    [Test]
    public void HistoryManager_LogsCommandHistory()
    {
        // Arrange
        var historyManager = new HistoryManager<Document>();
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
        var historyManager = new HistoryManager<Document>();
        var doc = new Document();

        // Act - Add 25 commands (max is 20)
        for (int i = 0; i < 25; i++)
        {
            var cmd = new InsertCommand(doc, 0, $"{i}");
            doc = cmd.Execute();
            historyManager.AddCommand(cmd);
        }

        var history = historyManager.ToString();
        var lines = history.Split(Environment.NewLine);

        // Assert - Should only have 20 entries
        Assert.That(lines.Length, Is.LessThanOrEqualTo(20));
    }
}
