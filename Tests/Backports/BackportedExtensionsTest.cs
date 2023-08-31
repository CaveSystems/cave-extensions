using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace Test.Backports;

[TestFixture]
public class BackportedExtensionsTests
{
    static string Select(string longest, string next) => longest == null ? next : next.Length > longest.Length ? next : longest;

    class Pet
    {
        #region Overrides

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Pet other && (other.Name == Name) && (other.Age == Age);

        #endregion

        #region Properties

        public int Age { get; init; }

        public string Name { get; init; }

        #endregion
    }

    Pet Barley => new() { Name = "Barley", Age = 10 };

    Pet Boots => new() { Name = "Boots", Age = 4 };

    Pet Bonkers => new() { Name = "Bonkers", Age = 6 };

    Pet Aloise => new() { Name = "Aloise", Age = 10 };

    Pet Charlie => new() { Name = "Charlie", Age = 4 };

    Pet Whiskers => new() { Name = "Whiskers", Age = 6 };

    [Test]
    public void CheckAggregate()
    {
        string[] fruits =
        {
            "apple",
            "mango",
            "orange",
            "passionfruit",
            "grape"
        };
#if NET20
        var longestName = fruits.Aggregate("banana", Select, fruit => fruit.ToUpper());
#else
        var longestName = fruits.Aggregate("banana", Select, fruit => fruit.ToUpper());
#endif
        Assert.AreEqual(longestName, "PASSIONFRUIT");
    }

    [Test]
    public void CheckAllAny()
    {
        // Create an array of Pets.
        Pet[] pets1 = { Barley, Boots, Bonkers };
        Pet[] pets2 = { Aloise, Charlie, Whiskers };
        Assert.AreEqual(true, pets1.All(pet => pet.Name.StartsWith("B")));
        Assert.AreEqual(false, pets2.All(pet => pet.Name.StartsWith("B")));
        Assert.AreEqual(true, pets1.Any(pet => pet.Name.StartsWith("B")));
        Assert.AreEqual(false, pets2.Any(pet => pet.Name.StartsWith("B")));
        Assert.AreEqual(false, pets1.Any(pet => pet.Name.StartsWith("C")));
        Assert.AreEqual(true, pets2.Any(pet => pet.Name.StartsWith("C")));
    }

#if !NETCOREAPP || NETCOREAPP3_0_OR_GREATER
    [Test]
    public void CheckWhereAny()
    {
        Assert.IsTrue(typeof(BackportedExtensionsTests).GetMethods().Where(m => m.Name == nameof(CheckWhereAny)).Any());
        Assert.AreEqual(1, typeof(BackportedExtensionsTests).GetMethods().Where(m => m.Name == nameof(CheckWhereAny)).Count());
    }
#endif

    [Test]
    public void TestGrouping()
    {
        Pet[] pets = { Barley, Boots, Bonkers, Aloise, Charlie, Whiskers };
        var group = pets.OrderBy(p => p.Age).GroupBy(p => p.Age).ToList();
        Assert.AreEqual(3, group.Count());
        Assert.AreEqual(true, group.Any());
        Assert.AreEqual(4, group.First().Key);
        Assert.AreEqual(6, group.Skip(1).First().Key);
        Assert.AreEqual(10, group.Skip(2).First().Key);
        Assert.AreEqual(10, group.Last().Key);
        CollectionAssert.AreEqual(group.Single(g => g.Key == 6), new[] { Bonkers, Whiskers });
        CollectionAssert.AreEqual(group.SingleOrDefault(g => g.Key == 7), null);
        Assert.IsTrue(group.Single(g => g.Key == 6).SequenceEqual(new[] { Bonkers, Whiskers }));
    }

    [Test]
    public void TestLookup()
    {
        Pet[] pets = { Barley, Boots, Bonkers, Aloise, Charlie, Whiskers };
        var lookup = pets.OrderBy(p => p.Age).ToLookup(p => p.Age, p => p);
        Assert.IsTrue(lookup.Contains(6));
        Assert.IsFalse(lookup.Contains(7));
        Assert.AreEqual(3, lookup.Count);
        Assert.AreEqual(true, lookup.Any());
        Assert.AreEqual(4, lookup.First().Key);
        Assert.AreEqual(6, lookup.Skip(1).First().Key);
        Assert.AreEqual(10, lookup.Skip(2).First().Key);
        Assert.AreEqual(10, lookup.Last().Key);
        CollectionAssert.AreEqual(lookup.Single(g => g.Key == 6), new[] { Bonkers, Whiskers });
        CollectionAssert.AreEqual(lookup.SingleOrDefault(g => g.Key == 7), null);
        Assert.IsTrue(lookup.Single(g => g.Key == 6).SequenceEqual(new[] { Bonkers, Whiskers }));
    }

    [Test]
    public void TestLookupDefaultGroup()
    {
        Pet[] pets = { new() { Age = 7 }, Barley, Boots, Bonkers, Aloise, Charlie, Whiskers };
        if (pets.ToLookup(p => p.Age) is Lookup<int, Pet> typed)
        {
            var listOfPets = typed.ApplyResultSelector((key, items) => items.Select(p => p.Name)).SelectMany(i => i);
            CollectionAssert.AreEqual(listOfPets.OrderByDescending(p => p), pets.Select(p => p.Name).OrderByDescending(p => p));
        }
        else
        {
            Assert.Fail();
        }

        var defaultGroup = pets.ToLookup(p => p.Name);
        Assert.AreEqual(7, defaultGroup.Count);
        Assert.AreEqual(null, defaultGroup.First().Key);

        IEnumerable enumerable = defaultGroup;
        Assert.IsTrue(enumerable.Cast<IGrouping<string, Pet>>().All(i => i.All(p => p.Age > 0)));
    }
}
