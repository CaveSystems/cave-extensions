using System;
using NUnit.Framework;
using Cave;

namespace Tests
{
    [TestFixture]
    class PlatformTests
    {
        [Test]
        public void Test()
        {
            Console.WriteLine($"Platform.IsAndroid: {Platform.IsAndroid}");
            Console.WriteLine($"Platform.IsMicrosoft: {Platform.IsMicrosoft}");
            Console.WriteLine($"Platform.IsMono: {Platform.IsMono}");
            Console.WriteLine($"Platform.Type: {Platform.Type}");
            Console.WriteLine($"Platform.SystemVersionString: {Platform.SystemVersionString}");
        }
    }
}
