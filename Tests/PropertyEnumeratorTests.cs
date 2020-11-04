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
        static Random rnd = new Random();
        class Item
        {
            public int TestInt { get; } = rnd.Next();
        }
        class Intermediate
        {
            public long TestLong { get; } = rnd.Next()* (long)rnd.Next();
            public Item EmptyItem { get; }
        }

        class Root
        {
            public Intermediate EmptyIntermediate { get; }

            public Intermediate Intermediate { get; } = new Intermediate();

            public object TestRandom { get; } = rnd;

            public Item TestItem { get; } = new Item();
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
                    "/TestRandom",
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

        void TestRoot(Root root)
        {
            try { root.GetPropertyValue<long>("EmptyIntermediate.TestLong"); }
            catch (Exception ex) { Assert.AreEqual(typeof(NullReferenceException), ex.GetType()); }
            try { root.GetPropertyValue<Item>("EmptyIntermediate.EmptyObject"); }
            catch (Exception ex) { Assert.AreEqual(typeof(NullReferenceException), ex.GetType()); }
            try { root.GetPropertyValue<Item>(".EmptyIntermediate.EmptyItem.TestInt"); }
            catch (Exception ex) { Assert.AreEqual(typeof(NullReferenceException), ex.GetType()); }

            Assert.AreEqual(root.EmptyIntermediate, root.GetPropertyValue("/EmptyIntermediate"));
            Assert.AreEqual(root.EmptyIntermediate?.TestLong, root.GetPropertyValue("EmptyIntermediate.TestLong", noException: true));
            Assert.AreEqual(root.EmptyIntermediate?.EmptyItem, root.GetPropertyValue("EmptyIntermediate.EmptyItem", noException: true));
            Assert.AreEqual(root.EmptyIntermediate?.EmptyItem?.TestInt, root.GetPropertyValue(".EmptyIntermediate.EmptyItem.TestInt", noException: true));
            Assert.AreEqual(root.Intermediate, root.GetPropertyValue<Intermediate>("/Intermediate"));
            Assert.AreEqual(root.Intermediate.TestLong, root.GetPropertyValue("Intermediate.TestLong"));
            Assert.AreEqual(root.Intermediate.EmptyItem, root.GetPropertyValue("Intermediate.EmptyItem"));
            Assert.AreEqual(root.Intermediate.EmptyItem?.TestInt, root.GetPropertyValue(".Intermediate.EmptyItem.TestInt", noException: true));
            Assert.AreEqual(root.TestRandom, root.GetPropertyValue("TestRandom"));
            Assert.AreEqual(root.TestItem, root.GetPropertyValue("TestItem"));
            Assert.AreEqual(root.TestItem.TestInt, root.GetPropertyValue("TestItem/TestInt"));

            Assert.AreEqual(root.EmptyIntermediate, root.GetPropertyValue<Intermediate>("/EmptyIntermediate"));
            Assert.AreEqual(root.EmptyIntermediate?.TestLong ?? 0L, root.GetPropertyValue<long>("EmptyIntermediate.TestLong", noException: true));
            Assert.AreEqual(root.EmptyIntermediate?.EmptyItem, root.GetPropertyValue<Item>("EmptyIntermediate.EmptyItem", noException: true));
            Assert.AreEqual(root.EmptyIntermediate?.EmptyItem?.TestInt ?? 0, root.GetPropertyValue<int>(".EmptyIntermediate.EmptyItem.TestInt", noException: true));
            Assert.AreEqual(root.Intermediate, root.GetPropertyValue<Intermediate>("/Intermediate"));
            Assert.AreEqual(root.Intermediate.TestLong, root.GetPropertyValue<long>("Intermediate.TestLong"));
            Assert.AreEqual(root.Intermediate.EmptyItem, root.GetPropertyValue<Item>("Intermediate.EmptyItem"));
            Assert.AreEqual(root.Intermediate.EmptyItem?.TestInt, root.GetPropertyValue<Item>(".Intermediate.EmptyItem.TestInt", noException: true));
            Assert.AreEqual(root.TestRandom, root.GetPropertyValue<object>("TestRandom"));
            Assert.AreEqual(root.TestItem, root.GetPropertyValue<Item>("TestItem"));
            Assert.AreEqual(root.TestItem.TestInt, root.GetPropertyValue<int>("TestItem/TestInt"));
        }
    }
}
