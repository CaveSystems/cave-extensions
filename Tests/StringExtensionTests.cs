using System;
using System.Globalization;
using System.Text;
using Cave;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class StringExtensionTests
    {
        [Test]
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

        [Test]
        public void ToStringParse()
        {
            Random rnd = new Random();
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var culture in allCultures)
            {
                float f = (float)rnd.NextDouble();
                var s = StringExtensions.ToString(f, culture);
                var test = TypeExtension.ConvertValue(typeof(float), s, culture);
                Assert.AreEqual(f, test);
            }
            foreach (var culture in allCultures)
            {
                double d = rnd.NextDouble();
                var s = StringExtensions.ToString(d, culture);
                var test = TypeExtension.ConvertValue(typeof(double), s, culture);
                Assert.AreEqual(d, test);
            }
        }
    }
}
