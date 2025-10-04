using Microsoft.Extensions.DependencyInjection;

namespace Command;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("===============================================");
        Console.WriteLine("   COMMAND EDITOR");
        Console.WriteLine("===============================================");
        Console.WriteLine();

        var serviceProvider = Registrations.BuildServiceProvider();
        var editor = serviceProvider.GetRequiredService<TextEditor>();

        // Show current state
        editor.ShowCurrentState();

        Console.WriteLine("1. BASIC OPERATIONS");
        Console.WriteLine("====================================================");

        editor.InsertText(0, "Hello");
        editor.ShowCurrentState();

        editor.InsertText(5, " World");
        editor.ShowCurrentState();

        editor.ShowOperationHistory();

        Console.WriteLine("2. TRYING TO UNDO OPERATIONS");
        Console.WriteLine("=============================");
        editor.AttemptUndo();
        editor.ShowCurrentState();

        Console.WriteLine("2.1. TRYING TO UNDO THEN REDO OPERATIONS");
        Console.WriteLine("=============================");

        editor.AttemptUndo();
        editor.ShowCurrentState();

        editor.AttemptRedo();
        editor.ShowCurrentState();

        Console.WriteLine("3. MORE OPERATIONS");
        Console.WriteLine("==================");

        editor.InsertText(0, "Roaring");
        editor.InsertText(editor.DocumentLength, " tiger");
        editor.InsertText(editor.DocumentLength, ": ");
        editor.ShowCurrentState();

        editor.ReplaceText(0, 5, "Hi");
        editor.ShowCurrentState();

        editor.DeleteText(2, 3);
        editor.ShowCurrentState();

        editor.ShowOperationHistory();

        Console.WriteLine("4. TRYING TO CREATE MACROS");
        Console.WriteLine("===========================");
        editor.AttemptMacro();
        editor.ShowCurrentState();

        Console.WriteLine("5. TRYING TO QUEUE OPERATIONS");
        Console.WriteLine("==============================");
        editor.AttemptQueueOperations();
        editor.ShowCurrentState();

        Console.WriteLine("6. APPLYING QUEUED OPERATIONS");
        Console.WriteLine("==============================");
        editor.ApplyChanges();

        // Final state
        editor.ShowCurrentState();
        editor.ShowOperationHistory();
    }
}
