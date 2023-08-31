using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Test.Backports;

[TestFixture]
class FormattableStringTest
{
    [Test]
    public void TestType()
    {
        IFormattable f = $"Test double 1234.56 -> {1234.56:N2}";
        if (f is not FormattableString fs)
        {
            Assert.Fail("f is not FormattableString");
            throw new();
        }
        Assert.AreEqual("Test double 1234.56 -> 1,234.56", f.ToString(null, CultureInfo.InvariantCulture));
        Assert.AreEqual("Test double 1234.56 -> 1.234,56", f.ToString(null, new CultureInfo("de-DE")));
        Assert.AreEqual("Test double 1234.56 -> 1,234.56", fs.ToString(CultureInfo.InvariantCulture));
        Assert.AreEqual("Test double 1234.56 -> 1.234,56", fs.ToString(new CultureInfo("de-DE")));
    }

    [Test]
    public void TestSystemRuntime()
    {
        var fs = FormattableStringFactory.Create("{0} {1:N2} {2:G6}", 123456, 123456789.12345, 12.3456789);
        Assert.IsInstanceOf<FormattableString>(fs);
        Assert.AreEqual("123456 123,456,789.12 12.3457", fs.ToString(CultureInfo.InvariantCulture));
        Assert.AreEqual("123456 123.456.789,12 12,3457", fs.ToString(new CultureInfo("de-DE")));
    }
}
