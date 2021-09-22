#if NET20 || NET35

using System.Linq;
using NUnit.Framework;

namespace Test.Backports
{
    [TestFixture]
    public class BackportedExtensionsTests
    {
        static string Select(string longest, string next) => longest == null ? next : next.Length > longest.Length ? next : longest;

        class Pet
        {
#region Properties

            public int Age { get; set; }
            public string Name { get; set; }

#endregion
        }

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
            var longestName = Enumerable.Aggregate(fruits, "banana", Select, fruit => fruit.ToUpper());
#endif
            Assert.AreEqual(longestName, "PASSIONFRUIT");
        }

        [Test]
        public void CheckAllAny()
        {
            // Create an array of Pets.
            Pet[] pets1 =
            {
                new Pet
                {
                    Name = "Barley",
                    Age = 10
                },
                new Pet
                {
                    Name = "Boots",
                    Age = 4
                },
                new Pet
                {
                    Name = "Bonkers",
                    Age = 6
                }
            };
            Pet[] pets2 =
            {
                new Pet
                {
                    Name = "Aloise",
                    Age = 10
                },
                new Pet
                {
                    Name = "Charlie",
                    Age = 4
                },
                new Pet
                {
                    Name = "Whiskers",
                    Age = 6
                }
            };
            Assert.AreEqual(true, pets1.All(pet => pet.Name.StartsWith("B")));
            Assert.AreEqual(false, pets2.All(pet => pet.Name.StartsWith("B")));
            Assert.AreEqual(true, pets1.Any(pet => pet.Name.StartsWith("B")));
            Assert.AreEqual(false, pets2.Any(pet => pet.Name.StartsWith("B")));
            Assert.AreEqual(false, pets1.Any(pet => pet.Name.StartsWith("C")));
            Assert.AreEqual(true, pets2.Any(pet => pet.Name.StartsWith("C")));
        }

        [Test]
        public void CheckWhereAny()
        {
            Assert.True(typeof(BackportedExtensionsTests).GetMethods().Where(m => m.Name == nameof(CheckWhereAny)).Any());
            Assert.AreEqual(1, typeof(BackportedExtensionsTests).GetMethods().Where(m => m.Name == nameof(CheckWhereAny)).Count());
        }
    }
}

#endif
