using System.Reflection;
using Cave;
using NUnit.Framework;

namespace Test;

[TestFixture]
class ObjectExtensionTests
{
    class TestItem
    {
        public string StringProperty { get; set; } = "1234";
        public string[] StringArray { get; set; } = new[] { "1", "2", "3" };
    }

    class TestRoot
    {
        public TestItem SubItem { get; } = new();
    }

    [Test]
    public void TestProperties()
    {
        var test = new TestRoot();
        Assert.AreEqual("1234", test.GetPropertyValue("SubItem.StringProperty", BindingFlags.Public | BindingFlags.Instance));
        Assert.AreEqual("1234", test.GetPropertyValue("/SubItem/StringProperty", BindingFlags.Public | BindingFlags.Instance));
        Assert.AreEqual('3', test.GetPropertyValue("/SubItem/StringProperty[2]", BindingFlags.Public | BindingFlags.Instance));
        Assert.AreEqual('2', test.GetPropertyValue("/SubItem/StringProperty/[1]", BindingFlags.Public | BindingFlags.Instance));

        Assert.AreEqual(3, test.GetPropertyValue("SubItem.StringArray.Length", BindingFlags.Public | BindingFlags.Instance));
        Assert.AreEqual("1", test.GetPropertyValue("SubItem.StringArray[0]", BindingFlags.Public | BindingFlags.Instance));
        Assert.AreEqual("1", test.GetPropertyValue("SubItem/StringArray/[0]", BindingFlags.Public | BindingFlags.Instance));
        Assert.AreEqual(1, test.GetPropertyValue("SubItem.StringArray[0].Length", BindingFlags.Public | BindingFlags.Instance));
        Assert.AreEqual("3", test.GetPropertyValue("SubItem.StringArray[2][0]", BindingFlags.Public | BindingFlags.Instance));
    }
}
