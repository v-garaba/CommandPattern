using System.Collections.Immutable;
using System.Text;
using Command.Commands;
using Command.History;
using Command.Operations;
using Command.Validation;

namespace Command;

public class TextEditor(Document document, ICommandOperator commandOperator)
{
    private readonly Document _document = document.AssertNotNull();
    private readonly ICommandOperator _commandOperator = commandOperator.AssertNotNull();

    public string Content => _document.Content;
    public int DocumentLength => _document.Length;

    public void InsertText(int position, string text)
    {
        var insertCommand = new InsertCommand(_document, position, text);
        _commandOperator.ExecuteCommand(insertCommand);
        Console.WriteLine($"✓ Inserted '{text}' at position {position}");
    }

    public void DeleteText(int position, int length)
    {
        var deleteCommand = new DeleteCommand(_document, position, length);
        _commandOperator.ExecuteCommand(deleteCommand);
        Console.WriteLine($"✓ Deleted {length} characters at position {position}");
    }

    public void ReplaceText(int position, int length, string newText)
    {
        var replaceCommand = new ReplaceCommand(_document, position, length, newText);
        _commandOperator.ExecuteCommand(replaceCommand);
        Console.WriteLine(
            $"✓ Replaced {length} characters at position {position} with '{newText}'"
        );
    }

    public void ShowOperationHistory()
    {
        Console.WriteLine("\n=== OPERATION HISTORY ===");
        Console.WriteLine(_commandOperator.RevealHistory());
        Console.WriteLine("=========================\n");
    }

    public void AttemptUndo()
    {
        var reversedOperation = _commandOperator.UndoLastCommand();
        if (reversedOperation != null)
        {
            Console.WriteLine($"✓ Undid operation: {reversedOperation}");
        }
    }

    public void AttemptRedo()
    {
        var reversedOperation = _commandOperator.RedoLastCommand();
        if (reversedOperation != null)
        {
            Console.WriteLine($"✓ Redid operation: {reversedOperation}");
        }
    }

    public void AttemptMacro()
    {
        var macroCommand = new CureTextCommand(_document);
        _commandOperator.ExecuteCommand(macroCommand);
        Console.WriteLine(
            "✓ Macro executed: Cure text (replaced ':' with '@' and spaces with '_')"
        );
    }

    public void AttemptQueueOperations()
    {
        _commandOperator.QueueCommand(
            new MacroCommand(
                [
                    new InsertCommand(_document, 0, "Queued "),
                    new InsertCommand(_document, 7, "operations "),
                    new ReplaceCommand(_document, 7, 11, "commands "),
                    new InsertCommand(_document, 16, ": "),
                ]
            )
        );

        Console.WriteLine("✓ Operations queued for later execution");
    }

    public void ApplyChanges()
    {
        _commandOperator.ExecuteQueuedCommands();
    }

    public void Clear()
    {
        _document.Clear();
        _commandOperator.Clear();
        _commandOperator.ClearQueue();
        Console.WriteLine("✓ Document and history cleared");
    }

    public void ShowCurrentState()
    {
        Console.WriteLine($"\n--- CURRENT DOCUMENT STATE ---");
        Console.WriteLine($"Length: {_document.Length} characters");
        if (string.IsNullOrEmpty(_document.Content))
        {
            Console.WriteLine("Content: [Empty]");
        }
        else
        {
            Console.WriteLine($"Content: \"{_document.Content}\"");
        }
        Console.WriteLine("-----------------------------\n");
    }
}
