using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cave;
using NUnit.Framework;

namespace Test.Strings;

[TestFixture]
public class StringExtensionsTests
{
    readonly IEnumerable<CultureInfo> allCultures =
#if !(NETCOREAPP1_0_OR_GREATER && !NETCOREAPP2_0_OR_GREATER)
        CultureInfo.GetCultures(CultureTypes.AllCultures).Where(c => !c.IsNeutralCulture);

#else
        new[] { CultureInfo.CurrentCulture, new CultureInfo("de-DE"), new CultureInfo("en-US") };

#endif

    [Test]
    public void GetCasingTest()
    {
        Assert.AreEqual("testText123", "Test Text 123".GetCamelCaseName());
        Assert.AreEqual("testText123", "Test Text 123".GetLowerCamelCaseName());
        Assert.AreEqual("TestText123", "Test Text 123".GetPascalCaseName());
        Assert.AreEqual("test_text_123", "Test Text 123".GetSnakeCaseName());
        Assert.AreEqual("test-text-123", "Test Text 123".GetKebabCaseName());
    }

    [Test]
    public void SplitCasingTest()
    {
        Assert.AreEqual(new[] { "Test", "ID" }.Join('|'), "TestID".SplitCamelCase().Join('|'));
        Assert.AreEqual(new[] { "Test", "Id" }.Join('|'), "TestId".SplitCamelCase().Join('|'));
        Assert.AreEqual(new[] { "test", "ID" }.Join('|'), "testID".SplitCamelCase().Join('|'));
        Assert.AreEqual(new[] { "test", "Id" }.Join('|'), "testId".SplitCamelCase().Join('|'));
        Assert.AreEqual(new[] { "Test", "ID", "123" }.Join('|'), "TestID123".SplitCamelCase().Join('|'));
        Assert.AreEqual(new[] { "Test", "Id", "123" }.Join('|'), "TestId123".SplitCamelCase().Join('|'));
        Assert.AreEqual(new[] { "test", "ID", "123" }.Join('|'), "testID123".SplitCamelCase().Join('|'));
        Assert.AreEqual(new[] { "test", "Id", "123" }.Join('|'), "testId123".SplitCamelCase().Join('|'));

        Assert.AreEqual(new[] { "Test", "ID" }.Join('|'), "TestID".SplitCasing().Join('|'));
        Assert.AreEqual(new[] { "Test", "Id" }.Join('|'), "TestId".SplitCasing().Join('|'));
        Assert.AreEqual(new[] { "test", "ID" }.Join('|'), "testID".SplitCasing().Join('|'));
        Assert.AreEqual(new[] { "test", "Id" }.Join('|'), "testId".SplitCasing().Join('|'));
        Assert.AreEqual(new[] { "Test", "ID", "123" }.Join('|'), "TestID123".SplitCasing().Join('|'));
        Assert.AreEqual(new[] { "Test", "Id", "123" }.Join('|'), "TestId123".SplitCasing().Join('|'));
        Assert.AreEqual(new[] { "test", "ID", "123" }.Join('|'), "testID123".SplitCasing().Join('|'));
        Assert.AreEqual(new[] { "test", "Id", "123" }.Join('|'), "testId123".SplitCasing().Join('|'));

        Assert.AreEqual(new[] { "Test", "ID", "123", "Special", "2", "splits" }.Join('|'), "TestID123Special2splits".SplitCasing().Join('|'));
    }

    [Test]
    public void CamelCaseTest()
    {
        const string expected = "TestId";
        Assert.AreEqual(expected, new[] { "Test", "ID" }.JoinCamelCase());
        Assert.AreEqual(expected, new[] { "Test", "Id" }.JoinCamelCase());
        Assert.AreEqual(expected, new[] { "test", "ID" }.JoinCamelCase());
        Assert.AreEqual(expected, new[] { "test", "Id" }.JoinCamelCase());
        Assert.AreEqual(expected, new[] { "test", "id" }.JoinCamelCase());
        Assert.AreEqual(expected, new[] { "teSt", "iD" }.JoinCamelCase());
    }

    [Test]
    public void PascalCaseTest()
    {
        const string expected = "TestId";
        Assert.AreEqual(expected, new[] { "Test", "ID" }.JoinPascalCase());
        Assert.AreEqual(expected, new[] { "Test", "Id" }.JoinPascalCase());
        Assert.AreEqual(expected, new[] { "test", "ID" }.JoinPascalCase());
        Assert.AreEqual(expected, new[] { "test", "Id" }.JoinPascalCase());
        Assert.AreEqual(expected, new[] { "test", "id" }.JoinPascalCase());
        Assert.AreEqual(expected, new[] { "teSt", "iD" }.JoinPascalCase());
    }

    [Test]
    public void SnakeCaseTest()
    {
        const string expected = "test_id";
        Assert.AreEqual(expected, new[] { "Test", "ID" }.JoinSnakeCase());
        Assert.AreEqual(expected, new[] { "Test", "Id" }.JoinSnakeCase());
        Assert.AreEqual(expected, new[] { "test", "ID" }.JoinSnakeCase());
        Assert.AreEqual(expected, new[] { "test", "Id" }.JoinSnakeCase());
        Assert.AreEqual(expected, new[] { "test", "id" }.JoinSnakeCase());
        Assert.AreEqual(expected, new[] { "teSt", "iD" }.JoinSnakeCase());
    }

    [Test]
    public void KebabCaseTest()
    {
        const string expected = "test-id";
        Assert.AreEqual(expected, new[] { "Test", "ID" }.JoinKebabCase());
        Assert.AreEqual(expected, new[] { "Test", "Id" }.JoinKebabCase());
        Assert.AreEqual(expected, new[] { "test", "ID" }.JoinKebabCase());
        Assert.AreEqual(expected, new[] { "test", "Id" }.JoinKebabCase());
        Assert.AreEqual(expected, new[] { "test", "id" }.JoinKebabCase());
        Assert.AreEqual(expected, new[] { "teSt", "iD" }.JoinKebabCase());
    }

    [Test]
    public void LowerCamelCaseTest()
    {
        const string expected = "testId";
        Assert.AreEqual(expected, new[] { "Test", "ID" }.JoinLowerCamelCase());
        Assert.AreEqual(expected, new[] { "Test", "Id" }.JoinLowerCamelCase());
        Assert.AreEqual(expected, new[] { "test", "ID" }.JoinLowerCamelCase());
        Assert.AreEqual(expected, new[] { "test", "Id" }.JoinLowerCamelCase());
        Assert.AreEqual(expected, new[] { "test", "id" }.JoinLowerCamelCase());
        Assert.AreEqual(expected, new[] { "teSt", "iD" }.JoinLowerCamelCase());
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
                if (size is > 10 and < long.MaxValue)
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
    public void FormatTimeString()
    {
        var value = 1.234;
        Assert.AreEqual("1.23ns", (value / 1000 / 1000 / 1000).FormatTime());
        Assert.AreEqual("1.23µs", (value / 1000 / 1000).FormatTime());
        Assert.AreEqual("1.23ms", (value / 1000).FormatTime());
        Assert.AreEqual("1.23s", value.FormatTime());
        Assert.AreEqual("1.23min", (value * 60).FormatTime());
        Assert.AreEqual("1.23h", (value * 3600).FormatTime());
        Assert.AreEqual("1.23d", (value * 3600 * 24).FormatTime());

        value = 12.3678;
        Assert.AreEqual("12.4ns", (value / 1000 / 1000 / 1000).FormatTime());
        Assert.AreEqual("12.4µs", (value / 1000 / 1000).FormatTime());
        Assert.AreEqual("12.4ms", (value / 1000).FormatTime());
        Assert.AreEqual("12.4s", value.FormatTime());
        Assert.AreEqual("12.4min", (value * 60).FormatTime());
        Assert.AreEqual("12.4h", (value * 3600).FormatTime());
        Assert.AreEqual("12.4d", (value * 3600 * 24).FormatTime());

        Assert.AreEqual("12.3678ns", (value / 1000 / 1000 / 1000).FormatSeconds("g"));
        Assert.AreEqual("12.3678µs", (value / 1000 / 1000).FormatSeconds("g"));
        Assert.AreEqual("12.3678ms", (value / 1000).FormatSeconds("g"));
        Assert.AreEqual("12.3678s", value.FormatSeconds("g"));
        Assert.AreEqual("12.3678min", (value * 60).FormatSeconds("g"));
        Assert.AreEqual("12.3678h", (value * 3600).FormatSeconds("g"));
        Assert.AreEqual("12.3678d", (value * 3600 * 24).FormatSeconds("g"));
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
            var end = "/fio";
            var expected = "GiudgsfpUIGFEIPUEGT";
            var str = data.GetString(-1, start, end);
            Assert.AreEqual(expected, str);
        }
        {
            var start = '<';
            var end = '/';
            var expected = "IUFGiudgsfpUIGFEIPUEGT";
            var str = data.GetString(-1, start, end);
            Assert.AreEqual(expected, str);
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
    public void JoinNewLineNull()
    {
        string[] array = null;
        Assert.AreEqual(string.Empty, array.JoinNewLine());
    }

    [Test]
    public void JoinNull()
    {
        string[] array = null;
        Assert.AreEqual(string.Empty, array.Join("-"));
        Assert.AreEqual(string.Empty, array.Join('-'));
    }

    [Test]
    public void ParseDateTime()
    {
        var dateTimeLowPrec = new DateTime(2004, 12, 31, 12, 34, 56, DateTimeKind.Local);
        var dateTimeHighPrec = dateTimeLowPrec + TimeSpan.FromSeconds(0.1234567);
        var s1 = dateTimeHighPrec.ToString(StringExtensions.InteropDateTimeFormat);
        Assert.AreEqual(dateTimeHighPrec, s1.ParseDateTime());
        var s2 = dateTimeLowPrec.ToString(StringExtensions.DisplayDateTimeFormat);
        var s3 = dateTimeLowPrec.ToString();
        Assert.AreEqual(dateTimeLowPrec, s2.ParseDateTime());
        Assert.AreEqual(dateTimeLowPrec, s3.ParseDateTime());
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
        Assert.AreEqual(point, point.ToString().ParsePoint());
        Assert.AreEqual(pointF, pointF.ToString().ParsePointF());
        Assert.AreEqual(size, size.ToString().ParseSize());
        Assert.AreEqual(sizeF, sizeF.ToString().ParseSizeF());
        Assert.AreEqual(rect, rect.ToString().ParseRectangle());
        Assert.AreEqual(rectF, rectF.ToString().ParseRectangleF());
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
        var expected = new[]
        {
            "Test1",
            "|",
            "Test2",
            "|",
            "|",
            "Test3"
        };
        var l_String = "Test1|Test2||Test3";
        CollectionAssert.AreEqual(expected, l_String.SplitKeepSeparators('|'));
    }

    [Test]
    public void SplitNewLine()
    {
        var result = "Test1\r\nTest2 Test3\0TestTestTest4\rTest5\nTest6\0Test7\rTest8\n\r\0\r\n\0End".SplitNewLine();
        var expected = new[]
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
        CollectionAssert.AreEqual(expected, result);
    }

    [Test]
    public void SplitNewLine1()
    {
        var result = "Test1\r\nTest2 Test3\0TestTestTest4\rTest5\nTest6\0Test7\rTest8\n\r\0\r\n\0End".SplitNewLine(StringSplitOptions.RemoveEmptyEntries);
        var expected = new[]
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
        CollectionAssert.AreEqual(expected, result);
    }

    [Test]
    public void SplitNewLineAndLength()
    {
        var result = "Test1\r\nTest2 Test3\tTestTestTest4".SplitNewLineAndLength(6);
        var expected = new[]
        {
            "Test1",
            "Test2 ",
            "Test3 ",
            "TestTe",
            "stTest",
            "4"
        };
        CollectionAssert.AreEqual(expected, result);
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
        CollectionAssert.AreEqual(data, "123456789abcdef0".ParseHexString());
        CollectionAssert.AreEqual(data, "123456789ABCDEF0".ParseHexString());
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

    static void ToStringParseTestValue<T>(T value, CultureInfo culture)
    {
        var type = typeof(T);
#if NETCOREAPP1_0 || NETCOREAPP1_1
            var typeInfo = type.GetTypeInfo();
            var isGeneric = typeInfo.IsGenericType;
            var valueType = isGeneric ? typeInfo.GetGenericArguments().Single() : null;
            var typeName = isGeneric ? type.Name + $" ({valueType.Name})" : type.Name;
#else
        var isGeneric = type.IsGenericType;
        var valueType = isGeneric ? type.GetGenericArguments().Single() : null;
        var typeName = isGeneric ? type.Name + $" ({valueType.Name})" : type.Name;
#endif

        var str = StringExtensions.ToString(value, culture);
        var read = typeof(T).ConvertValue(str, culture);
        if (value is not null && culture.Name.Contains("-SS") && valueType == typeof(double))
        {
            //Bug in some ToString(CultureInfo("*-SS"), "R") implementations
            Assert.AreEqual(Convert.ToDouble(value), (double)read, 0.0000000000001d, $"Roundtrip ToString->ConvertValue not successful at type {typeName} and culture {culture.Name}!");
        }
        else
        {
            Assert.AreEqual(value, read, $"Roundtrip ToString->ConvertValue not successful at type {typeName} and culture {culture.Name}!");
        }
    }

    static void ToStringParseTestDateTime(DateTime? value, CultureInfo culture)
    {
        if (value is DateTime dt)
        {
            var test = new DateTime(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond));
            var str = StringExtensions.ToString(test, culture);
            var read = typeof(DateTime).ConvertValue(str, culture);
            if (!Equals(read, test))
            {
                if (Equals(new CultureInfo("mi-NZ"), culture))
                {
#if NET5_0_OR_GREATER
                    Assert.Warn("NET Framework bug. Culture mi-NZ fails AM / PM test when converting to and from string.");
#else
                    Assert.Inconclusive("NET Framework bug. Culture mi-NZ fails AM / PM test when converting to and from string.");
#endif
                    return;
                }
                Assert.Fail($"Roundtrip ToString->ConvertValue not successful at type DateTime and culture {culture.Name}! '{test}' is not equal to '{read}' (string '{str}')!");
            }
        }
        else
        {
            ToStringParseTestValue(value, culture);
        }
    }

    static void ToStringParseTestCulture(CultureInfo culture)
    {
        var rnd = new Random(1337);
        if (culture.IsNeutralCulture)
        {
            return;
        }

        if (culture.Calendar is GregorianCalendar) { }
        else
        {
            return;
        }

        for (int i = 0; i < 10; i++)
        {
            double NextDouble() => (rnd.NextDouble() / rnd.NextDouble()) + double.MinValue;
            ToStringParseTestValue((float)NextDouble(), culture);
            ToStringParseTestValue(NextDouble(), culture);
            ToStringParseTestValue((decimal)(NextDouble() % 1d), culture);
            ToStringParseTestValue((sbyte)rnd.Next(), culture);
            ToStringParseTestValue((byte)rnd.Next(), culture);
            ToStringParseTestValue((short)rnd.Next(), culture);
            ToStringParseTestValue((ushort)rnd.Next(), culture);
            ToStringParseTestValue(rnd.Next(), culture);
            ToStringParseTestValue((uint)rnd.Next(), culture);
            ToStringParseTestValue(((long)rnd.Next() * rnd.Next()) + rnd.Next(), culture);
            ToStringParseTestValue((ulong)((rnd.Next() * rnd.Next()) + rnd.Next()), culture);
            ToStringParseTestValue(TimeSpan.FromDays(rnd.NextDouble()), culture);
            ToStringParseTestDateTime(DateTime.Now + TimeSpan.FromDays(rnd.NextDouble()), culture);
            ToStringParseTestValue((float?)rnd.NextDouble(), culture);
            ToStringParseTestValue((double?)rnd.NextDouble(), culture);
            ToStringParseTestValue((decimal?)rnd.NextDouble(), culture);
            ToStringParseTestValue((sbyte?)rnd.Next(), culture);
            ToStringParseTestValue((byte?)rnd.Next(), culture);
            ToStringParseTestValue((short?)rnd.Next(), culture);
            ToStringParseTestValue((ushort?)rnd.Next(), culture);
            ToStringParseTestValue((int?)rnd.Next(), culture);
            ToStringParseTestValue((uint?)rnd.Next(), culture);
            ToStringParseTestValue((long?)(((long)rnd.Next() * rnd.Next()) + rnd.Next()), culture);
            ToStringParseTestValue((ulong?)((rnd.Next() * rnd.Next()) + rnd.Next()), culture);
            ToStringParseTestValue((TimeSpan?)TimeSpan.FromDays(rnd.NextDouble()), culture);
            ToStringParseTestDateTime(DateTime.Now + TimeSpan.FromDays(rnd.NextDouble()), culture);
            ToStringParseTestValue((float?)null, culture);
            ToStringParseTestValue((double?)null, culture);
            ToStringParseTestValue((decimal?)null, culture);
            ToStringParseTestValue((sbyte?)null, culture);
            ToStringParseTestValue((byte?)null, culture);
            ToStringParseTestValue((short?)null, culture);
            ToStringParseTestValue((ushort?)null, culture);
            ToStringParseTestValue((int?)null, culture);
            ToStringParseTestValue((uint?)null, culture);
            ToStringParseTestValue((long?)null, culture);
            ToStringParseTestValue((ulong?)null, culture);
            ToStringParseTestValue((TimeSpan?)null, culture);
            ToStringParseTestValue((DateTime?)null, culture);
            var buf = new byte[50];
            rnd.NextBytes(buf);
            ToStringParseTestValue(buf, culture);
            var arrayI = new[] { rnd.Next(), rnd.Next() };
            ToStringParseTestValue(arrayI, culture);
        }
    }

    [Test]
    public void ToStringParse()
    {
        foreach (var culture in allCultures)
        {
            ToStringParseTestCulture(culture);
        };
    }

    [Test]
    public void TryParseDateTime()
    {
        var dateTimeLowPrec = new DateTime(2004, 12, 31, 12, 34, 56, DateTimeKind.Local);
        var dateTimeHighPrec = dateTimeLowPrec + TimeSpan.FromSeconds(0.1234567);
        var s1 = dateTimeHighPrec.ToString(StringExtensions.InteropDateTimeFormat);
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
