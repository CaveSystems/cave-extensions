using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cave;

/// <summary>Retrieves the main assembly of the running program.</summary>
public static class MainAssembly
{
    #region Private Fields

    static Assembly? mainAssembly;

    #endregion Private Fields

    #region Private Methods

    static Assembly FindProgramAssembly()
    {
        {
            Debug.WriteLine("FindProgramAssembly via GetEntryAssembly()");
            var result = Assembly.GetEntryAssembly();
            if (result != null)
            {
                return result;
            }
        }

        var stackMethods = new StackTrace().GetFrames().Select(s => s.GetMethod()).OfType<MethodInfo>().Where(m => !IsFilteredAssembly(m.Module?.Assembly)).ToArray();

        if (Platform.Type == PlatformType.Android)
        {
            Debug.WriteLine("FindProgramAssembly via OnCreate");
            MethodInfo? bestOnCreate = null;
            foreach (var method in stackMethods)
            {
                if (method.Name == "OnCreate")
                {
                    bestOnCreate = method;
                }
            }

            var result = bestOnCreate?.Module?.Assembly;
            if (result is not null) return result;
        }

        {
            Debug.WriteLine("FindProgramAssembly via static main");
            MethodInfo? bestStatic = null;
            MethodInfo? firstStatic = null;
            foreach (var method in stackMethods)
            {
                if (method.IsStatic)
                {
                    if (Path.GetExtension(method.Module.Name) == ".exe")
                    {
                        bestStatic = method;
                    }
                    else
                    {
                        firstStatic = method;
                    }
                }
            }

            var result = bestStatic?.Module?.Assembly ?? firstStatic?.Module?.Assembly;
            if (result is not null) return result;
        }

        {
            Debug.WriteLine("CurrentDomain");
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!IsFilteredAssembly(assembly) && (assembly.EntryPoint != null) && !assembly.ReflectionOnly)
                {
                    return assembly;
                }
            }
        }

        throw new InvalidOperationException("Could not find program assembly!");
    }

    static bool IsFilteredAssembly(Assembly? assembly)
    {
        var asmName = assembly?.GetName().Name;
        return (asmName is null or "mscorlib" or "Mono.Android");
    }

    #endregion Private Methods

    #region Public Methods

    /// <summary>Gets the MainAssembly.</summary>
    /// <returns>Returns the main assembly instance.</returns>
    public static Assembly Get() => mainAssembly ??= FindProgramAssembly();

    #endregion Public Methods
}
