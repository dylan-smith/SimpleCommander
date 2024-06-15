using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Reflection;

namespace SimpleCommander.Extensions;

/// <summary>
/// Adds some convenience extension methods when dealing with Assembly objects.
/// </summary>
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
