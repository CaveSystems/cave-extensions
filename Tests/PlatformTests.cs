#if !(NETCOREAPP1_0_OR_GREATER && !NETCOREAPP2_0_OR_GREATER)

using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace Test;

[TestFixture]
class PlatformTests
{
    [Test]
    public void Platform()
    {
        Console.WriteLine($"Platform.IsAndroid: {Cave.Platform.IsAndroid}");
        Console.WriteLine($"Platform.IsMicrosoft: {Cave.Platform.IsMicrosoft}");
        Console.WriteLine($"Platform.IsMono: {Cave.Platform.IsMono}");
        Console.WriteLine($"Platform.Type: {Cave.Platform.Type}");
        Console.WriteLine($"Platform.SystemVersionString: {Cave.Platform.SystemVersionString}");
    }
}

#endif
