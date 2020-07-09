using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Cave
{
    /// <summary>
    /// Retrieves the main assembly of the running program.
    /// </summary>
    public static class MainAssembly
    {
        static Assembly mainAssembly = null;

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
            if (Platform.Type == PlatformType.Android)
            {
                Debug.WriteLine("androidEntryPoint");
                MethodInfo bestOnCreate = null;
                MethodInfo first = null;
                foreach (StackFrame frame in new StackTrace().GetFrames())
                {
                    if (!(frame.GetMethod() is MethodInfo method))
                    {
                        continue;
                    }

                    string asmName = method.Module?.Assembly?.GetName().Name;
                    if (asmName != null && (asmName != "mscorlib") && (asmName != "Mono.Android"))
                    {
                        first = method;
                        if (method.Name == "OnCreate")
                        {
                            bestOnCreate = method;
                        }
                    }
                }
                if (bestOnCreate != null)
                {
                    return bestOnCreate.Module.Assembly;
                }

                return first.Module.Assembly;
            }

            Debug.WriteLine("GetEntryAssembly");
            Assembly result = Assembly.GetEntryAssembly();
            if (result != null)
            {
                return result;
            }

            Debug.WriteLine("bestStatic");
            MethodInfo bestStatic = null;
            foreach (StackFrame frame in new StackTrace().GetFrames())
            {
                if (!(frame.GetMethod() is MethodInfo method))
                {
                    continue;
                }

                if (method.IsStatic && Path.GetExtension(method.Module.Name) == ".exe")
                {
                    bestStatic = method;
                }
            }
            if (bestStatic != null)
            {
                return bestStatic.Module.Assembly;
            }

            Debug.WriteLine("CurrentDomain");
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.EntryPoint != null && !a.ReflectionOnly)
                {
                    return a;
                }
            }
            return null;
        }
    }
}
