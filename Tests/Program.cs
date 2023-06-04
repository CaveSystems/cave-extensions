#if NETCOREAPP1_0_OR_GREATER && !NETCOREAPP2_0_OR_GREATER
#define ALTERNATE_CODE
#endif

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using NUnit.Framework;

namespace Test;

[ExcludeFromCodeCoverage]
class Program
{
    public const bool Verbose = false;

    static int Main(string[] args)
    {
        var errors = 0;

#if ALTERNATE_CODE
        var asm = typeof(Program).GetTypeInfo().Assembly;
        var targetFramework = asm.GetCustomAttributes<TargetFrameworkAttribute>().FirstOrDefault();
        var frameworkVersion = targetFramework.FrameworkDisplayName;
        var types = asm.DefinedTypes.Select(t => t.AsType());
#else
        var types = typeof(Program).Assembly.GetTypes().ToArray();
        var frameworkVersion = "net " + Environment.Version;
#endif
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(frameworkVersion);
        Console.WriteLine();
        foreach (var type in types)
        {
            Console.ResetColor();
#if ALTERNATE_CODE
            var attrib = type.GetTypeInfo().GetCustomAttribute<TestFixtureAttribute>();
            if (attrib is not TestFixtureAttribute) continue;
#else
            var typeAttributes = type.GetCustomAttributes(typeof(TestFixtureAttribute), false).ToArray();
            var typeAttributesCount = typeAttributes.Length;
            if (typeAttributesCount == 0)
            {
                continue;
            }
#endif

            Console.WriteLine("Create " + type);
            var instance = Activator.CreateInstance(type);
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                var methodAttributes = method.GetCustomAttributes(typeof(TestAttribute), false).ToArray();

#if ALTERNATE_CODE
                var methodAttributesCount = methodAttributes.Count();
#else
                var methodAttributesCount = methodAttributes.Length;
#endif
                if (methodAttributesCount == 0)
                {
                    continue;
                }

                GC.Collect(999, GCCollectionMode.Forced);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{method.DeclaringType.Name}.cs: info TI0001: Start {method.Name}: {frameworkVersion}");
                Console.ResetColor();
                try
                {
                    method.Invoke(instance, null);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{method.DeclaringType.Name}.cs: info TI0002: Success {method.Name}: {frameworkVersion}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{method.DeclaringType.Name}.cs: error TE0001: {ex.Message}: {frameworkVersion}");
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    Console.ResetColor();
                    errors++;
                }
                Console.WriteLine("---");
            }
        }
        if (errors == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"---: info TI9999: All tests successfully completed: {frameworkVersion}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"---: error TE9999: {errors} tests failed: {frameworkVersion}");
        }
        Console.ResetColor();
        if (Debugger.IsAttached)
        {
            WaitExit();
        }

        return errors;
    }

#if ALTERNATE_CODE
    static void WaitExit() { }
#else
    static void WaitExit()
    {
        Console.Write("--- press enter to exit ---");
        while (Console.ReadKey(true).Key != ConsoleKey.Enter)
        {
            ;
        }
    }
#endif
}
