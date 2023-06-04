using System;
using System.Diagnostics;
using System.Globalization;
using Cave;
using NUnit.Framework;

namespace Test;

[TestFixture]
class DateTimeParserTest
{
    static void Test(CultureInfo ci, DateTime dt, string format, TimeSpan? delta = null)
    {
        if (ci.Calendar is not GregorianCalendar)
        {
            Debug.WriteLine($"Cannot test culture {ci} and format {format}!");
            return;
        }
        var text = dt.ToString(format, ci);
        var rt = text.ParseDateTime();
        if (delta is TimeSpan diff)
        {
            var max = dt + diff;
            var min = dt - diff;
            Assert.IsTrue((dt < max) && (dt > min), $"Roundtrip failed for culture {ci} and format {format}!");
        }
        else
        {
            Assert.AreEqual(dt, rt, $"Roundtrip failed for culture {ci} and format {format}!");
        }
    }

#if !(NETCOREAPP1_0_OR_GREATER && !NETCOREAPP2_0_OR_GREATER)
    [Test]
    public void DateTimeParserTests()
    {
        foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
        {
            Test(culture, DateTime.Now, StringExtensions.InteropDateTimeFormat);
            Test(culture, DateTime.Now, StringExtensions.InteropDateTimeFormatWithoutTimeZone);
            Test(culture, DateTime.Now, StringExtensions.DisplayDateTimeWithTimeZoneFormat, TimeSpan.FromMilliseconds(1));
            Test(culture, DateTime.Now, StringExtensions.DisplayDateTimeFormat, TimeSpan.FromMilliseconds(1));
        }
    }
#endif
}
