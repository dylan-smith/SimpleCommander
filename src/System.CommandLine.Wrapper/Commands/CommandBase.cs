namespace System.CommandLine.Wrapper.Commands;

/// <summary>
/// The base class for all commands. Inherit from this class to define a new command, and define all the command options as public properties of type Option&lt;T&gt;.
/// </summary>
/// <typeparam name="TArgs">The CommandArgs type</typeparam>
/// <typeparam name="THandler">The CommandHandler type</typeparam>
public abstract class CommandBase<TArgs, THandler> : Command where TArgs : CommandArgs where THandler : ICommandHandler<TArgs>
{
    /// <summary>
    /// Creates a new instance of the CommandBase class.
    /// </summary>
    /// <param name="name">The name of the command that will be used when passing it on the command-line</param>
    /// <param name="description">The description of the command that will be displayed in the built-in CLI help</param>
    protected CommandBase(string name, string description = null) : base(name, description) { }

    /// <summary>
    /// This needs to overridden in child classes and should use the provided ServiceProvider to get all the dependencies needed to initialize an instance of the Command Handler.
    /// </summary>
    /// <param name="args">The CommandArgs instance populated with all the argument values</param>
    /// <param name="sp">The ServiceProvider instance used to resolve any other dependencies required to construct the command handler (e.g. an instance of CLILogger)</param>
    public abstract THandler BuildHandler(TArgs args, IServiceProvider sp);
}
