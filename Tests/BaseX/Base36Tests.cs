using System;
using System.Linq;
using System.Numerics;
using Cave;
using Cave.Collections;
using NUnit.Framework;

namespace Test.BaseX;

[TestFixture]
public class Base36Tests
{
    #region Public Methods

    [Test]
    public void Base36Test()
    {
        Assert.AreEqual("RS~", BaseDynamic.Base36.Encode(1000).ToUpper());
        Assert.AreEqual("RS", BaseDynamic.Base36NoPadding.Encode(1000).ToUpper());
        Assert.AreEqual(1000, BaseDynamic.Base36.DecodeInt32("RS~"));
        Assert.AreEqual(1000, BaseDynamic.Base36NoPadding.DecodeInt32("RS"));

        {
            var original = new byte[1000];
            for (int i = 0; i < original.Length; i++) original[i] = (byte)(i + 1);
            var encoded = BaseDynamic.Base36.Encode(original);
            var decoded = BaseDynamic.Base36.Decode(encoded);
            CollectionAssert.AreEqual(original, decoded);
        }

        foreach (var b in new[] { BaseDynamic.Base36, BaseDynamic.Base36NoPadding })
        {
            {
                var encoded = b.Encode(ASCII.Strings.Printable);
                var decoded = b.DecodeUtf8(encoded);
                Assert.AreEqual(ASCII.Strings.Printable, decoded);
            }
            {
                var encoded = b.Encode(ASCII.Bytes.Printable.ToArray());
                var decoded = b.Decode(encoded);
                CollectionAssert.AreEqual(ASCII.Bytes.Printable, decoded);
            }
        }
    }

    [Test]
    public void Base36TestRandom()
    {
        var rnd = new Random();
        foreach (var b in new[] { BaseDynamic.Base36, BaseDynamic.Base36NoPadding })
        {
            for (var i = 0; i < 1000; i++)
            {
                var value = ((ulong)rnd.Next() * (ulong)rnd.Next() * (ulong)rnd.Next()) + (ulong)rnd.Next();
                var buf = new byte[(int)(value % 64)];
                rnd.NextBytes(buf);

                var encoded = b.Encode(buf);
                var decoded = b.Decode(encoded);
                var reencoded = b.Encode(decoded);
                Assert.AreEqual(encoded, reencoded);
                CollectionAssert.AreEqual(buf, decoded);

                Assert.AreEqual(value.ToString(), b.DecodeUtf8(b.Encode(value.ToString())));

                var b1 = BitConverter.GetBytes(value);
                var b2 = b.Decode(b.Encode(b1));
                Assert.AreEqual(value, BitConverter.ToUInt64(b2, 0));
            }
        }
    }

    [Test]
    public void BigIntTest()
    {
#if !NET20 && !NET35
        BigInteger bigValue = BigInteger.Parse("34766967346521604105876783743698741175");
        Assert.AreEqual(bigValue, BaseDynamic.Base36.DecodeValue("1jqu2wy81psrshxd2dp313x6f~"));
        Assert.AreEqual("1jqu2wy81psrshxd2dp313x6f~", BaseDynamic.Base36.EncodeValue(bigValue).ToLower());
#endif
    }

    #endregion Public Methods
}
