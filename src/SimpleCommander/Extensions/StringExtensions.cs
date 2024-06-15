namespace SimpleCommander.Extensions;

/// <summary>
/// Adds some convenience extension methods when dealing with strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Provides a more readable way to check if a string is null or empty.
    /// </summary>
    /// <param name="s">The string instance we are checking</param>
    /// <returns>true is the the string is null or whitespace</returns>
    public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);

    /// <summary>
    /// Provides a more readable way to check if a string has a value. Under the covers does a IsNullOrWhiteSpace check.
    /// </summary>
    /// <param name="s">The string instance we are checking</param>
    /// <returns>true if the string has a value</returns>
    public static bool HasValue(this string s) => !s.IsNullOrWhiteSpace();
}
