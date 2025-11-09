using System.Threading.Tasks;
using Command.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Command;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("===============================================");
        Console.WriteLine("   COMMAND EDITOR");
        Console.WriteLine("===============================================");
        Console.WriteLine();

        var serviceProvider = Registrations.BuildServiceProvider();
        var editor = serviceProvider.GetRequiredService<TextEditor>();
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        // Show current state
        await editor.ShowCurrentStateAsync(cancellationToken);

        Console.WriteLine("1. BASIC OPERATIONS");
        Console.WriteLine("====================================================");

        await editor.InsertTextAsync(0, "Hello", cancellationToken);
        await editor.ShowCurrentStateAsync(cancellationToken);

        await editor.InsertTextAsync(5, " World", cancellationToken);
        await editor.ShowCurrentStateAsync(cancellationToken);

        editor.ShowOperationHistory();

        Console.WriteLine("2. TRYING TO UNDO OPERATIONS");
        Console.WriteLine("=============================");
        await editor.AttemptUndoAsync(cancellationToken);
        await editor.ShowCurrentStateAsync(cancellationToken);

        Console.WriteLine("2.1. TRYING TO UNDO THEN REDO OPERATIONS");
        Console.WriteLine("=============================");

        await editor.AttemptUndoAsync(cancellationToken);
        await editor.ShowCurrentStateAsync(cancellationToken);

        await editor.AttemptRedoAsync(cancellationToken);
        await editor.ShowCurrentStateAsync(cancellationToken);

        Console.WriteLine("3. MORE OPERATIONS");
        Console.WriteLine("==================");

        await editor.InsertTextAsync(0, "Roaring", cancellationToken);

        Result<int> length = await editor.DocumentLengthAsync(cancellationToken);
        await editor.InsertTextAsync(length.Value, " tiger", cancellationToken);
        await editor.InsertTextAsync(length.Value, ": ", cancellationToken);
        await editor.ShowCurrentStateAsync(cancellationToken);

        await editor.ReplaceTextAsync(0, 5, "Hi", cancellationToken);
        await editor.ShowCurrentStateAsync(cancellationToken);

        await editor.DeleteTextAsync(2, 3, cancellationToken);
        await editor.ShowCurrentStateAsync(cancellationToken);

        editor.ShowOperationHistory();

        Console.WriteLine("4. TRYING TO CREATE MACROS");
        Console.WriteLine("===========================");
        await editor.AttemptMacroAsync(cancellationToken);
        await editor.ShowCurrentStateAsync(cancellationToken);

        Console.WriteLine("5. TRYING TO QUEUE OPERATIONS");
        Console.WriteLine("==============================");
        await editor.AttemptQueueOperationsAsync();
        await editor.ShowCurrentStateAsync(cancellationToken);

        Console.WriteLine("6. APPLYING QUEUED OPERATIONS");
        Console.WriteLine("==============================");
        await editor.ApplyChangesAsync(cancellationToken);

        // Final state
        await editor.ShowCurrentStateAsync(cancellationToken);
        editor.ShowOperationHistory();
    }
}
