using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using Cave;

namespace Tests
{
    [TestClass]
    public class StringExtensionTests
    {
        [TestMethod]
        public void UnEscape()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ushort.MaxValue; i++)
            {
                sb.Append(Encoding.Unicode.GetString(BitConverter.GetBytes((ushort)i)));
            }
            var text = sb.ToString();
            var escaped = StringExtensions.Escape(text);
            var unescaped = StringExtensions.Unescape(escaped);
            Assert.AreEqual(text, unescaped);
        }
    }
}
