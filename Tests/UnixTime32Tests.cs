using Cave;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnixTime.Tests
{
    [TestFixture]
    public class TestUnixTime32
    {
        #region Private Fields

        const int structSize = 4;

        #endregion Private Fields

        #region Public Methods

        [Test]
        public void TestMethodAdd()
        {
            Random rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var datetime = new DateTime(rnd.Next(1970, 2035), rnd.Next(1, 13), rnd.Next(1, 29), rnd.Next(0, 24), rnd.Next(0, 60), rnd.Next(0, 60), (DateTimeKind)rnd.Next(0, 3));
                UnixTime32 u32 = datetime;

                for (int n = 0; n < 100; n++)
                {
                    int seconds = (int)(rnd.NextDouble() * uint.MaxValue + int.MinValue);

                    while (true)
                    {
                        long test = seconds;
                        test += u32.TimeStamp;
                        if (test > uint.MaxValue || test < 0)
                        {
                            //would overflow or underflow
                            seconds /= 3;
                            continue;
                        }
                        break;
                    }

                    var ts = TimeSpan.FromSeconds(seconds);
                    u32 += ts;
                    datetime = datetime + ts;

                    Assert.AreEqual(datetime, (DateTime)u32);
                    Assert.AreEqual(true, u32 == datetime);
                    Assert.AreEqual(true, u32 == u32.TimeStamp);
                    Assert.AreEqual(false, u32 != datetime);
                    Assert.AreEqual(false, u32 != u32.TimeStamp);
                }
            }
        }

        [Test]
        public void TestMethodCompareTo()
        {
            Random rnd = new Random();
            List<DateTime> test1 = new List<DateTime>();
            List<UnixTime32> test2 = new List<UnixTime32>();
            for (int i = 0; i < 1000; i++)
            {
                var datetime = new DateTime(rnd.Next(1970, 2035), rnd.Next(1, 13), rnd.Next(1, 29), rnd.Next(0, 24), rnd.Next(0, 60), rnd.Next(0, 60), (DateTimeKind)rnd.Next(0, 3));
                test1.Add(datetime);
                test2.Add((UnixTime32)datetime);
            }

            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(test1[i], (DateTime)test2[i]);
                Assert.AreEqual((UnixTime32)test1[i], test2[i]);
            }
            test1.Sort();
            test2.Sort();
            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(test1[i], (DateTime)test2[i]);
                Assert.AreEqual((UnixTime32)test1[i], test2[i]);
                Assert.AreEqual(true, test2[i].Equals(test1[i]));
            }
        }

        [Test]
        public void TestMethodComparison()
        {
            Random rnd = new Random();
            DateTime last = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                var datetime = new DateTime(rnd.Next(1970, 2035), rnd.Next(1, 13), rnd.Next(1, 29), rnd.Next(0, 24), rnd.Next(0, 60), rnd.Next(0, 60), (DateTimeKind)rnd.Next(0, 3));
                Assert.AreEqual(datetime < last, (UnixTime32)datetime < last);
                Assert.AreEqual(datetime > last, (UnixTime32)datetime > last);
                Assert.AreEqual(datetime <= last, (UnixTime32)datetime <= last);
                Assert.AreEqual(datetime >= last, (UnixTime32)datetime >= last);
                last = datetime;
            }
            DateTime copy = last;
            Assert.AreEqual(copy < last, (UnixTime32)copy < last);
            Assert.AreEqual(copy > last, (UnixTime32)copy > last);
            Assert.AreEqual(copy <= last, (UnixTime32)copy <= last);
            Assert.AreEqual(copy >= last, (UnixTime32)copy >= last);
            Assert.AreEqual(true, ((UnixTime32)copy).GetHashCode() == ((UnixTime32)last).GetHashCode());
        }

        [Test]
        public void TestMethodNow()
        {
            TimeSpan diff = default(TimeSpan);
            for (int i = 0; i < 10000; i++)
            {
                var u32 = UnixTime32.Now;
                var u32utc = UnixTime32.UtcNow;
                diff += u32.DateTime.ToUniversalTime() - u32utc.DateTime;
            }
            Assert.AreEqual(true, Math.Abs(diff.Ticks) < TimeSpan.TicksPerSecond);
        }

        [Test]
        public void TestMethodOverflow()
        {
            var datetime = DateTime.Now;
            UnixTime32 u32 = datetime;
            TimeSpan outOfRange = new TimeSpan(TimeSpan.TicksPerDay * 365 * 200);
            try
            {
                u32 += outOfRange;
            }
            catch (Exception ex)
            {
                Assert.AreEqual(typeof(OverflowException), ex.GetType());
            }
            try
            {
                u32 -= outOfRange;
            }
            catch (Exception ex)
            {
                Assert.AreEqual(typeof(OverflowException), ex.GetType());
            }

            for (int i = -9; i < 10; i++)
            {
                DateTime check = new DateTime(1970, 1, 1) + TimeSpan.FromSeconds((uint.MaxValue + 1L) * i);
                Assert.AreEqual(check, UnixTime32.Convert(0, DateTimeKind.Local, TimeSpan.FromHours(i)));
                Assert.AreEqual(check, UnixTime32.Convert(0, DateTimeKind.Utc, TimeSpan.FromHours(i)));
                Assert.AreEqual(check, UnixTime32.Convert(0, DateTimeKind.Unspecified, TimeSpan.FromHours(i)));
            }

            for (int i = -9; i < 10; i++)
            {
                TimeSpan zone = new TimeSpan(2, 0, 0);
                DateTime check = new DateTime(1970, 1, 1) + TimeSpan.FromSeconds((uint.MaxValue + 1L) * i) - zone;
                Assert.AreEqual(check, UnixTime32.ConvertToUTC(0, TimeSpan.FromHours(i)));
            }
        }

        [Test]
        public void TestMethodParse()
        {
            Random rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var datetime = new DateTime(rnd.Next(1970, 2035), rnd.Next(1, 13), rnd.Next(1, 29), rnd.Next(0, 24), rnd.Next(0, 60), rnd.Next(0, 60), (DateTimeKind)rnd.Next(0, 3));
                var dts = datetime.ToString(StringExtensions.InteropDateTimeFormat);
                var u32 = (UnixTime32)datetime;
                var u32s = u32.ToString();
                var u32b = UnixTime32.Parse(u32s);

                Assert.AreEqual(datetime, (DateTime)u32);
                Assert.AreEqual(datetime, (DateTime)u32b);
                Assert.AreEqual(dts, u32s);
            }
        }

        [Test]
        public void TestMethodStruct()
        {
            Assert.AreEqual(false, new UnixTime32().Equals(null));
            Assert.AreEqual(new UnixTime32(), new UnixTime32());
            Assert.AreEqual(0, new UnixTime32().TimeStamp);
            Assert.AreEqual((UnixTime32)0, new UnixTime32());
            Assert.AreEqual(true, 0 == new UnixTime32().TimeStamp);
            Assert.AreEqual(new DateTime(1970, 1, 1), new UnixTime32().DateTime);

            unsafe
            {
                Assert.AreEqual(structSize, Marshal.SizeOf(typeof(UnixTime32)));
                Assert.AreEqual(structSize, sizeof(UnixTime32));
            }
            var ptr = Marshal.AllocHGlobal(100);
            Marshal.Copy(new byte[100], 0, ptr, 100);
            try
            {
                UnixTime32 now = UnixTime32.Now;
                Marshal.StructureToPtr(now, ptr, true);
                long value = (long)Marshal.ReadInt64(ptr);
                Assert.AreEqual(true, now.TimeStamp == value);
                var copy = (UnixTime32)Marshal.PtrToStructure(ptr, typeof(UnixTime32));
                Assert.AreEqual(now.TimeStamp, copy.TimeStamp);
                Assert.AreEqual(now, copy);
                Assert.AreEqual((DateTime)now, (DateTime)copy);
                Assert.AreEqual(now.DateTime, copy.DateTime);
                Assert.AreEqual(now.GetHashCode(), copy.TimeStamp.GetHashCode());
                Assert.AreEqual(now.GetHashCode(), copy.GetHashCode());
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        [Test]
        public void TestMethodSubtract()
        {
            Random rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var datetime = new DateTime(rnd.Next(1970, 2035), rnd.Next(1, 13), rnd.Next(1, 29), rnd.Next(0, 24), rnd.Next(0, 60), rnd.Next(0, 60), (DateTimeKind)rnd.Next(0, 3));
                UnixTime32 u32 = datetime;

                for (int n = 0; n < 100; n++)
                {
                    int seconds = (int)(rnd.NextDouble() * uint.MaxValue + int.MinValue);

                    while (true)
                    {
                        long test = -seconds;
                        test += u32.TimeStamp;
                        if (test > uint.MaxValue || test < 0)
                        {
                            //would overflow or underflow
                            seconds /= 3;
                            continue;
                        }
                        break;
                    }

                    var ts = TimeSpan.FromSeconds(seconds);
                    u32 -= ts;
                    datetime = datetime - ts;

                    Assert.AreEqual(datetime, (DateTime)u32);
                    Assert.AreEqual(true, u32 == datetime);
                    Assert.AreEqual(true, u32 == u32.TimeStamp);
                    Assert.AreEqual(false, u32 != datetime);
                    Assert.AreEqual(false, u32 != u32.TimeStamp);
                }
            }
        }

        #endregion Public Methods
    }
}
