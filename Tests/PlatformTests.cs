using System;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    class PlatformTests
    {
        #region Public Methods

        [Test]
        public void Platform()
        {
            Console.WriteLine($"Platform.IsAndroid: {Cave.Platform.IsAndroid}");
            Console.WriteLine($"Platform.IsMicrosoft: {Cave.Platform.IsMicrosoft}");
            Console.WriteLine($"Platform.IsMono: {Cave.Platform.IsMono}");
            Console.WriteLine($"Platform.Type: {Cave.Platform.Type}");
            Console.WriteLine($"Platform.SystemVersionString: {Cave.Platform.SystemVersionString}");
        }

        #endregion Public Methods
    }
}
