#if NETCOREAPP1_0_OR_GREATER && !NETCOREAPP2_0_OR_GREATER
#define ALTERNATE_CODE
#endif

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Cave;
using Cave.Console;
using Cave.Logging;
using NUnit.Framework;
using Test.BaseX;

namespace Test;

[ExcludeFromCodeCoverage]
class Program
{
    public const bool Verbose = false;

    static bool exit = false;

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

        SystemConsole.WriteLine($"Framework: <yellow>{frameworkVersion}");

        SystemConsole.SetKeyPressedEvent(KeyPressed);
        foreach (var type in types)
        {
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

            SystemConsole.WriteLine($"Program.cs: info TI0000: <cyan>Create {type}<reset>: {frameworkVersion}");
            var instance = Activator.CreateInstance(type);
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                if (exit) return 1;

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
                SystemConsole.WriteLine($"{method.DeclaringType.Name}.cs: info TI0001: <cyan>Start {method.Name}<reset>: {frameworkVersion}:");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                var watch = StopWatch.StartNew();
                try
                {
                    var task = Task.Factory.StartNew(() => method.Invoke(instance, null));
                    while (!task.Wait(120 * 1000))
                    {
                        if (!Debugger.IsAttached) throw new TimeoutException($"Timeout of test after {watch.Elapsed.FormatTime()}");
                    }
                    SystemConsole.WriteLine($"{method.DeclaringType.Name}.cs: info TI0002: <green>Success {method.Name} <reset>{watch.Elapsed.FormatTime()}: {frameworkVersion}");
                }
                catch (Exception ex)
                {
                    SystemConsole.WriteLine($"{method.DeclaringType.Name}.cs: error TE0001: <red>Error {method.Name} <reset>{watch.Elapsed.FormatTime()}: {frameworkVersion}: {ex.Message}");
                    SystemConsole.WriteLine(ex.ToString());
                    SystemConsole.WriteLine(ex.StackTrace);
                    errors++;
                }
            }
        }
        if (errors == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            SystemConsole.WriteLine($"Program.cs: info TI9999: <green>All tests successfully completed<reset>: {frameworkVersion}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            SystemConsole.WriteLine($"Program.cs: error TE9999: {errors} tests failed: {frameworkVersion}");
        }
        SystemConsole.ResetColor();
        if (Debugger.IsAttached)
        {
            WaitExit();
        }

        return errors;
    }

    static void KeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.Escape || (keyInfo.Modifiers == ConsoleModifiers.Control && keyInfo.Key == ConsoleKey.C))
        {
            SystemConsole.WriteLine("<red>Break<reset> by user interaction!");
            exit = true;
        }
    }

#if ALTERNATE_CODE
    static void WaitExit() { }
#else

    static void WaitExit()
    {
        SystemConsole.Write("--- press enter to exit ---");
        while (SystemConsole.ReadKey().Key != ConsoleKey.Enter)
        {
            ;
        }
    }

#endif
}
