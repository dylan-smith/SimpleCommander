namespace System.CommandLine.Wrapper.Commands;

/// <summary>
/// Use then attribute to mark an argument as a secret. The value of the argument will not be logged or output to the terminal.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SecretAttribute : Attribute
{
}
