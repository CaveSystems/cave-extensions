using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Cave;

/// <summary>Retrieves the main assembly of the running program.</summary>
public static class MainAssembly
{
    #region Static

    static Assembly mainAssembly;

    /// <summary>Gets the MainAssembly.</summary>
    /// <returns>Returns the main assembly instance.</returns>
    public static Assembly Get()
    {
        if (mainAssembly == null)
        {
            mainAssembly = FindProgramAssembly();
        }

        return mainAssembly;
    }

    static Assembly FindProgramAssembly()
    {
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
        if (Platform.Type == PlatformType.Android)
        {
            Debug.WriteLine("androidEntryPoint");
            MethodInfo bestOnCreate = null;
            MethodInfo first = null;
            foreach (var frame in new StackTrace().GetFrames())
            {
                if (frame.GetMethod() is not MethodInfo method)
                {
                    continue;
                }

                var asmName = method.Module?.Assembly?.GetName().Name;
                if (asmName is not null and not "mscorlib" and not "Mono.Android")
                {
                    first = method;
                    if (method.Name == "OnCreate")
                    {
                        bestOnCreate = method;
                    }
                }
            }

            return bestOnCreate != null ? bestOnCreate.Module.Assembly : first.Module.Assembly;
        }
#endif

#if !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD1_6_OR_GREATER)
        Debug.WriteLine("GetEntryAssembly");
        var result = Assembly.GetEntryAssembly();
        if (result != null)
        {
            return result;
        }
#endif

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
        Debug.WriteLine("bestStatic");
        MethodInfo bestStatic = null;
        foreach (var frame in new StackTrace().GetFrames())
        {
            if (frame.GetMethod() is not MethodInfo method)
            {
                continue;
            }

            if (method.IsStatic && (Path.GetExtension(method.Module.Name) == ".exe"))
            {
                bestStatic = method;
            }
        }

        if (bestStatic != null)
        {
            return bestStatic.Module.Assembly;
        }

        Debug.WriteLine("CurrentDomain");
        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            if ((a.EntryPoint != null) && !a.ReflectionOnly)
            {
                return a;
            }
        }
#endif
        return null;
    }

#endregion
}
