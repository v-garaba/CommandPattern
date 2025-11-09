using Command.Commands;
using Command.Operations;
using Command.Validation;

namespace Command;

public class TextEditor(ICommandOperator<Document> commandOperator)
{
    private readonly ICommandOperator<Document> _commandOperator = commandOperator.AssertNotNull();
    private Document _document = new();

    public int DocumentLength => _document.Length;

    public void InsertText(int position, string text)
    {
        var insertCommand = new InsertCommand(_document, position, text);
        _document = _commandOperator.ExecuteCommand(insertCommand);
        Console.WriteLine($"✓ Inserted '{text}' at position {position}");
    }

    public void DeleteText(int position, int length)
    {
        var deleteCommand = new DeleteCommand(_document, position, length);
        _document = _commandOperator.ExecuteCommand(deleteCommand);
        Console.WriteLine($"✓ Deleted {length} characters at position {position}");
    }

    public void ReplaceText(int position, int length, string newText)
    {
        var replaceCommand = new ReplaceCommand(_document, position, length, newText);
        _document = _commandOperator.ExecuteCommand(replaceCommand);
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
            _document = reversedOperation.Value.Target;
            Console.WriteLine($"✓ Undid operation: {reversedOperation.Value.Command}");
        }
    }

    public void AttemptRedo()
    {
        var reversedOperation = _commandOperator.RedoLastCommand();
        if (reversedOperation != null)
        {
            _document = reversedOperation.Value.Target;
            Console.WriteLine($"✓ Redid operation: {reversedOperation.Value.Command}");
        }
    }

    public void AttemptMacro()
    {
        var macroCommand = new CureTextCommand(_document);
        _document = _commandOperator.ExecuteCommand(macroCommand);
        Console.WriteLine(
            "✓ Macro executed: Cure text (replaced ':' with '@' and spaces with '_')"
        );
    }

    public void AttemptQueueOperations()
    {
        _commandOperator.QueueCommand(
            new MacroCommand(
                _document,
                [
                    doc => new InsertCommand(doc, 0, "Queued "),
                    doc => new InsertCommand(doc, 7, "operations "),
                    doc => new ReplaceCommand(doc, 7, 11, "commands "),
                    doc => new InsertCommand(doc, 16, ": "),
                ]
            )
        );

        Console.WriteLine("✓ Operations queued for later execution");
    }

    public void ApplyChanges()
    {
        var lastResult = _commandOperator.ExecuteQueuedCommands();
        if (lastResult != null)
        {
            _document = lastResult;
            Console.WriteLine("✓ Applied queued changes");
        }
    }

    public void Clear()
    {
        _document = _document.Clear();
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
