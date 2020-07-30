using Cave.Collections;
using NUnit.Framework;
using System;

namespace Test.Collections
{
    [TestFixture]
    public class DefaultComparerTests
    {
        [Test]
        public void Equals()
        {
            DateTime now = DateTime.Now;
            object[] array1 = new object[] { "string", 123, 123.5d, 45.67m, now, };
            object[] array2 = new object[] { "string", 123, 123.5d, 45.67m, now, };
            object[] array3 = new object[] { "string", 444, 123.5d, 45.67m, now, };

            Assert.AreEqual(true, DefaultComparer.Equals(array1, array1));
            Assert.AreEqual(true, DefaultComparer.Equals(array1, array2));
            Assert.AreEqual(false, DefaultComparer.Equals(array1, array3));
        }

        [Test]
        public void Equals1()
        {
            InteropTestStruct s1 = InteropTestStruct.Create(1);
            InteropTestStruct s2 = InteropTestStruct.Create(1);
            InteropTestStruct s3 = InteropTestStruct.Create(3);
            Assert.AreEqual(true, DefaultComparer.Equals(s1, s1));
            Assert.AreEqual(true, DefaultComparer.Equals(s1, s2));
            Assert.AreEqual(false, DefaultComparer.Equals(s1, s3));
            object o1 = s1;
            object o2 = s2;
            object o3 = s3;
            Assert.AreEqual(true, DefaultComparer.Equals(o1, o1));
            Assert.AreEqual(true, DefaultComparer.Equals(o2, o1));
            Assert.AreEqual(false, DefaultComparer.Equals(o3, o1));
        }
    }
}