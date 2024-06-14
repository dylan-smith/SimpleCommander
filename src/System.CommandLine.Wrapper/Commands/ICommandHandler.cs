using System.Threading.Tasks;

namespace System.CommandLine.Wrapper.Commands;

public interface ICommandHandler<in TArgs> where TArgs : CommandArgs
{
    Task Handle(TArgs args);
}
