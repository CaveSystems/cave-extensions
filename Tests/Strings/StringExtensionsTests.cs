using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using Cave;
using NUnit.Framework;

namespace Test.Strings
{
    [TestFixture]
    public class StringExtensionsTests
    {
        readonly IEnumerable<CultureInfo> allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(c => !c.IsNeutralCulture);

        [Test]
        public void CamelCaseTest()
        {
            Assert.AreEqual("TestID".SplitCamelCase().Join('|'), new[]
            {
                "Test",
                "ID"
            }.Join('|'));
            Assert.AreEqual("TestId".SplitCamelCase().Join('|'), new[]
            {
                "Test",
                "Id"
            }.Join('|'));
            Assert.AreEqual("testID".SplitCamelCase().Join('|'), new[]
            {
                "test",
                "ID"
            }.Join('|'));
            Assert.AreEqual("testId".SplitCamelCase().Join('|'), new[]
            {
                "test",
                "Id"
            }.Join('|'));
            Assert.AreEqual("TestId", new[]
            {
                "Test",
                "ID"
            }.JoinCamelCase());
            Assert.AreEqual("TestId", new[]
            {
                "Test",
                "Id"
            }.JoinCamelCase());
            Assert.AreEqual("TestId", new[]
            {
                "test",
                "ID"
            }.JoinCamelCase());
            Assert.AreEqual("TestId", new[]
            {
                "test",
                "Id"
            }.JoinCamelCase());
            Assert.AreEqual("TestId", new[]
            {
                "test",
                "id"
            }.JoinCamelCase());
            Assert.AreEqual("TestId", new[]
            {
                "teSt",
                "iD"
            }.JoinCamelCase());
        }

        [Test]
        public void DetectNewLine()
        {
            Assert.AreEqual("\r\n", "Line1\rLine2\nLine3\r\nLine4".DetectNewLine());
            Assert.AreEqual("\n", "Line1\rLine2\nLine3\nLine4".DetectNewLine());
            Assert.AreEqual("\r", "Line1\0Line2\0Line3\rLine4".DetectNewLine());
            Assert.AreEqual(null, "Line1\0Line2\0Line3\0Line4".DetectNewLine());
        }

        [Test]
        public void ExceptionToMessage()
        {
            try
            {
                var inner = new Exception("Inner");
                var ex = new Exception("Line1\nLine2\n\nEnd.", inner);
                ex.Data.Add("TestData", "Some test data...");
                throw ex;
            }
            catch (Exception ex)
            {
                ex.ToStrings();
                ex.ToStrings(true);
            }
        }

        [Test]
        public void ExceptionToText()
        {
            try
            {
                var inner = new Exception("Inner");
                var ex = new Exception("Line1\nLine2\n\nEnd.", inner);
                ex.Data.Add("TestData", "Some test data...");
                throw ex;
            }
            catch (Exception ex)
            {
                ex.ToText();
                ex.ToText(true);
            }
        }

        [Test]
        public void ForceLength()
        {
            Assert.AreEqual("0123 ", "0123".ForceLength(5));
            Assert.AreEqual("01234", "0123456789".ForceLength(5));
            Assert.AreEqual("...123", "123".ForceLength(6, ".", ""));
            Assert.AreEqual("123...", "123".ForceLength(6, "", "..."));
            Assert.AreEqual(".123..", "123".ForceLength(6, ".", ".."));
            Assert.AreEqual("..123..", "123".ForceLength(7, ".", ".."));
            Assert.AreEqual("..123...", "123".ForceLength(8, ".", ".."));
            Assert.AreEqual("..123....", "123".ForceLength(9, ".", ".."));
        }

        [Test]
        public void Format()
        {
            var a = "{0} {1} {3}".Format(1, 2, 3, 4);
            Assert.AreEqual("1 2 4", a);
            var b = "{0} {1} {3}".Format(1, 2, 3);
            Assert.AreEqual("1 2 {3}", b);
            var c = "{0} {1}".Format(1, 2, 3);
            Assert.AreEqual("1 2", c);
            var d = "{0} {1}".Format(1);
            Assert.AreEqual("1 {1}", d);
            var f = "Test".Format(null);
            Assert.AreEqual("Test", f);
        }

        [Test]
        public void FormatBinarySize()
        {
            foreach (var culture in allCultures)
            {
                var size = 1.432;
                for (var i = 0; i <= (int)IecUnit.YiB; i++)
                {
                    var expected = 1.432d.ToString("0.000", culture) + " " + (IecUnit)i;
                    var string1 = size.FormatBinarySize(culture);
                    Assert.AreEqual(expected, string1);
                    if ((size > 10) && (size < long.MaxValue))
                    {
                        var string2 = ((ulong)size).FormatBinarySize(culture);
                        var string3 = ((long)size).FormatBinarySize(culture);
                        Assert.AreEqual(expected, string2);
                        Assert.AreEqual(expected, string3);
                    }

                    size *= 1024;
                }
            }
        }

        [Test]
        public void FormatSize()
        {
            foreach (var culture in allCultures)
            {
                var size = 1.234;
                var i = 0;
                while (size < (long.MaxValue / 1000))
                {
                    var string1 = size.FormatSize(culture);
                    var string2 = ((ulong)size).FormatSize(culture);
                    var expected = 1.234d.ToString("0.000", culture);
                    if (i > 0)
                    {
                        expected += " " + (SiUnit)i;
                        Assert.AreEqual(expected, string2);
                    }

                    Assert.AreEqual(expected, string1);
                    size *= 1000;
                    i++;
                }
            }
        }

        [Test]
        public void FormatSize1()
        {
            foreach (var culture in allCultures)
            {
                var size = 1.432;
                for (var i = 0; i <= (int)SiUnit.Y; i++)
                {
                    var expected = 1.432d.ToString("0.000", culture);
                    if (i > 0)
                    {
                        expected += " " + (SiUnit)i;
                    }

                    var string1 = size.FormatSize(culture);
                    Assert.AreEqual(expected, string1);
                    size *= 1000;
                }
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
                Assert.AreEqual(true, ((timespan.Ticks / 100) + 1) > Math.Abs(timespan.Ticks - check2.Ticks));
            }

            var rnd = new Random();
            for (var i = 0; i < 1000; i++)
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
        public void FormatTimeSpan()
        {
            var ticks = DateTime.Now.Ticks;
            while (ticks > TimeSpan.TicksPerMillisecond)
            {
                TimeSpan.FromTicks(ticks).FormatTime();
                ticks /= 10;
            }
        }

        [Test]
        public void GetString()
        {
            var data = "iupz<IUFGiudgsfpUIGFEIPUEGT/fiouazgsdiupzfi_ugsfFUZTGI%dUPfGisd";
            {
                var start = "<IUF";
                var l_End = "/fio";
                var l_Expected = "GiudgsfpUIGFEIPUEGT";
                var l_String = data.GetString(-1, start, l_End);
                Assert.AreEqual(l_Expected, l_String);
            }
            {
                var start = '<';
                var l_End = '/';
                var l_Expected = "IUFGiudgsfpUIGFEIPUEGT";
                var l_String = data.GetString(-1, start, l_End);
                Assert.AreEqual(l_Expected, l_String);
            }
        }

        [Test]
        public void GetValidChars() => Assert.AreEqual("5712", "an5712jagalw".GetValidChars("0123456789"));

        [Test]
        public void HasInvalidChars()
        {
            Assert.IsTrue("01234176205192a8685156789".HasInvalidChars("0123456789"));
            Assert.IsFalse("0650213177123123456789".HasInvalidChars("0123456789"));
        }

        [Test]
        public void IndexOfInvalidChar()
        {
            Assert.AreEqual(-1, "12345".IndexOfInvalidChar("0123456789"));
            Assert.AreEqual(0, "an5712jagalw".IndexOfInvalidChar("0123456789"));
            Assert.AreEqual(4, "5712jagalw".IndexOfInvalidChar("0123456789"));
            Assert.AreEqual(0, "ajkdfgh".IndexOfInvalidChar(null));
            Assert.AreEqual(0, "ajkdfgh".IndexOfInvalidChar(""));
        }

        [Test]
        public void IndexOfInvalidChar1()
        {
            Assert.AreEqual(-1, "jagalw5712".IndexOfInvalidChar("0123456789", 6));
            Assert.AreEqual(4, "jagalw5712".IndexOfInvalidChar("0123456789", 4));
            Assert.AreEqual(6, "an5712jagalw".IndexOfInvalidChar("0123456789", 2));
            Assert.AreEqual(0, "ajkdfgh".IndexOfInvalidChar(null, 5));
            Assert.AreEqual(0, "ajkdfgh".IndexOfInvalidChar("", 5));
        }

        [Test]
        public void Join()
        {
            try { StringExtensions.Join(null, '-'); }
            catch (Exception ex) { Assert.IsInstanceOf<ArgumentNullException>(ex); }

            {
                var result = new object[]
                {
                    "1",
                    2,
                    null
                }.Join(',');
                var expected = "1,2,<null>";
                Assert.AreEqual(expected, result);
            }
            {
                var result = new object[]
                {
                    "1",
                    2,
                    null
                }.Join("");
                var expected = "12<null>";
                Assert.AreEqual(expected, result);
            }
        }

        [Test]
        public void Join1()
        {
            var result = new object[]
            {
                "1",
                2,
                null
            }.Join("|");
            var expected = "1|2|<null>";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void JoinNewLine()
        {
            var result = new[]
            {
                1,
                2,
                3
            }.JoinNewLine();
            var expected = "1\r\n2\r\n3";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void JoinNewLine1()
        {
            var result = new[]
            {
                "1",
                "2",
                null
            }.JoinNewLine();
            var expected = "1\r\n2\r\n<null>";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ParseDateTime()
        {
            var dateTimeLowPrec = new DateTime(2004, 12, 31, 12, 34, 56, DateTimeKind.Local);
            var dateTimeHighPrec = dateTimeLowPrec + TimeSpan.FromSeconds(0.1234567);
            var s1 = dateTimeHighPrec.ToString(StringExtensions.InterOpDateTimeFormat);
            Assert.AreEqual(dateTimeHighPrec, StringExtensions.ParseDateTime(s1));
            var s2 = dateTimeLowPrec.ToString(StringExtensions.DisplayDateTimeFormat);
            var s3 = dateTimeLowPrec.ToString();
            Assert.AreEqual(dateTimeLowPrec, StringExtensions.ParseDateTime(s2));
            Assert.AreEqual(dateTimeLowPrec, StringExtensions.ParseDateTime(s3));
        }

        [Test]
        public void ParsePointSizeRect()
        {
            var point = new Point(123, 456);
            var pointF = new PointF(123.456f, 456.789f);
            var size = new Size(321, 654);
            var sizeF = new SizeF(321.098f, 654.876f);
            var rect = new Rectangle(point, size);
            var rectF = new RectangleF(pointF, sizeF);
            Assert.AreEqual(point, StringExtensions.ParsePoint(point.ToString()));
            Assert.AreEqual(pointF, StringExtensions.ParsePointF(pointF.ToString()));
            Assert.AreEqual(size, StringExtensions.ParseSize(size.ToString()));
            Assert.AreEqual(sizeF, StringExtensions.ParseSizeF(sizeF.ToString()));
            Assert.AreEqual(rect, StringExtensions.ParseRectangle(rect.ToString()));
            Assert.AreEqual(rectF, StringExtensions.ParseRectangleF(rectF.ToString()));
        }

        [Test]
        public void RemoveNewLine() => Assert.AreEqual("Test1Test2Test3Test4", "Test1\rTest2\nTest3\r\nTest4".RemoveNewLine());

        [Test]
        public void ReplaceChars()
        {
            Assert.AreEqual("!#123bc...xy!#123", "abc...xyz".ReplaceChars(new[]
            {
                'a',
                'z'
            }, "!#123"));
            Assert.AreEqual("!#123bc!#123!#123!#123xy!#123", "abc...xyz".ReplaceChars("a.z", "!#123"));
        }

        [Test]
        public void ReplaceInvalidChars()
        {
            Assert.AreEqual("------5712-", "jagalw5712z".ReplaceInvalidChars("0123456789", "-"));
            Assert.AreEqual("-a-a-------", "jagalw5712z".ReplaceInvalidChars(new[] { 'a' }, "-"));
        }

        [Test]
        public void ReplaceNewLine() => Assert.AreEqual("Test1,Test2,Test3,Test4", "Test1\rTest2\nTest3\r\nTest4".ReplaceNewLine(","));

        [Test]
        public void SplitKeepSeparators()
        {
            var l_Expected = new[]
            {
                "Test1",
                "|",
                "Test2",
                "|",
                "|",
                "Test3"
            };
            var l_String = "Test1|Test2||Test3";
            CollectionAssert.AreEqual(l_Expected, l_String.SplitKeepSeparators('|'));
        }

        [Test]
        public void SplitNewLine()
        {
            var result = "Test1\r\nTest2 Test3\0TestTestTest4\rTest5\nTest6\0Test7\rTest8\n\r\0\r\n\0End".SplitNewLine();
            var l_Expected = new[]
            {
                "Test1",
                "Test2 Test3",
                "TestTestTest4",
                "Test5",
                "Test6",
                "Test7",
                "Test8",
                "",
                "",
                "",
                "",
                "End"
            };
            CollectionAssert.AreEqual(l_Expected, result);
        }

        [Test]
        public void SplitNewLine1()
        {
            var result = "Test1\r\nTest2 Test3\0TestTestTest4\rTest5\nTest6\0Test7\rTest8\n\r\0\r\n\0End".SplitNewLine(StringSplitOptions.RemoveEmptyEntries);
            var l_Expected = new[]
            {
                "Test1",
                "Test2 Test3",
                "TestTestTest4",
                "Test5",
                "Test6",
                "Test7",
                "Test8",
                "End"
            };
            CollectionAssert.AreEqual(l_Expected, result);
        }

        [Test]
        public void SplitNewLineAndLength()
        {
            var result = "Test1\r\nTest2 Test3\tTestTestTest4".SplitNewLineAndLength(6);
            var l_Expected = new[]
            {
                "Test1",
                "Test2 ",
                "Test3 ",
                "TestTe",
                "stTest",
                "4"
            };
            CollectionAssert.AreEqual(l_Expected, result);
        }

        [Test]
        public void ToHexString()
        {
            var data = new byte[]
            {
                0x12,
                0x34,
                0x56,
                0x78,
                0x9A,
                0xBC,
                0xDE,
                0xF0
            };
            Assert.AreEqual("123456789abcdef0", data.ToHexString());
            Assert.AreEqual("123456789ABCDEF0", data.ToHexString(true));
            CollectionAssert.AreEqual(data, StringExtensions.ParseHexString("123456789abcdef0"));
            CollectionAssert.AreEqual(data, StringExtensions.ParseHexString("123456789ABCDEF0"));
        }

        [Test]
        public void ToString1()
        {
            Assert.AreEqual("1", StringExtensions.ToString(1));
            Assert.AreEqual("1", StringExtensions.ToString(1.0d));
            Assert.AreEqual("<null>", StringExtensions.ToString(null));
            var l_List = new[]
            {
                1,
                2,
                3,
                4
            };
            Assert.AreEqual("System.Int32[] {1,2,3,4}", StringExtensions.ToString(l_List));
        }

        [Test]
        public void ToStringArray()
        {
            var array = new object[]
            {
                1,
                null,
                "3"
            };
            var expected = new[]
            {
                "1",
                "<null>",
                "3"
            };
            CollectionAssert.AreEqual(expected, array.ToStringArray());
        }

        [Test]
        public void ToStringParse()
        {
            void Test<T>(T value, CultureInfo culture)
            {
                var str = StringExtensions.ToString(value, culture);
                var read = typeof(T).ConvertValue(str, culture);
                Assert.AreEqual(value, read, $"Roundtrip ToString->ConvertValue not successful at type {typeof(T).Name} and culture {culture.Name}!");
            }

            void TestDateTime(DateTime? value, CultureInfo culture)
            {
                if (value is DateTime dt)
                {
                    var test = new DateTime(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond));
                    var str = StringExtensions.ToString(test, culture);
                    var read = typeof(DateTime).ConvertValue(str, culture);
                    if (!Equals(read, test))
                    {
                        Assert.Fail($"Roundtrip ToString->ConvertValue not successful at type DateTime and culture {culture.Name}!");
                    }
                }
                else
                {
                    Test(value, culture);
                }
            }

            var rnd = new Random();
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var culture in allCultures)
            {
                if (culture.IsNeutralCulture)
                {
                    continue;
                }

                if (culture.Calendar is GregorianCalendar)
                {
                }
                else
                {
                    continue;
                }

                Test((float)rnd.NextDouble(), culture);
                Test(rnd.NextDouble(), culture);
                Test((decimal)rnd.NextDouble(), culture);
                Test((sbyte)rnd.Next(), culture);
                Test((byte)rnd.Next(), culture);
                Test((short)rnd.Next(), culture);
                Test((ushort)rnd.Next(), culture);
                Test(rnd.Next(), culture);
                Test((uint)rnd.Next(), culture);
                Test(((long)rnd.Next() * rnd.Next()) + rnd.Next(), culture);
                Test((ulong)((rnd.Next() * rnd.Next()) + rnd.Next()), culture);
                Test(TimeSpan.FromDays(rnd.NextDouble()), culture);
                TestDateTime(DateTime.Now + TimeSpan.FromDays(rnd.NextDouble()), culture);
                Test((float?)rnd.NextDouble(), culture);
                Test((double?)rnd.NextDouble(), culture);
                Test((decimal?)rnd.NextDouble(), culture);
                Test((sbyte?)rnd.Next(), culture);
                Test((byte?)rnd.Next(), culture);
                Test((short?)rnd.Next(), culture);
                Test((ushort?)rnd.Next(), culture);
                Test((int?)rnd.Next(), culture);
                Test((uint?)rnd.Next(), culture);
                Test((long?)(((long)rnd.Next() * rnd.Next()) + rnd.Next()), culture);
                Test((ulong?)((rnd.Next() * rnd.Next()) + rnd.Next()), culture);
                Test((TimeSpan?)TimeSpan.FromDays(rnd.NextDouble()), culture);
                TestDateTime((DateTime?)(DateTime.Now + TimeSpan.FromDays(rnd.NextDouble())), culture);
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
                var buf = new byte[50];
                rnd.NextBytes(buf);
                Test(buf, culture);
                var arrayI = new[]
                {
                    rnd.Next(),
                    rnd.Next()
                };
                Test(arrayI, culture);
            }
        }

        [Test]
        public void TryParseDateTime()
        {
            var dateTimeLowPrec = new DateTime(2004, 12, 31, 12, 34, 56, DateTimeKind.Local);
            var dateTimeHighPrec = dateTimeLowPrec + TimeSpan.FromSeconds(0.1234567);
            var s1 = dateTimeHighPrec.ToString(StringExtensions.InterOpDateTimeFormat);
            Assert.IsTrue(StringExtensions.TryParseDateTime(s1, out var check));
            Assert.AreEqual(dateTimeHighPrec, check);
            var s2 = dateTimeLowPrec.ToString(StringExtensions.DisplayDateTimeFormat);
            var s3 = dateTimeLowPrec.ToString();
            Assert.IsTrue(StringExtensions.TryParseDateTime(s2, out check));
            Assert.AreEqual(dateTimeLowPrec, check);
            Assert.IsTrue(StringExtensions.TryParseDateTime(s3, out check));
            Assert.AreEqual(dateTimeLowPrec, check);
        }

        [Test]
        public void Unbox()
        {
            Assert.AreEqual("123", "(123)".Unbox("(", ")"));
            Assert.AreEqual("(123)", "(123)".Unbox("(", "-", false));
            Assert.AreEqual("(123)", "(123)".Unbox("-", ")", false));
            try
            {
                "(123)".Unbox("-", ")");
                Assert.Fail();
            }
            catch (Exception ex) { Assert.IsInstanceOf<FormatException>(ex); }

            Assert.AreEqual("123", "(123)".UnboxBrackets());
            Assert.AreEqual("123", "{123}".UnboxBrackets());
            Assert.AreEqual("123", "[123]".UnboxBrackets());
            Assert.AreEqual("(123]", "(123]".UnboxBrackets(false));
            try
            {
                "123)".UnboxBrackets();
                Assert.Fail();
            }
            catch (Exception ex) { Assert.IsInstanceOf<FormatException>(ex); }

            Assert.AreEqual("123", "'123'".UnboxText());
            Assert.AreEqual("123", "\"123\"".UnboxText());
            Assert.AreEqual("'123\"", "'123\"".UnboxText(false));
            try
            {
                "123'".UnboxText();
                Assert.Fail();
            }
            catch (Exception ex) { Assert.IsInstanceOf<FormatException>(ex); }
        }

        [Test]
        public void UnEscape()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < ushort.MaxValue; i++)
            {
                sb.Append(Encoding.Unicode.GetString(BitConverter.GetBytes((ushort)i)));
            }

            var text = sb.ToString();
            var escaped = text.Escape();
            var unescaped = escaped.Unescape();
            Assert.AreEqual(text, unescaped);
        }
    }
}
