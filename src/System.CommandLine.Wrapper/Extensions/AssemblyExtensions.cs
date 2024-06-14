using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.CommandLine.Wrapper.Extensions;

public static class AssemblyExtensions
{
    internal static IEnumerable<Type> GetAllDescendantsOfCommandBase(this Assembly assembly) =>
        assembly?
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                t.IsAssignableTo(typeof(Command)))
        ?? throw new ArgumentNullException(nameof(assembly));
}
