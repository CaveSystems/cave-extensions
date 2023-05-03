using System;
using Cave;
using Cave.Security;
using NUnit.Framework;

namespace Test;

[TestFixture]
class PBKDF2Tests
{
    [Test]
    [Obsolete]
    public void PBKDF2Test1()
    {
        Assert.AreEqual(8, PBKDF2.GuessComplexity(ASCII.GetBytes("ABCDEFGH")));
        Assert.AreEqual(9, PBKDF2.GuessComplexity(ASCII.GetBytes("ZYXABC123")));
        Assert.AreEqual(22, PBKDF2.GuessComplexity(ASCII.GetBytes("FAHDGCEB")));
        Assert.AreEqual(49, PBKDF2.GuessComplexity(ASCII.GetBytes("2ZY3AXBC1")));
        Assert.AreEqual(37, PBKDF2.GuessComplexity(ASCII.GetBytes("gqTU7x_kP!")));
    }
}
