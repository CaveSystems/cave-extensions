using NUnit.Framework;

namespace Test.Backports;

[TestFixture]
public class TestIndexer
{
    [Test]
    public void TestRangeAndIndex()
    {
        var array = new[] { 0, 1, 2, 3, 4, 5 };
        Assert.AreEqual(3, array[3]);
        Assert.AreEqual(5, array[^1]);
        Assert.AreEqual(4, array[^2]);

        CollectionAssert.AreEqual(new[] { 2, 3 }, array[2..4]);
        CollectionAssert.AreEqual(new[] { 2 }, array[2..3]);
        CollectionAssert.AreEqual(new int[0], array[2..2]);

        CollectionAssert.AreEqual(new int[0], array[2..^4]);
        CollectionAssert.AreEqual(new[] { 2 }, array[2..^3]);
        CollectionAssert.AreEqual(new[] { 2, 3 }, array[2..^2]);

        CollectionAssert.AreEqual(new[] { 0, 1, 2 }, array[..^3]);
        CollectionAssert.AreEqual(new[] { 0, 1, 2 }, array[..3]);

        CollectionAssert.AreEqual(new[] { 2, 3, 4, 5 }, array[2..]);
        CollectionAssert.AreEqual(array, array[..]);
        CollectionAssert.AreEqual(array, array[..]);

        for (var i = 0; i < array.Length; i++)
        {
            Assert.AreEqual(i, array[^(array.Length - i)]);
            Assert.AreEqual(i, array[^(6 - i)]);
            Assert.AreEqual(5 - i, array[^(i + 1)]);
        }
    }
}
