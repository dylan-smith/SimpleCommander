namespace SimpleCommander.Extensions;

/// <summary>
/// Adds some convenience extension methods when dealing with objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// A more readable way to check if an object is null.
    /// </summary>
    /// <param name="obj">The object instance we are checking</param>
    public static bool HasValue(this object obj) => obj is not null;
}
