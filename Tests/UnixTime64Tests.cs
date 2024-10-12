using Cave;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnixTime.Tests
{
    [TestFixture]
    public class TestUnixTime64
    {
        #region Private Fields

        const int structSize = 8;

        #endregion Private Fields

        #region Public Methods

        [Test]
        public void TestMethodAdd()
        {
            Random rnd = new Random();

            for (int i = 0; i < 1000; i++)
            {
                var datetime = new DateTime(rnd.Next(1970, 3000), rnd.Next(1, 13), rnd.Next(1, 29), rnd.Next(0, 24), rnd.Next(0, 60), rnd.Next(0, 60), (DateTimeKind)rnd.Next(0, 3));
                UnixTime64 ut = datetime;

                for (int n = 0; n < 10;)
                {
                    long seconds = (long)((rnd.NextDouble() * 2 - 1) * 1000 * 365 * TimeSpan.TicksPerDay / TimeSpan.TicksPerSecond);

                    try
                    {
                        var test = datetime + TimeSpan.FromSeconds(seconds);
                    }
                    catch (Exception ex)
                    {
                        ///OverflowException at TimeSpan.FromSeconds()
                        ///The added or subtracted value results in an un-representable DateTime.
                        continue;
                    }

                    var ts = TimeSpan.FromSeconds(seconds);
                    ut += ts;
                    datetime = datetime + ts;

                    n++;
                    Assert.AreEqual(datetime, (DateTime)ut);
                    Assert.AreEqual(true, ut == datetime);
                    Assert.AreEqual(true, ut == ut.TimeStamp);
                    Assert.AreEqual(false, ut != datetime);
                    Assert.AreEqual(false, ut != ut.TimeStamp);
                }
            }
        }

        [Test]
        public void TestMethodCompareTo()
        {
            Random rnd = new Random();
            List<DateTime> test1 = new List<DateTime>();
            List<UnixTime64> test2 = new List<UnixTime64>();
            for (int i = 0; i < 1000; i++)
            {
                var datetime = new DateTime(rnd.Next(1970, 2035), rnd.Next(1, 13), rnd.Next(1, 29), rnd.Next(0, 24), rnd.Next(0, 60), rnd.Next(0, 60), (DateTimeKind)rnd.Next(0, 3));
                test1.Add(datetime);
                test2.Add((UnixTime64)datetime);
            }

            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(test1[i], (DateTime)test2[i]);
                Assert.AreEqual((UnixTime64)test1[i], test2[i]);
                Assert.AreEqual(true, test2[i].Equals(test1[i]));
            }
            test1.Sort();
            test2.Sort();
            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(test1[i], (DateTime)test2[i]);
                Assert.AreEqual((UnixTime64)test1[i], test2[i]);
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
                Assert.AreEqual(datetime < last, (UnixTime64)datetime < last);
                Assert.AreEqual(datetime > last, (UnixTime64)datetime > last);
                Assert.AreEqual(datetime <= last, (UnixTime64)datetime <= last);
                Assert.AreEqual(datetime >= last, (UnixTime64)datetime >= last);
                last = datetime;
            }
            DateTime copy = last;
            Assert.AreEqual(copy < last, (UnixTime64)copy < last);
            Assert.AreEqual(copy > last, (UnixTime64)copy > last);
            Assert.AreEqual(copy <= last, (UnixTime64)copy <= last);
            Assert.AreEqual(copy >= last, (UnixTime64)copy >= last);
            Assert.AreEqual(true, ((UnixTime64)copy).GetHashCode() == ((UnixTime64)last).GetHashCode());
        }

        [Test]
        public void TestMethodNow()
        {
            TimeSpan diff = default(TimeSpan);
            for (int i = 0; i < 10000; i++)
            {
                var ut = UnixTime64.Now;
                var ututc = UnixTime64.UtcNow;
                diff += ut.DateTime.ToUniversalTime() - ututc.DateTime;
            }
            Assert.AreEqual(true, Math.Abs(diff.Ticks) < TimeSpan.TicksPerSecond);
        }

        [Test]
        public void TestMethodOverflow()
        {
            var datetime = DateTime.Now;
            UnixTime64 ut = datetime;
            try
            {
                ut += TimeSpan.MaxValue;
            }
            catch (Exception ex)
            {
                Assert.AreEqual(typeof(OverflowException), ex.GetType());
            }
            try
            {
                ut -= TimeSpan.MaxValue;
            }
            catch (Exception ex)
            {
                Assert.AreEqual(typeof(OverflowException), ex.GetType());
            }

            for (int i = -9; i < 10; i++)
            {
                DateTime check = new DateTime(1970, 1, 1) + TimeSpan.FromSeconds((uint.MaxValue + 1L) * i);
                Assert.AreEqual(check, UnixTime64.Convert(0, DateTimeKind.Local, TimeSpan.FromHours(i)));
                Assert.AreEqual(check, UnixTime64.Convert(0, DateTimeKind.Utc, TimeSpan.FromHours(i)));
                Assert.AreEqual(check, UnixTime64.Convert(0, DateTimeKind.Unspecified, TimeSpan.FromHours(i)));
            }

            {
                TimeSpan zone = new TimeSpan(2, 0, 0);
                DateTime check = new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(uint.MaxValue + 1L) - zone;
                Assert.AreEqual(check, UnixTime64.ConvertToUTC(0, zone));
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
                var ut = (UnixTime64)datetime;
                var uts = ut.ToString();
                var utb = UnixTime64.Parse(uts);

                Assert.AreEqual(datetime, (DateTime)ut);
                Assert.AreEqual(datetime, (DateTime)utb);
                Assert.AreEqual(dts, uts);
            }
        }

        [Test]
        public void TestMethodStruct()
        {
            Assert.AreEqual(false, new UnixTime64().Equals(null));
            Assert.AreEqual(new UnixTime64(), new UnixTime64());
            Assert.AreEqual(0, new UnixTime64().TimeStamp);
            Assert.AreEqual((UnixTime64)0, new UnixTime64());
            Assert.AreEqual(true, 0 == new UnixTime64().TimeStamp);
            Assert.AreEqual(new DateTime(1970, 1, 1), (DateTime)new UnixTime64());

            unsafe
            {
                Assert.AreEqual(structSize, Marshal.SizeOf(typeof(UnixTime64)));
                Assert.AreEqual(structSize, sizeof(UnixTime64));
            }
            var ptr = Marshal.AllocHGlobal(100);
            Marshal.Copy(new byte[100], 0, ptr, 100);
            try
            {
                UnixTime64 now = UnixTime64.Now;
                Marshal.StructureToPtr(now, ptr, true);
                long value = (long)Marshal.ReadInt64(ptr);
                Assert.AreEqual(true, now.TimeStamp == value);
                var copy = (UnixTime64)Marshal.PtrToStructure(ptr, typeof(UnixTime64));
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
                var datetime = new DateTime(rnd.Next(1970, 3000), rnd.Next(1, 13), rnd.Next(1, 29), rnd.Next(0, 24), rnd.Next(0, 60), rnd.Next(0, 60), (DateTimeKind)rnd.Next(0, 3));
                UnixTime64 ut = datetime;

                for (int n = 0; n < 10;)
                {
                    long seconds = (long)((rnd.NextDouble() * 2 - 1) * 1000 * 365 * TimeSpan.TicksPerDay / TimeSpan.TicksPerSecond);

                    try
                    {
                        var test = datetime - TimeSpan.FromSeconds(seconds);
                    }
                    catch (Exception ex)
                    {
                        ///OverflowException at TimeSpan.FromSeconds()
                        ///The added or subtracted value results in an un-representable DateTime.
                        continue;
                    }

                    var oldDT = datetime;
                    var oldUT = ut;

                    var ts = TimeSpan.FromSeconds(seconds);
                    ut -= ts;
                    datetime = datetime - ts;

                    n++;
                    Assert.AreEqual(datetime, (DateTime)ut);
                    Assert.AreEqual(true, ut == datetime);
                    Assert.AreEqual(true, ut == ut.TimeStamp);
                    Assert.AreEqual(false, ut != datetime);
                    Assert.AreEqual(false, ut != ut.TimeStamp);
                }
            }
        }

        #endregion Public Methods
    }
}
