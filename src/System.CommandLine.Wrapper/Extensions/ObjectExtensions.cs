namespace System.CommandLine.Wrapper.Extensions;

public static class ObjectExtensions
{
    public static bool HasValue(this object obj) => obj is not null;
}
