using Cave.Collections;
using NUnit.Framework;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Collections
{
    [TestFixture]
    public class DefaultComparerTests
    {
        [Test]
        public void EqualsArray()
        {
            var now = DateTime.Now;
            var array1 = new object[] { "string", 123, 123.5d, 45.67m, now, };
            var array2 = new object[] { "string", 123, 123.5d, 45.67m, now, };
            var array3 = new object[] { "string", 444, 123.5d, 45.67m, now, };

            Assert.AreEqual(true, DefaultComparer.Equals(array1, array1));
            Assert.AreEqual(true, DefaultComparer.Equals(array1, array2));
            Assert.AreEqual(false, DefaultComparer.Equals(array1, array3));
        }

        [Test]
        public void EqualsStruct()
        {
            var s1 = InteropTestStruct.Create(1);
            var s2 = InteropTestStruct.Create(1);
            var s3 = InteropTestStruct.Create(3);
            Assert.AreEqual(true, DefaultComparer.Equals(s1, s1));
            Assert.AreEqual(true, DefaultComparer.Equals(s1, s2));
            Assert.AreEqual(false, DefaultComparer.Equals(s1, s3));
            object o1 = s1;
            object o2 = s2;
            object o3 = s3;
            Assert.AreEqual(true, DefaultComparer.Equals(o1, o1));
            Assert.AreEqual(true, DefaultComparer.Equals(o2, o1));
            Assert.AreEqual(false, DefaultComparer.Equals(o3, o1));
        }

        [Test]
        public void TestCultureEqual()
        {
            var savedCurrentCulture = CultureInfo.CurrentCulture;
            var savedCurrentUICulture = CultureInfo.CurrentUICulture;
            var rnd = new Random();
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (culture.IsNeutralCulture) continue;
                Console.WriteLine(culture);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                for (var i = 0; i < 1000; i++)
                {
                    var dt1 = new DateTime(rnd.Next() + DateTime.Now.Ticks, DateTimeKind.Unspecified);
                    var dt2 = new DateTime(dt1.Ticks, DateTimeKind.Local);

                    Assert.AreEqual(true, DefaultComparer.Equals(dt1, dt1));
                    Assert.AreEqual(true, DefaultComparer.Equals(dt1, dt2));
                    Assert.AreEqual(true, DefaultComparer.Equals(dt2, dt1));
                    Assert.AreEqual(true, DefaultComparer.Equals(dt1, dt2.ToUniversalTime()));
                    Assert.AreEqual(true, DefaultComparer.Equals(dt2.ToUniversalTime(), dt1));
                    Assert.AreEqual(true, DefaultComparer.Equals(dt1, dt2.ToLocalTime()));
                    Assert.AreEqual(true, DefaultComparer.Equals(dt2.ToLocalTime(), dt1));
                    Assert.AreEqual(true, DefaultComparer.Equals(dt2.ToLocalTime(), dt2.ToUniversalTime()));
                    Assert.AreEqual(true, DefaultComparer.Equals(dt2.ToUniversalTime(), dt2.ToLocalTime()));
                }
            }
            Thread.CurrentThread.CurrentCulture = savedCurrentCulture;
            Thread.CurrentThread.CurrentUICulture = savedCurrentUICulture;
        }
    }
}