using NUnit.Framework;

using System;
using Cave;
using System.Linq;
using System.IO;

namespace Test;

[TestFixture]
public class AsciiTest
{
    #region Public Methods

    [Test]
    public void TestAnsi()
    {
        Assert.Throws(typeof(InvalidDataException), () => ASCII.GetBytes("AOUÄÖÜ"));
    }

    [Test]
    public void TestGetBytes()
    {
        var seq1 = new byte[] { 97, 98, 99, 100, 101, 102, 103, 104, 0, 105, 106, 107 };
        var seq2 = new byte[] { 97, 98, 99, 100, 101, 102, 103, 104 };
        var bytes1 = ASCII.GetBytes("abcdefgh\0ijk", false);
        Assert.IsTrue(seq1.SequenceEqual(bytes1));
        var bytes2 = ASCII.GetBytes("abcdefgh\0ijk", true);
        Assert.IsTrue(seq2.SequenceEqual(bytes2));
    }

    [Test]
    public void TestGetString()
    {
        var bytes = ASCII.GetBytes("abcdefgh\0ijk");
        var str = ASCII.GetString(bytes);
        Assert.AreEqual("abcdefgh\0ijk", str);
        Assert.AreEqual("abcdefgh", ASCII.GetString(bytes, true));
        Assert.AreEqual("abcdefgh\0ijk", ASCII.GetString(bytes, false));
        Assert.AreEqual("bcdefgh", ASCII.GetString(bytes, 1, bytes.Length - 1, true));
        Assert.AreEqual("bcdefgh\0ijk", ASCII.GetString(bytes, 1, bytes.Length - 1, false));
    }

    [Test]
    public void TestPrintable()
    {
        var data = ASCII.GetBytes(ASCII.Strings.Printable);
        Assert.IsTrue(data.SequenceEqual(ASCII.Bytes.Printable));
    }

    #endregion Public Methods
}
