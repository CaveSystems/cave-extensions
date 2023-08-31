using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Test;

[TestFixture]
class PropertyInfoTest
{
    #region Private Classes

    class TestClass
    {
        #region Public Properties

        public int TestProperty { get; set; }

        #endregion Public Properties
    }

    #endregion Private Classes

    #region Public Methods

    [Test]
    public void GetSetMethodTest()
    {
        var propertyInfo = typeof(TestClass).GetProperties().Single(p => p.Name == "TestProperty");
        var getter = propertyInfo.GetGetMethod();
        var setter = propertyInfo.GetSetMethod();
        var test = new TestClass();
        for (int i = 0; i < 100000; i = i * 3 - 1)
        {
            test.TestProperty = 0;
            setter.Invoke(test, new object[] { i });
            Assert.AreEqual(i, test.TestProperty);
            var val = getter.Invoke(test, null);
            Assert.AreEqual(i, val);

            Assert.AreEqual(i, propertyInfo.GetValue(test));
            propertyInfo.SetValue(test, i + 1);
            Assert.AreEqual(i + 1, test.TestProperty);
            Assert.AreEqual(i + 1, propertyInfo.GetValue(test));
        }
    }

    #endregion Public Methods
}
