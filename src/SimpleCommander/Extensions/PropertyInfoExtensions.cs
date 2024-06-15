using System.Linq;
using System.Reflection;

namespace SimpleCommander.Extensions;

/// <summary>
/// Adds some convenience extension methods when dealing with PropertyInfo objects.
/// </summary>
public static class PropertyInfoExtensions
{
    internal static bool HasCustomAttribute<T>(this PropertyInfo propertyInfo) =>
        propertyInfo.GetCustomAttributes(typeof(T), true).Any();
}
