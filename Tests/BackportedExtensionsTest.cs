#if NET20 || NET35

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class BackportedExtensionsTest
    {
        static string Select(string longest, string next)
        {
            if (longest == null) return next;
            if (next.Length > longest.Length)
            {
                return next;
            }
            return longest;
        }

        [Test]
        public void CheckAggregate()
        {
            string[] fruits = { "apple", "mango", "orange", "passionfruit", "grape" };
#if NET20
            string longestName = BackportedExtensions.Aggregate(fruits, "banana", Select, fruit => fruit.ToUpper());
#else
            string longestName = Enumerable.Aggregate(fruits, "banana", Select, fruit => fruit.ToUpper());
#endif
            Assert.AreEqual(longestName, "PASSIONFRUIT");
        }

        class Pet
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public void CheckAllAny()
        {
            // Create an array of Pets.
            Pet[] pets1 =
            {
                new Pet { Name="Barley", Age=10 },
                new Pet { Name="Boots", Age=4 },
                new Pet { Name="Bonkers", Age=6 }
            };
            Pet[] pets2 =
            {
                new Pet { Name="Aloise", Age=10 },
                new Pet { Name="Charlie", Age=4 },
                new Pet { Name="Whiskers", Age=6 }
            };

            Assert.AreEqual(true, pets1.All(pet => pet.Name.StartsWith("B")));
            Assert.AreEqual(false, pets2.All(pet => pet.Name.StartsWith("B")));

            Assert.AreEqual(true, pets1.Any(pet => pet.Name.StartsWith("B")));
            Assert.AreEqual(false, pets2.Any(pet => pet.Name.StartsWith("B")));

            Assert.AreEqual(false, pets1.Any(pet => pet.Name.StartsWith("C")));
            Assert.AreEqual(true, pets2.Any(pet => pet.Name.StartsWith("C")));
        }
    }
}

#endif