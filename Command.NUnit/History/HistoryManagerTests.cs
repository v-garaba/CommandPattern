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
        cmd1.Execute();
        historyManager.AddCommand(cmd1);

        var cmd2 = new InsertCommand(doc1, 5, " World");
        cmd2.Execute();
        historyManager.AddCommand(cmd2);

        // Act - Undo last command
        historyManager.Undo();

        // Assert
        Assert.That(doc1.Content, Is.EqualTo("Hello"));

        // Act - Redo
        historyManager.Redo();

        // Assert
        Assert.That(doc1.Content, Is.EqualTo("Hello World"));
    }

    [Test]
    public void HistoryManager_MultipleUndos_RestoresCorrectStates()
    {
        // Arrange
        var historyManager = new HistoryManager<Document>();
        var doc1 = new Document();

        var cmd1 = new InsertCommand(doc1, 0, "A");
        cmd1.Execute();
        historyManager.AddCommand(cmd1);

        var cmd2 = new InsertCommand(doc1, 1, "B");
        cmd2.Execute();
        historyManager.AddCommand(cmd2);

        var cmd3 = new InsertCommand(doc1, 2, "C");
        cmd3.Execute();
        historyManager.AddCommand(cmd3);

        // Act & Assert
        Assert.That(doc1.Content, Is.EqualTo("ABC"));

        historyManager.Undo();
        Assert.That(doc1.Content, Is.EqualTo("AB"));

        historyManager.Undo();
        Assert.That(doc1.Content, Is.EqualTo("A"));

        historyManager.Undo();
        Assert.That(doc1.Content, Is.EqualTo(""));
    }

    [Test]
    public void HistoryManager_UndoThenNewCommand_ClearsRedoStack()
    {
        // Arrange
        var historyManager = new HistoryManager<Document>();
        var doc1 = new Document();

        var cmd1 = new InsertCommand(doc1, 0, "Hello");
        cmd1.Execute();
        historyManager.AddCommand(cmd1);

        var cmd2 = new InsertCommand(doc1, 5, " World");
        cmd2.Execute();
        historyManager.AddCommand(cmd2);

        // Undo
        historyManager.Undo();

        // Act - Add new command (should clear redo stack in proper implementation)
        var cmd3 = new InsertCommand(doc1, 5, " There");
        historyManager.AddCommand(cmd3);

        // Assert - Redo should not work
        historyManager.Redo();

        Assert.That(doc1.Content, Is.EqualTo("Hello"), "Redo should not change document after new command added post-undo.");
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
            cmd.Execute();
            historyManager.AddCommand(cmd);
        }

        var history = historyManager.ToString();
        var lines = history.Split(Environment.NewLine);

        // Assert - Should only have 20 entries
        Assert.That(lines, Has.Length.LessThanOrEqualTo(20));
    }
}
