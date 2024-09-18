using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cave.Collections;
using NUnit.Framework;

namespace Test.Comparer;

[TestFixture]
public class DefaultComparerTests
{
    #region Public Methods

    [Test]
    public void EqualsArray()
    {
        var now = DateTime.Now;
        var array1 = new object[]
        {
            "string",
            123,
            123.5d,
            45.67m,
            now
        };
        var array2 = new object[]
        {
            "string",
            123,
            123.5d,
            45.67m,
            now
        };
        var array3 = new object[]
        {
            "string",
            444,
            123.5d,
            45.67m,
            now
        };
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
    public void IntegerArrayTest()
    {
        var a2 = new int[] { 1, 2 };
        var a3 = new int[] { 1, 2, 3 };
        var b2 = new int[] { 1, 2 };
        Assert.IsTrue(DefaultComparer.Compare(a2, a3) < 0);
        Assert.IsTrue(DefaultComparer.Compare(a3, a2) > 0);
        Assert.IsTrue(DefaultComparer.Compare(a2, a2) == 0);
        Assert.IsTrue(DefaultComparer.Compare(a2, b2) == 0);
        Assert.IsTrue(DefaultComparer.Equals(a2, a2));
        Assert.IsTrue(DefaultComparer.Equals(a2, b2));
        Assert.IsFalse(DefaultComparer.Equals(a2, a3));
    }

    [Test]
    public void TestCultureEqual()
    {
#if !(NETCOREAPP1_0_OR_GREATER && !NETCOREAPP2_0_OR_GREATER)
        ThreadPool.SetMaxThreads(1000, 1000);
        ThreadPool.SetMinThreads(100, 100);

        var savedCurrentCulture = CultureInfo.CurrentCulture;
        var savedCurrentUICulture = CultureInfo.CurrentUICulture;
        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        Parallel.ForEach(cultures, culture =>
        {
            var rnd = new Random(1337);
            if (culture.IsNeutralCulture)
            {
                return;
            }

            if (Program.Verbose)
            {
                Console.WriteLine(culture);
            }
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            for (var i = 0; i < 100; i++)
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
        });

        Thread.CurrentThread.CurrentCulture = savedCurrentCulture;
        Thread.CurrentThread.CurrentUICulture = savedCurrentUICulture;
#endif
    }

    #endregion Public Methods
}
