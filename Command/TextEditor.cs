using System.Threading.Tasks;
using Command.Commands;
using Command.Operations;
using Command.Validation;

namespace Command;

public class TextEditor(ICommandOperator commandOperator)
{
    private readonly ICommandOperator _commandOperator = commandOperator.AssertNotNull();
    private readonly Document _document = new();

    public async Task InsertTextAsync(int position, string text, CancellationToken cancellationToken)
    {
        var insertCommand = new InsertCommand(_document, position, text);
        bool success = await _commandOperator.ExecuteCommandAsync(insertCommand, cancellationToken);
        if (success)
        {
            Console.WriteLine($"✓ Inserted '{text}' at position {position}");
        }
        else
        {
            Console.WriteLine($"✗ Failed to insert '{text}' at position {position}");
        }
    }

    public async Task DeleteTextAsync(int position, int length, CancellationToken cancellationToken)
    {
        var deleteCommand = new DeleteCommand(_document, position, length);
        bool success = await _commandOperator.ExecuteCommandAsync(deleteCommand, cancellationToken);
        if (success)
        {
            Console.WriteLine($"✓ Deleted {length} characters at position {position}");
        }
        else
        {
            Console.WriteLine($"✗ Failed to delete {length} characters at position {position}");
        }
    }

    public async Task ReplaceTextAsync(int position, int length, string newText, CancellationToken cancellationToken)
    {
        var replaceCommand = new ReplaceCommand(_document, position, length, newText);
        bool success = await _commandOperator.ExecuteCommandAsync(replaceCommand, cancellationToken);
        if (success)
        {
            Console.WriteLine($"✓ Replaced {length} characters at position {position} with '{newText}'");
        }
        else
        {
            Console.WriteLine($"✗ Failed to replace {length} characters at position {position} with '{newText}'");
        }
    }

    public void ShowOperationHistory()
    {
        Console.WriteLine("\n=== OPERATION HISTORY ===");
        Console.WriteLine(_commandOperator.RevealHistory());
        Console.WriteLine("=========================\n");
    }

    public async Task AttemptUndoAsync(CancellationToken cancellationToken)
    {
        var reversedOperation = await _commandOperator.UndoLastCommandAsync(cancellationToken);

        if (reversedOperation == null)
        {
            Console.WriteLine("✗ No operation to undo");
            return;
        }

        if (reversedOperation == true)
        {
            Console.WriteLine($"✓ Undid operation: {reversedOperation}");
        }
        else
        {
            Console.WriteLine("✗ Undo operation failed");
        }
    }

    public async Task AttemptRedoAsync(CancellationToken cancellationToken)
    {
        var redoneOperation = await _commandOperator.RedoLastCommandAsync(cancellationToken);

        if (redoneOperation == null)
        {
            Console.WriteLine("✗ No operation to redo");
            return;
        }

        if (redoneOperation == true)
        {
            Console.WriteLine($"✓ Redid operation: {redoneOperation}");
        }
        else
        {
            Console.WriteLine("✗ Redo operation failed");
        }
    }

    public async Task AttemptMacroAsync(CancellationToken cancellationToken)
    {
        var macroCommand = new CureTextCommand(_document);
        await _commandOperator.ExecuteCommandAsync(macroCommand, cancellationToken);
        Console.WriteLine(
            "✓ Macro executed: Cure text (replaced ':' with '@' and spaces with '_')"
        );
    }

    public async Task AttemptQueueOperationsAsync()
    {
        await Task.Yield();

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

        _commandOperator.QueueCommand(
            new InsertCommand(_document, 0, "LAST WORD.")
        );

        Console.WriteLine("✓ Operations queued for later execution");
    }

    public async Task ApplyChangesAsync(CancellationToken cancellationToken)
    {
        bool success = await _commandOperator.ExecuteQueuedCommandsAsync(cancellationToken);

        if (success)
        {
            Console.WriteLine("✓ Executed all queued operations");
        }
        else
        {
            Console.WriteLine("✗ Failed to execute some queued operations");
        }
    }

    public async Task<int> DocumentLengthAsync(CancellationToken cancellationToken = default)
    {
        return await _document.GetLengthAsync(cancellationToken);
    }

    public async Task ClearAsync(CancellationToken cancellationToken)
    {
        await _document.ClearAsync(cancellationToken);
        _commandOperator.Clear();
        _commandOperator.ClearQueue();
        Console.WriteLine("✓ Document and history cleared");
    }

    public async Task ShowCurrentStateAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"\n--- CURRENT DOCUMENT STATE ---");
        string text = await _document.GetTextAsync(cancellationToken);
        int length = await _document.GetLengthAsync(cancellationToken);
        Console.WriteLine($"Length: {length} characters");
        if (string.IsNullOrEmpty(text))
        {
            Console.WriteLine("Content: [Empty]");
        }
        else
        {
            Console.WriteLine($"Content: \"{text}\"");
        }
        Console.WriteLine("-----------------------------\n");
    }
}
