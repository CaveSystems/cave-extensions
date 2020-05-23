using System;
using System.Globalization;
using System.Linq;
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
                if (culture.IsNeutralCulture) continue;
                float f = (float)rnd.NextDouble();
                var s = StringExtensions.ToString(f, culture);
                var test = TypeExtension.ConvertValue(typeof(float), s, culture);
                Assert.AreEqual(f, test);
            }
            foreach (var culture in allCultures)
            {
                if (culture.IsNeutralCulture) continue;
                double d = rnd.NextDouble();
                var s = StringExtensions.ToString(d, culture);
                var test = TypeExtension.ConvertValue(typeof(double), s, culture);
                Assert.AreEqual(d, test);
            }
        }

        [Test]
        public void CamelCaseTest()
        {
            Assert.AreEqual("TestID".SplitCamelCase().Join('|'), new[] { "Test", "ID" }.Join('|'));
            Assert.AreEqual("TestId".SplitCamelCase().Join('|'), new[] { "Test", "Id" }.Join('|'));
            Assert.AreEqual("testID".SplitCamelCase().Join('|'), new[] { "test", "ID" }.Join('|'));
            Assert.AreEqual("testId".SplitCamelCase().Join('|'), new[] { "test", "Id" }.Join('|'));
            Assert.AreEqual("TestId", new[] { "Test", "ID" }.JoinCamelCase());
            Assert.AreEqual("TestId", new[] { "Test", "Id" }.JoinCamelCase());
            Assert.AreEqual("TestId", new[] { "test", "ID" }.JoinCamelCase());
            Assert.AreEqual("TestId", new[] { "test", "Id" }.JoinCamelCase());
            Assert.AreEqual("TestId", new[] { "test", "id" }.JoinCamelCase());
            Assert.AreEqual("TestId", new[] { "teSt", "iD" }.JoinCamelCase());
        }
    }
}
