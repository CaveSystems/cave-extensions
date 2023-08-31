using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Test.Backports;

[TestFixture]
public class TestHashSet
{
    [Test]
    public void TestAddInt()
    {
        var set1 = new HashSet<int>(new[] { 1, 2 });
        set1.Add(3);
        set1.Add(4);
        //add existing items
        set1.Add(1);
        set1.Add(2);
        Assert.IsTrue(set1.SequenceEqual(new[] { 1, 2, 3, 4 }));
    }

    [Test]
    public void TestAddString()
    {
        var set1 = new HashSet<string>(new[] { "1", "2" });
        set1.Add("3");
        set1.Add("4");
        //add existing items
        set1.Add("1");
        set1.Add("2");
        Assert.IsTrue(set1.SequenceEqual(new[] { "1", "2", "3", "4" }));
    }

    [Test]
    public void TestExcept()
    {
        var set1 = new HashSet<int>(new[] { 1, 2, 3 });
        var set2 = new HashSet<int>(new[] { 2, 3, 4 });
        var set3 = new HashSet<int>(set1);
        set3.ExceptWith(set2);
        Assert.IsTrue(set3.SetEquals(set1.Except(set2)));
        Assert.IsTrue(set3.SequenceEqual(new[] { 1 }));
    }

    [Test]
    public void TestIntersect()
    {
        var set1 = new HashSet<int>(new[] { 1, 2, 3 });
        var set2 = new HashSet<int>(new[] { 2, 3, 4 });
        var set3 = new HashSet<int>(set1);
        set3.IntersectWith(set2);
        Assert.IsTrue(set3.SetEquals(set1.Intersect(set2)));
        Assert.IsTrue(set3.SequenceEqual(new[] { 2, 3 }));
    }

    [Test]
    public void TestSuperSubProperOverlap()
    {
        var super = new HashSet<int>(new[] { 1, 2, 3, 4 });
        var sub = new HashSet<int>(new[] { 2, 3 });

        Assert.IsTrue(sub.IsSubsetOf(sub));
        Assert.IsTrue(sub.IsSubsetOf(super));
        Assert.IsFalse(super.IsSubsetOf(sub));
        Assert.IsTrue(super.IsSubsetOf(super));
        Assert.IsFalse(sub.IsSubsetOf(new[] { 0 }));

        Assert.IsTrue(sub.IsSupersetOf(sub));
        Assert.IsFalse(sub.IsSupersetOf(super));
        Assert.IsTrue(super.IsSupersetOf(sub));
        Assert.IsTrue(super.IsSupersetOf(super));
        Assert.IsFalse(sub.IsSupersetOf(new[] { 0 }));

        Assert.IsFalse(sub.IsProperSupersetOf(sub));
        Assert.IsFalse(sub.IsProperSupersetOf(super));
        Assert.IsTrue(super.IsProperSupersetOf(sub));
        Assert.IsFalse(super.IsProperSupersetOf(super));
        Assert.IsFalse(sub.IsProperSupersetOf(new[] { 0 }));

        Assert.IsFalse(sub.IsProperSubsetOf(sub));
        Assert.IsTrue(sub.IsProperSubsetOf(super));
        Assert.IsFalse(super.IsProperSubsetOf(sub));
        Assert.IsFalse(super.IsProperSubsetOf(super));
        Assert.IsFalse(sub.IsProperSubsetOf(new[] { 0 }));

        Assert.IsTrue(sub.Overlaps(super));
        Assert.IsTrue(super.Overlaps(sub));
        Assert.IsFalse(sub.Overlaps(new[] { 0 }));
    }

    [Test]
    public void TestSymetricExcept()
    {
        var set1 = new HashSet<int>(new[] { 1, 2, 3 });
        var set2 = new HashSet<int>(new[] { 2, 3, 4 });
        var set3 = new HashSet<int>(set1);
        set3.SymmetricExceptWith(set2);
        Assert.IsTrue(set3.SequenceEqual(new[] { 1, 4 }));
    }

    [Test]
    public void TestUnion()
    {
        var set1 = new HashSet<int>(new[] { 1, 2 });
        var set2 = new HashSet<int>(new[] { 2, 3 });
        var set3 = new HashSet<int>();
        set3.UnionWith(set1);
        set3.UnionWith(set2);
        Assert.IsTrue(set3.SetEquals(set1.Union(set2)));
        Assert.IsTrue(set3.SequenceEqual(new[] { 1, 2, 3 }));
    }
}
