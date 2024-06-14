using System.CommandLine.Binding;
using System.Linq;

namespace System.CommandLine.Wrapper.Services;

/// <summary>
/// A custom Binder that uses reflection to map the arguments of a command into a CommandArgs instance. It does the matching based on the property names of the CommandArgs class and the Option names of the command.
/// </summary>
public class GenericArgsBinder<TCommand, TArgs> : BinderBase<TArgs>
    where TCommand : notnull
    where TArgs : class, new()
{
    private readonly TCommand _command;

    /// <summary>
    /// Creates a new instance of the GenericArgsBinder class.
    /// </summary>
    public GenericArgsBinder(TCommand command) => _command = command;

    /// <summary>
    /// Iterates over all Options in the command and sets the corresponding property in the CommandArgs instance.
    /// </summary>
    protected override TArgs GetBoundValue(BindingContext bindingContext)
    {
        var args = new TArgs();

        foreach (var prop in typeof(TCommand).GetProperties().Where(p => p.PropertyType.IsAssignableTo(typeof(Option))))
        {
            typeof(TArgs)
                .GetProperty(prop.Name)?
                .SetValue(args, bindingContext?.ParseResult.GetValueForOption((Option)prop.GetValue(_command)!));
        }

        return args;
    }
}
