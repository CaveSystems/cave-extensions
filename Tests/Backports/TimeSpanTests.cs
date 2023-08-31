using System;
using System.Globalization;
using NUnit.Framework;

namespace Test.Backports;

[TestFixture]
class TimeSpanTests
{
    [Test]
    public void PositiveTimeToString()
    {
        var time = new TimeSpan(1, 2, 59, 56, 987);
        Assert.AreEqual("1.02:59:56.9870000", time.ToString());

        Assert.AreEqual("2:59", time.ToString("h':'mm"));
        Assert.AreEqual("02:59", time.ToString("hh':'mm"));
        Assert.AreEqual("02:59:56", time.ToString("hh':'mm':'ss"));
        Assert.AreEqual("02:59:56.9", time.ToString("hh':'mm':'ss'.'f"));
        Assert.AreEqual("02:59:56.987", time.ToString("hh':'mm':'ss'.'fff"));
        Assert.AreEqual("02:59:56.9870", time.ToString("hh':'mm':'ss'.'ffff"));
        Assert.AreEqual("1.02:59:56.98", time.ToString(@"d\.hh\:mm\:ss\.ff"));

        var de = new CultureInfo("de-DE");
        Assert.AreEqual("1.02:59:56.9870000", time.ToString("c", de));
        Assert.AreEqual("1.02:59:56.9870000", time.ToString("t", de));
        Assert.AreEqual("1.02:59:56.9870000", time.ToString("T", de));
        Assert.AreEqual("1:2:59:56,987", time.ToString("g", de));
        Assert.AreEqual("1:02:59:56,9870000", time.ToString("G", de));

        time = new(1, 3, 16, 50, 500);
        Assert.AreEqual("1.03:16:50.5000000", time.ToString("c", de));
        Assert.AreEqual("1:3:16:50,5", time.ToString("g", de));
        Assert.AreEqual("1:03:16:50,5000000", time.ToString("G", de));
    }

    [Test]
    public void NegativeTimeToString()
    {
        var time = -new TimeSpan(1, 02, 59, 56, 987);
        Assert.AreEqual("-1.02:59:56.9870000", time.ToString());

        Assert.AreEqual("2:59", time.ToString("h':'mm"));
        Assert.AreEqual("02:59", time.ToString("hh':'mm"));
        Assert.AreEqual("02:59:56", time.ToString("hh':'mm':'ss"));
        Assert.AreEqual("02:59:56.9", time.ToString("hh':'mm':'ss'.'f"));
        Assert.AreEqual("02:59:56.987", time.ToString("hh':'mm':'ss'.'fff"));
        Assert.AreEqual("02:59:56.9870", time.ToString("hh':'mm':'ss'.'ffff"));
        Assert.AreEqual("1.02:59:56.98", time.ToString(@"d\.hh\:mm\:ss\.ff"));

        var de = new CultureInfo("de-DE");
        Assert.AreEqual("-1.02:59:56.9870000", time.ToString("c", de));
        Assert.AreEqual("-1.02:59:56.9870000", time.ToString("t", de));
        Assert.AreEqual("-1.02:59:56.9870000", time.ToString("T", de));
        Assert.AreEqual("-1:2:59:56,987", time.ToString("g", de));
        Assert.AreEqual("-1:02:59:56,9870000", time.ToString("G", de));

        time = -new TimeSpan(1, 3, 16, 50, 500);
        Assert.AreEqual("-1.03:16:50.5000000", time.ToString("c", de));
        Assert.AreEqual("-1:3:16:50,5", time.ToString("g", de));
        Assert.AreEqual("-1:03:16:50,5000000", time.ToString("G", de));
    }
}
