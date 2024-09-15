using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Cave;
using Cave.Collections;
using NUnit.Framework;

namespace Test;

[TestFixture]
class IHashingFunctionTests
{
    #region Public Methods

    [Test]
    public void DefaultHashingFunctionTest()
    {
        var test1 = new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 6 } };
        var test2 = new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 6 } };

        var comp1 = DefaultComparer.Compare(test1[0], test2[1]);
        var comp2 = DefaultComparer.Compare(test2[0], test1[1]);
        Assert.AreEqual(comp1, comp2);

        Assert.IsTrue(DefaultComparer.Equals(test1, test2));
        Assert.IsTrue(DefaultComparer.ItemsEqual(test1, test2));

        var hash1 = DefaultHashingFunction.Combine(test1, test1[0], (string)null);
        var hash2 = DefaultHashingFunction.Combine(test2, test2[0], (string)null);

        Assert.AreEqual(hash1, hash2);
    }

    #endregion Public Methods
}
