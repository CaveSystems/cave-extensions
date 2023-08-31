#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NETCOREAPP1_0 || NETCOREAPP1_1 || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)

namespace System.Diagnostics;

class Trace
{
    internal static void TraceInformation(string info) { }

    internal static void WriteLine(string message) { }

    internal static void WriteLineIf(bool condition, string message) { }
}

#endif
