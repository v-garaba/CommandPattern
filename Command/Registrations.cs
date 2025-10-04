using Command.History;
using Command.Operations;
using Command.Queues;
using Microsoft.Extensions.DependencyInjection;

namespace Command;

public static class Registrations
{
    /// <summary>
    /// Registers services for the Command pattern implementation.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    /// <returns>An IServiceProvider containing the registered services.</returns>
    public static IServiceCollection RegisterCommand(this IServiceCollection services)
    {
        services.AddScoped<ICommandOperator, CommandOperator>();
        services.AddScoped<ICommandExecuter>(s => s.GetRequiredService<ICommandOperator>());
        services.AddScoped<ICommandQueuer>(s => s.GetRequiredService<ICommandOperator>());
        services.AddTransient<IManagesHistory, HistoryManager>();
        services.AddTransient<ICommandQueue, QueueManager>();
        services.AddTransient<Document>();
        services.AddTransient<TextEditor>();
        return services;
    }

    internal static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        RegisterCommand(services);
        return services.BuildServiceProvider();
    }
}
