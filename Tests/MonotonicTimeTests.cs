using System;
using System.Threading;
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

    int calibrationCount = MonotonicTime.IsHighResolution ? 100 : 10;

    void WatchTicks()
    {
        while (calibrationCount > 0)
        {
            Thread.Sleep(0);
            var times = new TimeSpan[500];
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
        while (calibrationCount > 0)
        {
            Thread.Sleep(0);
            var times = new DateTime[500];
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
        while (calibrationCount > 0)
        {
            var accuracy = new TimeSpan((calibrationCount * TimeSpan.TicksPerMillisecond) / 10);
            Assert.IsTrue(MonotonicTime.Calibrate(), "Calibration failed!");
            var drift = MonotonicTime.Drift;
            Console.WriteLine($"MonotonicTime calibrated to {accuracy.FormatTime()} current drift: {drift.FormatTime()}");
            Assert.IsTrue(Math.Abs(drift.Ticks) < accuracy.Ticks, $"Drift > {accuracy.FormatTime()}");
            calibrationCount--;
        }
    }
}
