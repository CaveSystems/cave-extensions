﻿using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Xml;
using Cave;
using NUnit.Framework;

namespace Test;

[TestFixture]
class PropertyEnumeratorTests
{
    class Intermediate
    {
        #region Properties

        public Item EmptyItem { get; }

        public Item[] ItemArray { get; } = { new Item(), null, new Item() };

        public long TestLong { get; } = rnd.Next() * (long)rnd.Next();

        public SomeStruct TestStruct { get; set; }

        #endregion Properties
    }

    class Item
    {
        #region Properties

        public int TestInt { get; } = rnd.Next();

        #endregion Properties
    }

    class Root
    {
        #region Properties

        public Intermediate EmptyIntermediate { get; }

        public Intermediate Intermediate { get; } = new();

        public Item TestItem { get; } = new();

        public object TestRandom { get; } = rnd;

        #endregion Properties
    }

    struct SomeStruct
    {
        public int IntField;

        public int IntProperty { get => IntField; set => IntField = value; }
    }

    static readonly Random rnd = new();

    static void TestRoot(Root root)
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
        Assert.AreEqual(root.Intermediate.ItemArray[0], root.GetPropertyValue<Item>("/Intermediate/ItemArray[0]"));
        Assert.AreEqual(root.Intermediate.ItemArray[1], root.GetPropertyValue<Item>("/Intermediate/ItemArray[1]"));
        Assert.AreEqual(root.Intermediate.ItemArray[2], root.GetPropertyValue<Item>("/Intermediate/ItemArray[2]"));
        Assert.AreEqual(root.Intermediate.TestLong, root.GetPropertyValue("Intermediate.TestLong"));
        Assert.AreEqual(root.Intermediate.TestLong, root.GetPropertyValue<object>("Intermediate.TestLong"));
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

    static void TestFullSequenceNoSource(IOrderedEnumerable<PropertyData> sequence, Root root)
    {
        var sequence1 = new[]
        {
            "/EmptyIntermediate",
            "/EmptyIntermediate/EmptyItem",
            "/EmptyIntermediate/EmptyItem/TestInt",
            "/EmptyIntermediate/ItemArray",
            "/EmptyIntermediate/TestLong",
            "/EmptyIntermediate/TestStruct",
            "/EmptyIntermediate/TestStruct/IntProperty",
            "/Intermediate",
            "/Intermediate/EmptyItem",
            "/Intermediate/EmptyItem/TestInt",
            "/Intermediate/ItemArray",
            "/Intermediate/TestLong",
            "/Intermediate/TestStruct",
            "/Intermediate/TestStruct/IntProperty",
            "/TestItem",
            "/TestItem/TestInt",
            "/TestRandom"
        };
        var items = sequence.ToList();
        Assert.IsTrue(items.Select(p => p.FullPath).SequenceEqual(sequence1));
        Assert.IsTrue(items.All(i => i.PropertyInfo != null));
    }

    static void TestSequenceWithSource(IOrderedEnumerable<PropertyData> sequence)
    {
        var sequence2 = new[]
        {
            "/EmptyIntermediate",
            "/Intermediate",
            "/Intermediate/EmptyItem",
            "/Intermediate/ItemArray",
            "/Intermediate/ItemArray[0]/TestInt",
            "/Intermediate/ItemArray[2]/TestInt",
            "/Intermediate/TestLong",
            "/Intermediate/TestStruct",
            "/Intermediate/TestStruct/IntProperty",
            "/TestItem",
            "/TestItem/TestInt",
            "/TestRandom"
        };
        var items = sequence.ToList();
        Assert.IsTrue(items.Select(p => p.FullPath).SequenceEqual(sequence2));
        Assert.IsTrue(items.All(i => i.PropertyInfo != null));
        Assert.IsTrue(items.All(i => i.Source != null));
    }

    static void TestType(Type type)
    {
#if NETCOREAPP1_0_OR_GREATER && !NETCOREAPP2_0_OR_GREATER
        if (type.GetTypeInfo().IsAbstract)
        {
            return;
        }
        if (type.GetTypeInfo().ContainsGenericParameters)
        {
            return;
        }
#else
        if (type.IsAbstract)
        {
            return;
        }
        if (type.ContainsGenericParameters)
        {
            return;
        }
#endif
        var constructor = type.GetConstructor(Type.EmptyTypes);
        if (constructor == null)
        {
            return;
        }
        object obj;
        try
        {
            obj = Activator.CreateInstance(type);
        }
        catch
        {
            return;
        }
        var p1 = obj.GetProperties(flags: PropertyFlags.FilterUnset);
        p1.ForEach(TestPropertyWithValue);
        var p2 = obj.GetProperties(flags: PropertyFlags.None);
        p1.ForEach(TestPropertyWithoutValue);
        var p1Count = p1.Count();
        var p2Count = p2.Count();
        if (Program.Verbose)
        {
            Console.WriteLine($"{type} PropertyValues: {p1Count} TypeProperties: {p2Count}");
            foreach (var p in p1) { Console.WriteLine($"+ {p.PropertyInfo}"); }
            foreach (var p in p2) { Console.WriteLine($"+ {p.PropertyInfo}"); }
        }
    }

    static void TestPropertyWithValue(PropertyData property)
    {
        Assert.IsTrue(property.Source != null, "Property.Source unset. {0}", property.FullPath);
        if (property.PropertyInfo.CanRead)
        {
            Assert.IsTrue(property.CanGetValue, "Property.CanGetValue unset. {0}", property.FullPath);
        }
        TestPropertyWithoutValue(property);
    }

    static void TestPropertyWithoutValue(PropertyData property)
    {
        if (property.FullPath.Count(c => c == '/') > 1)
        {
            Assert.IsTrue(property.Parent != null, "Property.Parent unset. {0}", property.FullPath);
        }
    }

    static void TestTypes(Type[] types)
    {
        foreach (var type in types) { TestType(type); }
    }

    [Test]
    public void GetProperties()
    {
        var root = new Root();
        var sequence = root.GetProperties(flags: PropertyFlags.FilterUnset | PropertyFlags.Recursive).OrderBy(p => p.FullPath);
        TestSequenceWithSource(sequence);
        TestRoot(root);
        foreach (var property in sequence)
        {
            Assert.IsTrue(property.Source is not null);
            Assert.AreEqual(property.Value, root.GetPropertyValue(property.FullPath));
            var result = root.TryGetPropertyValue(property.FullPath, out var test);
            Assert.AreEqual(property.Value, test);
        }
    }

    [Test]
    public void PropertyEnumerator()
    {
        var root = new Root();
        var enumerator = new PropertyEnumerator(typeof(Root), root, BindingFlags.Public | BindingFlags.Instance, true);
        var sequence = enumerator.OrderBy(p => p.FullPath);
        TestFullSequenceNoSource(sequence, root);
        TestRoot(root);
    }

    [Test]
    public void PropertyValueEnumerator()
    {
        var root = new Root();
        var enumerator = new PropertyValueEnumerator(root, BindingFlags.Public | BindingFlags.Instance, true);
        var sequence = enumerator.OrderBy(p => p.FullPath);
        TestSequenceWithSource(sequence);
        TestRoot(root);
        foreach (var property in sequence)
        {
            Assert.AreEqual(property.Value, root.GetPropertyValue(property.FullPath));
        }
    }

#if NETCOREAPP1_0_OR_GREATER && !NETCOREAPP2_0_OR_GREATER
    [Test]
    public void TestTypes_Cave() => TestTypes(typeof(StringExtensions).GetTypeInfo().Assembly.GetExportedTypes());

    [Test]
    public void TestTypes_mscorlib() => TestTypes(typeof(int).GetTypeInfo().Assembly.GetExportedTypes());

    [Test]
    public void TestTypes_System() => TestTypes(typeof(Uri).GetTypeInfo().Assembly.GetExportedTypes());

    [Test]
    public void TestTypes_SystemXml() => TestTypes(typeof(XmlDocument).GetTypeInfo().Assembly.GetExportedTypes());
#else

    [Test]
    public void TestTypes_Cave() => TestTypes(typeof(StringExtensions).Assembly.GetExportedTypes());

    [Test]
    public void TestTypes_mscorlib() => TestTypes(typeof(int).Assembly.GetExportedTypes());

    [Test]
    public void TestTypes_System() => TestTypes(typeof(Uri).Assembly.GetExportedTypes());

    [Test]
    public void TestTypes_SystemData() => TestTypes(typeof(IDbConnection).Assembly.GetExportedTypes());

    [Test]
    public void TestTypes_SystemXml() => TestTypes(typeof(XmlDocument).Assembly.GetExportedTypes());

#endif
}
