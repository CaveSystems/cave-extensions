using System;
using System.Linq;
using System.Reflection;
using Cave;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    class PropertyEnumeratorTests
    {
        class Intermediate
        {
            #region Properties

            public Item EmptyItem { get; }
            public long TestLong { get; } = Rnd.Next() * (long)Rnd.Next();

            #endregion
        }

        class Item
        {
            #region Properties

            public int TestInt { get; } = Rnd.Next();

            #endregion
        }

        class Root
        {
            #region Properties

            public Intermediate EmptyIntermediate { get; }

            public Intermediate Intermediate { get; } = new Intermediate();

            public Item TestItem { get; } = new Item();
            public object TestRandom { get; } = Rnd;

            #endregion
        }

        static readonly Random Rnd = new Random();

        void TestRoot(Root root)
        {
            try { root.GetPropertyValue<long>("EmptyIntermediate.TestLong"); }
            catch (Exception ex) { Assert.AreEqual(typeof(NullReferenceException), ex.GetType()); }

            try { root.GetPropertyValue<Item>("EmptyIntermediate.EmptyObject"); }
            catch (Exception ex) { Assert.AreEqual(typeof(NullReferenceException), ex.GetType()); }

            try { root.GetPropertyValue<Item>(".EmptyIntermediate.EmptyItem.TestInt"); }
            catch (Exception ex) { Assert.AreEqual(typeof(NullReferenceException), ex.GetType()); }

            Assert.AreEqual(root.EmptyIntermediate, root.GetPropertyValue("/EmptyIntermediate"));
            Assert.AreEqual(GetPropertyValueError.NullReference, root.TryGetPropertyValue("EmptyIntermediate.TestLong", out long testLong));
            Assert.AreEqual(root.EmptyIntermediate?.TestLong ?? default, testLong);
            Assert.AreEqual(GetPropertyValueError.NullReference, root.TryGetPropertyValue("EmptyIntermediate.EmptyItem", out var emptyItem));
            Assert.AreEqual(root.EmptyIntermediate?.EmptyItem ?? default, emptyItem);
            Assert.AreEqual(GetPropertyValueError.NullReference, root.TryGetPropertyValue("EmptyIntermediate.EmptyItem.TestInt", out int emptyItemTestInt));
            Assert.AreEqual(root.EmptyIntermediate?.EmptyItem?.TestInt ?? default, emptyItemTestInt);
            Assert.AreEqual(root.Intermediate, root.GetPropertyValue<Intermediate>("/Intermediate"));
            Assert.AreEqual(root.Intermediate.TestLong, root.GetPropertyValue("Intermediate.TestLong"));
            Assert.AreEqual((object)root.Intermediate.TestLong, root.GetPropertyValue<object>("Intermediate.TestLong"));
            Assert.AreEqual(root.Intermediate.EmptyItem, root.GetPropertyValue("Intermediate.EmptyItem"));
            Assert.AreEqual(GetPropertyValueError.NullReference, root.TryGetPropertyValue(".Intermediate.EmptyItem.TestInt", out int emptyItemTestInt2));
            Assert.AreEqual(root.Intermediate.EmptyItem?.TestInt ?? default, emptyItemTestInt2);
            Assert.AreEqual(GetPropertyValueError.NullReference, root.TryGetPropertyValue(".Intermediate.EmptyItem.TestInt", out var emptyItemTestIntObject));
            Assert.AreEqual(root.Intermediate.EmptyItem?.TestInt, emptyItemTestIntObject);
            Assert.AreEqual(root.TestRandom, root.GetPropertyValue("TestRandom"));
            Assert.AreEqual(root.TestItem, root.GetPropertyValue("TestItem"));
            Assert.AreEqual(root.TestItem.TestInt, root.GetPropertyValue("TestItem/TestInt"));
            Assert.AreEqual(root.EmptyIntermediate, root.GetPropertyValue<Intermediate>("/EmptyIntermediate"));
            Assert.AreEqual(GetPropertyValueError.NullReference, root.TryGetPropertyValue(".Intermediate.EmptyItem.TestLong", out long testLong2));
            Assert.AreEqual(root.EmptyIntermediate?.TestLong ?? 0L, testLong2);
            Assert.AreEqual(GetPropertyValueError.NullReference, root.TryGetPropertyValue("EmptyIntermediate.EmptyItem", out var emptyItem2));
            Assert.AreEqual(null, emptyItem2);
            Assert.AreEqual(GetPropertyValueError.NullReference, root.TryGetPropertyValue(".EmptyIntermediate.EmptyItem.TestInt", out int testInt3));
            Assert.AreEqual(0, testInt3);
            Assert.AreEqual(root.Intermediate, root.GetPropertyValue<Intermediate>("/Intermediate"));
            Assert.AreEqual(root.Intermediate.TestLong, root.GetPropertyValue<long>("Intermediate.TestLong"));
            Assert.AreEqual(root.Intermediate.EmptyItem, root.GetPropertyValue<Item>("Intermediate.EmptyItem"));
            Assert.AreEqual(root.TestRandom, root.GetPropertyValue<object>("TestRandom"));
            Assert.AreEqual(root.TestItem, root.GetPropertyValue<Item>("TestItem"));
            Assert.AreEqual(root.TestItem.TestInt, root.GetPropertyValue<int>("TestItem/TestInt"));
        }

        void TestSequence(IOrderedEnumerable<PropertyData> sequence, bool fullSequence)
        {
            if (fullSequence)
            {
                var sequence1 = new[]
                {
                    "/EmptyIntermediate",
                    "/EmptyIntermediate/EmptyItem",
                    "/EmptyIntermediate/EmptyItem/TestInt",
                    "/EmptyIntermediate/TestLong",
                    "/Intermediate",
                    "/Intermediate/EmptyItem",
                    "/Intermediate/EmptyItem/TestInt",
                    "/Intermediate/TestLong",
                    "/TestItem",
                    "/TestItem/TestInt",
                    "/TestRandom"
                };
                Assert.IsTrue(sequence.Select(p => p.FullPath).SequenceEqual(sequence1));
            }
            else
            {
                var sequence2 = new[]
                {
                    "/EmptyIntermediate",
                    "/Intermediate",
                    "/Intermediate/EmptyItem",
                    "/Intermediate/TestLong",
                    "/TestItem",
                    "/TestItem/TestInt",
                    "/TestRandom"
                };
                Assert.IsTrue(sequence.Select(p => p.FullPath).SequenceEqual(sequence2));
            }
        }

        [Test]
        public void GetProperties()
        {
            var root = new Root();
            var sequence = root.GetProperties(withValue: true).OrderBy(p => p.FullPath);
            TestSequence(sequence, false);
            TestRoot(root);
            foreach (var property in sequence)
            {
                Assert.AreEqual(property.Value, root.GetPropertyValue(property.FullPath));
            }
        }

        [Test]
        public void PropertyEnumerator()
        {
            var root = new Root();
            var enumerator = new PropertyEnumerator(typeof(Root), BindingFlags.Public | BindingFlags.Instance, true);
            var sequence = enumerator.OrderBy(p => p.FullPath);
            TestSequence(sequence, true);
            TestRoot(root);
        }

        [Test]
        public void PropertyValueEnumerator()
        {
            var root = new Root();
            var enumerator = new PropertyValueEnumerator(root, BindingFlags.Public | BindingFlags.Instance, true);
            var sequence = enumerator.OrderBy(p => p.FullPath);
            TestSequence(sequence, false);
            TestRoot(root);
            foreach (var property in sequence)
            {
                Assert.AreEqual(property.Value, root.GetPropertyValue(property.FullPath));
            }
        }
    }
}
