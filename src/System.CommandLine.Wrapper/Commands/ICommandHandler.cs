using System.Threading.Tasks;

namespace System.CommandLine.Wrapper.Commands;

/// <summary>
/// All command handlers must implement this interface in order to be executed.
/// </summary>
public interface ICommandHandler<in TArgs> where TArgs : CommandArgs
{
    /// <summary>
    /// Execute the command handler.
    /// </summary>
    /// <param name="args">The CommandArgs instance populated with all the argument values</param>
    Task Handle(TArgs args);
}
