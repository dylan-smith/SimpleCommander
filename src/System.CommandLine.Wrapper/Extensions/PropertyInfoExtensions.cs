using System.Linq;
using System.Reflection;

namespace System.CommandLine.Wrapper.Extensions;

public static class PropertyInfoExtensions
{
    internal static bool HasCustomAttribute<T>(this PropertyInfo propertyInfo) =>
        propertyInfo.GetCustomAttributes(typeof(T), true).Any();
}
