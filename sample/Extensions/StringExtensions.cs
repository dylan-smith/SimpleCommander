using SimpleCommander.Extensions;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Sample.Extensions;

public static class StringExtensions
{
    public static string EscapeDataString(this string value) => Uri.EscapeDataString(value);

    public static ulong? ToULongOrNull(this string s) => ulong.TryParse(s, out var result) ? result : null;

    public static bool ToBool(this string s) => bool.TryParse(s, out var result) && result;

    public static StringContent ToStringContent(this string s) => new(s, Encoding.UTF8, "application/json");

    public static string ReplaceInvalidCharactersWithDash(this string s) => s.HasValue() ? Regex.Replace(s, @"[^\w.-]+", "-", RegexOptions.Compiled | RegexOptions.CultureInvariant) : string.Empty;
}
