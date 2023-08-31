using System;
using System.Collections;
using NUnit.Framework;

namespace Test.Backports;

[TestFixture]
class ArrayListTest
{
    [Test]
    public void ArrayListTest1()
    {
        var list = new ArrayList();
        list.Add(1);
        list.Add(2L);
        list.Add(Math.PI);
        var array = list.ToArray();
        Assert.AreEqual(3, array.Length);
        Assert.AreEqual(3, list.Count);
        for (int i = 0; i < 3; i++)
        {
            Assert.AreEqual(array[i], list[i]);
        }
        CollectionAssert.AreEqual(array, list);
    }
}
