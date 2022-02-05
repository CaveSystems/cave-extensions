using NUnit.Framework;
using Cave.Security;
using Cave;
using System;

namespace Test
{
    [TestFixture]
    class PBKDF2Tests
    {
        [Test]
        public void PBKDF2Test1()
        {
            Assert.AreEqual(8, PBKDF2.GuessComplexity(ASCII.GetBytes("ABCDEFGH")));
            Assert.AreEqual(9, PBKDF2.GuessComplexity(ASCII.GetBytes("ZYXABC123")));
            Assert.AreEqual(22, PBKDF2.GuessComplexity(ASCII.GetBytes("FAHDGCEB")));
            Assert.AreEqual(49, PBKDF2.GuessComplexity(ASCII.GetBytes("2ZY3AXBC1")));
            Assert.AreEqual(37, PBKDF2.GuessComplexity(ASCII.GetBytes("gqTU7x_kP!")));
        }
    }
}
