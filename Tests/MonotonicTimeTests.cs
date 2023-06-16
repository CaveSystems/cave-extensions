using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cave;
using NUnit.Framework;

namespace Test;

[TestFixture]
class MonotonicTimeTests
{
    [Test]
    public void MonotonicTimeTest()
    {
        Console.WriteLine("Properties:"); 
        Console.WriteLine($"Uptime: {MonotonicTime.Uptime}");
        Console.WriteLine($"Now: {(InteropDateTime)MonotonicTime.Now}");
        Console.WriteLine($"UtcNow: {(InteropDateTime)MonotonicTime.UtcNow}");
        Console.WriteLine($"Drift: {MonotonicTime.GetDrift().Average.FormatTime()}");
        Console.WriteLine($"IsHighResolution: {MonotonicTime.IsHighResolution}");

        var result = MonotonicTime.Calibrate();
        Console.WriteLine($"Drift after first Calibration: {result.Average.FormatTime()}");

        Console.WriteLine("---");
        Console.WriteLine("Test:");
        Console.WriteLine($"CalibrationCount: {calibrationCount}");

        var tasks = new[]
        {
            Task.Factory.StartNew(WatchTicks),
            Task.Factory.StartNew(WatchTime),
        };
        CalibrateTest();
        Task.WaitAll(tasks);
    }

    int calibrationCount = 5;

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
                if (times[i] > times[i + 1]) Debugger.Break();
                Assert.IsTrue(times[i] <= times[i + 1], $"UtcNow is not monotonic! {i}: {times[i].TimeOfDay.Ticks} > {times[i + 1].TimeOfDay.Ticks}");
            }
        }
    }

    void CalibrateTest()
    {
        var i = 0;
        while (calibrationCount > 0)
        {
#if NET20 || NET35
            //net 2.0 and 3.5 use a 15msec timer for datetime. This affects our accuracy during tests.
            //we might not be able to keep the accuracy in all cases below 1ms.
            var accuracy = TimeSpan.FromMilliseconds(5);
#else
            var accuracy = TimeSpan.FromMilliseconds(1);
#endif
            Assert.IsTrue(MonotonicTime.Calibrate() < accuracy, $"Calibration failed! Drift > {accuracy.FormatTime()}");
            var drift = (TimeSpan)MonotonicTime.GetDrift();
            Assert.IsTrue(Math.Abs(drift.Ticks) < accuracy.Ticks, $"Drift < {accuracy.FormatTime()}: real {drift.Absolute().FormatTime()}");
            Console.WriteLine($"Calibration Test {++i} drift {drift.FormatTime()} < {accuracy.FormatTime()}, current time {(InteropDateTime)MonotonicTime.Now}");
            calibrationCount--;
        }
    }
}
