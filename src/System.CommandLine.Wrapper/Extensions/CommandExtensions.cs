using System.CommandLine.Wrapper.Commands;
using System.CommandLine.Wrapper.Services;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace System.CommandLine.Wrapper.Extensions;

/// <summary>
/// Extension to the RootCommand class that registers and wires up all the commands in the calling assembly.
/// </summary>
public static class CommandExtensions
{
    /// <summary>
    /// Finds all the command classes in the calling assembly (inherits from CommandBase&lt;TArgs, THandler&gt;) and wires them up.
    /// </summary>
    /// <param name="rootCommand">The root command to add the commands to</param>
    /// <param name="serviceProvider">The service provider to use to resolve dependencies</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static RootCommand AddCommands(this RootCommand rootCommand, ServiceProvider serviceProvider)
    {
        if (rootCommand is null)
        {
            throw new ArgumentNullException(nameof(rootCommand));
        }

        foreach (var commandType in Assembly.GetCallingAssembly().GetAllDescendantsOfCommandBase())
        {
            var commandBaseType = commandType.BaseType.IsGenericType && commandType.BaseType.GetGenericTypeDefinition() == typeof(CommandBase<,>)
                ? commandType.BaseType
                : commandType.BaseType.BaseType;

            var argsType = commandBaseType.GetGenericArguments()[0];
            var handlerType = commandBaseType.GetGenericArguments()[1];

            var command = (Command)typeof(CommandExtensions)
                .GetMethod("ConfigureCommand", BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(commandType, argsType, handlerType)
                .Invoke(null, new object[] { serviceProvider });

            rootCommand.AddCommand(command);
        }

        return rootCommand;
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called via reflection")]
    private static TCommand ConfigureCommand<TCommand, TArgs, THandler>(ServiceProvider sp)
        where TArgs : CommandArgs, new()
        where TCommand : CommandBase<TArgs, THandler>, new()
        where THandler : ICommandHandler<TArgs>
    {
        var command = new TCommand();
        var argsBinder = new GenericArgsBinder<TCommand, TArgs>(command);
        command.SetHandler(async args => await RunHandler(args, sp, command), argsBinder);
        return command;
    }

    private static async Task RunHandler<TArgs, THandler>(TArgs args, ServiceProvider sp, CommandBase<TArgs, THandler> command) where TArgs : CommandArgs
                                                                                                                                where THandler : ICommandHandler<TArgs>
    {
        var log = sp.GetRequiredService<CLILogger>();

        args.RegisterSecrets(log);
        args.Log(log);
        args.Validate(log);

        var handler = command.BuildHandler(args, sp);
        await handler.Handle(args);
    }
}
