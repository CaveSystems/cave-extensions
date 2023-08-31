#if NET40_OR_GREATER || NETCOREAPP2_0_OR_GREATER
using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace Test;

public class RecordTest
{
    record Item
    {
        static readonly Random rnd = new(123);
        public int Value { get; init; } = rnd.Next();
    }

    #region Public Methods

    [Test]
    public void RecordLinkTest()
    {
        var i1 = new Item();
        var i2 = new Item();
        var i3 = new Item()
        {
            Value = i1.Value
        };
        Assert.AreEqual(i1, i3);
        Assert.IsTrue(object.Equals(i1, i3));
        Assert.IsFalse(object.Equals(i1, i2));
        Assert.IsFalse(object.Equals(i3, i2));
    }

    #endregion Public Methods
}
#endif
