using System.Text.RegularExpressions;

namespace System.CommandLine.Wrapper.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);

    public static bool HasValue(this string s) => !s.IsNullOrWhiteSpace();

    public static string ReplaceInvalidCharactersWithDash(this string s) => s.HasValue() ? Regex.Replace(s, @"[^\w.-]+", "-", RegexOptions.Compiled | RegexOptions.CultureInvariant) : string.Empty;
}
