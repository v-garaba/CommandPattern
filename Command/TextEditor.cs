using System.Threading.Tasks;
using Command.Commands;
using Command.Common;
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
        Result result = await _commandOperator.ExecuteCommandAsync(insertCommand, cancellationToken);
        if (result.IsSuccess)
        {
            Console.WriteLine($"✓ Inserted '{text}' at position {position}");
        }
        else
        {
            Console.WriteLine($"✗ Failed to insert: {result.ErrorMessage}");
        }
    }

    public async Task DeleteTextAsync(int position, int length, CancellationToken cancellationToken)
    {
        var deleteCommand = new DeleteCommand(_document, position, length);
        Result result = await _commandOperator.ExecuteCommandAsync(deleteCommand, cancellationToken);
        if (result.IsSuccess)
        {
            Console.WriteLine($"✓ Deleted {length} characters at position {position}");
        }
        else
        {
            Console.WriteLine($"✗ Failed to delete: {result.ErrorMessage}");
        }
    }

    public async Task ReplaceTextAsync(int position, int length, string newText, CancellationToken cancellationToken)
    {
        var replaceCommand = new ReplaceCommand(_document, position, length, newText);
        Result result = await _commandOperator.ExecuteCommandAsync(replaceCommand, cancellationToken);
        if (result.IsSuccess)
        {
            Console.WriteLine($"✓ Replaced {length} characters at position {position} with '{newText}'");
        }
        else
        {
            Console.WriteLine($"✗ Failed to replace: {result.ErrorMessage}");
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
        Result<ICommandAsync?> reversedOperation = await _commandOperator.UndoLastCommandAsync(cancellationToken);

        if (!reversedOperation.IsSuccess)
        {
            Console.WriteLine("✗ Undo operation failed");
            return;
        }

        if (reversedOperation.Value == null)
        {
            Console.WriteLine("✗ No operation to undo");
            return;
        }

        Console.WriteLine($"✓ Undid operation: {reversedOperation}");
    }

    public async Task AttemptRedoAsync(CancellationToken cancellationToken)
    {
        Result<ICommandAsync?> redoneOperation = await _commandOperator.RedoLastCommandAsync(cancellationToken);

        if (!redoneOperation.IsSuccess)
        {
            Console.WriteLine("✗ Redo operation failed");
            return;
        }

        if (redoneOperation.Value == null)
        {
            Console.WriteLine("✗ No operation to redo");
            return;
        }

        Console.WriteLine($"✓ Redid operation: {redoneOperation}");
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
        Result success = await _commandOperator.ExecuteQueuedCommandsAsync(cancellationToken);

        if (success.IsSuccess)
        {
            Console.WriteLine("✓ Executed all queued operations");
        }
        else
        {
            Console.WriteLine($"✗ Failed to execute queued operations: {success.ErrorMessage}");
        }
    }

    public async Task<Result<int>> DocumentLengthAsync(CancellationToken cancellationToken = default)
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
        Result<string> textResult = await _document.GetTextAsync(cancellationToken);
        Result<int> lengthResult = await _document.GetLengthAsync(cancellationToken);
        Console.WriteLine($"Length: {lengthResult.Value} characters");
        if (string.IsNullOrEmpty(textResult.Value))
        {
            Console.WriteLine("Content: [Empty]");
        }
        else
        {
            Console.WriteLine($"Content: \"{textResult.Value}\"");
        }
        Console.WriteLine("-----------------------------\n");
    }
}
