using Command.Commands;

namespace Command.Queues
{
    /// <summary>
    /// Defines a queue for storing commands to be executed later.
    /// </summary>
    public interface ICommandQueue<TTarget>
        where TTarget : notnull
    {
        /// <summary>
        /// Adds a command to the queue.
        /// </summary>
        /// <param name="command">The command to enqueue.</param>
        void Enqueue(ICommand<TTarget> command);

        /// <summary>
        /// Removes and returns the next command from the queue.
        /// </summary>
        /// <returns>The next command in the queue, or null if the queue is empty.</returns>
        ICommand<TTarget>? Dequeue();
    }
}
