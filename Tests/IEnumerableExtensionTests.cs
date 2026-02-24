using System.Linq;
using Cave;
using NUnit.Framework;

namespace Test;

[TestFixture]
public class IEnumerableExtensionTests
{
    private readonly string[] array =
    {
        "AB", "AD", "CX", "BA", "BC", "BD", "CD", "CA"
    };

    [Test]
    public void TakeFirstReturnsFirstElementOfEachGroup()
    {
        var result = array.TakeFirst(s => s[0]).ToList();

        CollectionAssert.AreEqual(new[] { "AB", "CX", "BA" }, result);
    }

    [Test]
    public void TakeLastReturnsLastElementOfEachGroup()
    {
        var result = array.TakeLast(s => s[0]).ToList();
        CollectionAssert.AreEqual(new[] { "AD", "CA", "BD" }, result);
    }

    [Test]
    public void TakeOneReturnsOneRandomElementPerGroupWithSeed()
    {
        var result = array.TakeOne(s => s[0], seed: 123).ToList();
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(1, result.Count(s => s.StartsWith("A")));
        Assert.AreEqual(1, result.Count(s => s.StartsWith("B")));
        Assert.AreEqual(1, result.Count(s => s.StartsWith("C")));
    }

    [Test]
    public void TakeOneReturnsDifferentResultsForDifferentSeeds()
    {
        var r1 = array.TakeOne(s => s[0], seed: 1).ToList();
        var r2 = array.TakeOne(s => s[0], seed: 2).ToList();
        Assert.AreNotEqual(r1, r2);
    }
}
