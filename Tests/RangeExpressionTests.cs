using System.Linq;
using Cave;
using Cave.Collections;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class RangeExpressionTests
    {
        [Test]
        public void ParseTest()
        {
            {
                var test = RangeExpression.Parse("5-10/2", 0, 10);
                Assert.AreEqual("5,7,9", test.Join(","));
                Assert.AreEqual("5/2", test.ToString());
            }
            {
                var test = RangeExpression.Parse("1-9/2", 0, 10);
                Assert.AreEqual("1,3,5,7,9", test.Join(","));
                Assert.AreEqual("1-9/2", test.ToString());
            }
            {
                var test = RangeExpression.Parse("*/3", 0, 10);
                Assert.AreEqual("0,3,6,9", test.Join(","));
                Assert.AreEqual("*/3", test.ToString());
            }
            {
                var test = RangeExpression.Parse("*/3", -3, 12);
                Assert.AreEqual("-3,0,3,6,9,12", test.Join(","));
            }
            {
                var test = RangeExpression.Parse("1,2,3,7", 1, 7);
                Assert.AreEqual("1,2,3,7", test.Join(","));
            }
            {
                var test = RangeExpression.Parse("1,2,3,7", 2, 7);
                Assert.AreEqual("2,3,7", test.Join(","));
            }
            {
                var test = RangeExpression.Parse("1,2,3,7", 1, 6);
                Assert.AreEqual("1,2,3", test.Join(","));
            }
        }
    }
}
