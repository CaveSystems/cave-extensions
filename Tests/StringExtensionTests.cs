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
            void Test<T>(T value, CultureInfo culture)
            {
                var s = StringExtensions.ToString(value, culture);
                var test = TypeExtension.ConvertValue(typeof(T), s, culture);
                Assert.AreEqual(value, test);
            }

            Random rnd = new Random();
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var culture in allCultures)
            {
                if (culture.IsNeutralCulture) continue;

                Test((float)rnd.NextDouble(), culture);
                Test(rnd.NextDouble(), culture);
                Test((decimal)rnd.NextDouble(), culture);
                Test((sbyte)rnd.Next(), culture);
                Test((byte)rnd.Next(), culture);
                Test((short)rnd.Next(), culture);
                Test((ushort)rnd.Next(), culture);
                Test(rnd.Next(), culture);
                Test((uint)rnd.Next(), culture);
                Test(rnd.Next() * rnd.Next() + rnd.Next(), culture);
                Test((ulong)(rnd.Next() * rnd.Next() + rnd.Next()), culture);
                Test(TimeSpan.FromHours(rnd.NextDouble()), culture);
                Test(DateTime.Now + TimeSpan.FromHours(rnd.NextDouble()), culture);
                byte[] buf = new byte[50];
                rnd.NextBytes(buf);
                Test(buf, culture);
                var arrayI = new[] { rnd.Next(), rnd.Next() };
                Test(arrayI, culture);
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
