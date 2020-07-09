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
                Test((long)rnd.Next() * rnd.Next() + rnd.Next(), culture);
                Test((ulong)(rnd.Next() * rnd.Next() + rnd.Next()), culture);
                Test(TimeSpan.FromDays(rnd.NextDouble()), culture);
                Test(DateTime.Now + TimeSpan.FromDays(rnd.NextDouble()), culture);

                Test((float?)rnd.NextDouble(), culture);
                Test((double?)rnd.NextDouble(), culture);
                Test((decimal?)rnd.NextDouble(), culture);
                Test((sbyte?)rnd.Next(), culture);
                Test((byte?)rnd.Next(), culture);
                Test((short?)rnd.Next(), culture);
                Test((ushort?)rnd.Next(), culture);
                Test((int?)rnd.Next(), culture);
                Test((uint?)rnd.Next(), culture);
                Test((long?)((long)rnd.Next() * rnd.Next() + rnd.Next()), culture);
                Test((ulong?)(rnd.Next() * rnd.Next() + rnd.Next()), culture);
                Test((TimeSpan?)TimeSpan.FromDays(rnd.NextDouble()), culture);
                Test((DateTime?)(DateTime.Now + TimeSpan.FromDays(rnd.NextDouble())), culture);

                Test((float?)null, culture);
                Test((double?)null, culture);
                Test((decimal?)null, culture);
                Test((sbyte?)null, culture);
                Test((byte?)null, culture);
                Test((short?)null, culture);
                Test((ushort?)null, culture);
                Test((int?)null, culture);
                Test((uint?)null, culture);
                Test((long?)null, culture);
                Test((ulong?)null, culture);
                Test((TimeSpan?)null, culture);
                Test((DateTime?)null, culture);

                byte[] buf = new byte[50];
                rnd.NextBytes(buf);
                Test(buf, culture);
                var arrayI = new[] { rnd.Next(), rnd.Next() };
                Test(arrayI, culture);
            }
        }

        [Test]
        public void FormatTime()
        {
            void Test(TimeSpan timespan)
            {
                var str1 = StringExtensions.ToString(timespan);
                var check1 = str1.ParseValue<TimeSpan>();
                var str2 = timespan.FormatTime();
                var check2 = str2.ParseValue<TimeSpan>();
                Assert.AreEqual(timespan, check1);
                // 1% precision
                Assert.AreEqual(timespan.Ticks / 100 + 1, Math.Abs(timespan.Ticks - check2.Ticks));
            }

            Random rnd = new Random();
            for(int i = 0; i < 1000; i++)
            {
                var timespan = new TimeSpan(rnd.Next());
                Test(timespan);
            }

            Test(TimeSpan.FromMilliseconds(0.00123));
            Test(TimeSpan.FromMilliseconds(0.123));
            Test(TimeSpan.FromMilliseconds(1.234));
            Test(TimeSpan.FromSeconds(1.234));
            Test(TimeSpan.FromMinutes(1.234));
            Test(TimeSpan.FromHours(1.234));
            Test(TimeSpan.FromDays(1.234));
            Test(TimeSpan.FromDays(1000));
            Test(TimeSpan.FromDays(10000));
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
