using System;
using System.Threading.Tasks;
using Cave;
using NUnit.Framework;

namespace Test;

[TestFixture]
class MonotonicTimeTests
{
    [Test]
    public void MonotonicTest()
    {
        Console.WriteLine($"Uptime: {MonotonicTime.Uptime}");
        Console.WriteLine($"Now: {(InteropDateTime)MonotonicTime.Now}");
        Console.WriteLine($"UtcNow: {(InteropDateTime)MonotonicTime.UtcNow}");
        Console.WriteLine($"Drift: {MonotonicTime.Drift.FormatTime()}");

        Task.WaitAll(new[]
        {
            Task.Factory.StartNew(CalibrateTest),
            Task.Factory.StartNew(WatchTicks),
            Task.Factory.StartNew(WatchTime),
        });
    }

    void WatchTicks()
    {
        var start = MonotonicTime.UtcNow;
        var end = start + TimeSpan.FromSeconds(15);
        while (MonotonicTime.UtcNow < end)
        {
            var times = new TimeSpan[100];
            for (int i = 0; i < times.Length; i++)
            {
                times[i] = MonotonicTime.Uptime;
            }
            for (int i = 0; i < times.Length - 1; i++)
            {
                Assert.IsTrue(times[i] <= times[i + 1], $"Uptime is not monotonic! {i}: {times[i].Ticks} > {times[i + 1].Ticks}");
            }
        }
    }

    void WatchTime()
    {
        var start = MonotonicTime.UtcNow;
        var end = start + TimeSpan.FromSeconds(15);
        while (MonotonicTime.UtcNow < end)
        {
            var times = new DateTime[100];
            for (int i = 0; i < times.Length; i++)
            {
                times[i] = MonotonicTime.UtcNow;
            }
            for (int i = 0; i < times.Length - 1; i++)
            {
                Assert.IsTrue(times[i] <= times[i + 1], $"UtcNow is not monotonic! {i}: {times[i].TimeOfDay.Ticks} > {times[i + 1].TimeOfDay.Ticks}");
            }
        }
    }

    void CalibrateTest()
    {
        var end = MonotonicTime.UtcNow + TimeSpan.FromSeconds(10);
        int i = 0;
        while (MonotonicTime.UtcNow < end)
        {
            Assert.IsTrue(MonotonicTime.Calibrate(), "Calibration failed!");
            var drift = MonotonicTime.Drift;
            Assert.IsTrue(Math.Abs(drift.Ticks) < TimeSpan.TicksPerMillisecond, "Drift > 1ms");
            i++;
        }
        Assert.IsTrue(i > 1, "Calibrate Test did not run!");
    }
}
