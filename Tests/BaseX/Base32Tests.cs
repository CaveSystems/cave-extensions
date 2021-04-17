using System;
using Cave;
using NUnit.Framework;

namespace Test.BaseX
{
    [TestFixture]
    public class Base32Tests
    {
        [Test]
        public void Base32Test()
        {
            var rnd = new Random();
            foreach (var b in new[]
            {
                Base32.Default,
                Base32.NoPadding,
                Base32.Safe
            })
            {
                for (var i = 0; i < 1000; i++)
                {
                    var value = ((ulong)rnd.Next() * (ulong)rnd.Next() * (ulong)rnd.Next()) + (ulong)rnd.Next();
                    var buf = new byte[(int)(value % 64)];
                    rnd.NextBytes(buf);
                    CollectionAssert.AreEqual(buf, b.Decode(b.Encode(buf)));
                    Assert.AreEqual(value.ToString(), b.DecodeUtf8(b.Encode(value.ToString())));
                    var b1 = BitConverter.GetBytes(value);
                    var b2 = b.Decode(b.Encode(b1));
                    Assert.AreEqual(value, BitConverter.ToUInt64(b2, 0));
                }
            }
        }

        [Test]
        public void DecodeUtf8Test() => Assert.AreEqual("äöüÄÖÜ!", Base32.Default.DecodeUtf8("OEIC7DM3NJ1O9GSMOEE22==="));

        [Test]
        public void EncodeTest() => Assert.AreEqual("OEIC7DM3NJ1O9GSMOEE22===", Base32.Default.Encode("äöüÄÖÜ!"));

        [Test]
        public void ValueTest()
        {
            var i = Base32.Default.DecodeInt32(Base32.Default.Encode(int.MinValue));
            if (i != int.MinValue)
            {
                throw new Exception();
            }

            for (var n = 1; n < 0x10000000; n <<= 1)
            {
                i = Base32.Default.DecodeInt32(Base32.Default.Encode(n));
                Assert.AreEqual(n, i);
                i = Base32.Default.DecodeInt32(Base32.Default.Encode(-n));
                Assert.AreEqual(-n, i);
            }

            var l = Base32.Default.DecodeInt64(Base32.Default.Encode(long.MinValue));
            if (l != long.MinValue)
            {
                throw new Exception();
            }

            for (long n = 1; n < 0x1000000000000000L; n <<= 1)
            {
                l = Base32.Default.DecodeInt64(Base32.Default.Encode(n));
                Assert.AreEqual(n, l);
                l = Base32.Default.DecodeInt64(Base32.Default.Encode(-n));
                Assert.AreEqual(n, -l);
            }

            Assert.AreEqual(byte.MaxValue, Base32.Default.DecodeUInt8(Base32.Default.Encode(byte.MaxValue)));
            Assert.AreEqual(ushort.MaxValue, Base32.Default.DecodeUInt16(Base32.Default.Encode(ushort.MaxValue)));
            Assert.AreEqual(uint.MaxValue, Base32.Default.DecodeUInt32(Base32.Default.Encode(uint.MaxValue)));
            Assert.AreEqual(ulong.MaxValue, Base32.Default.DecodeUInt64(Base32.Default.Encode(ulong.MaxValue)));
        }
    }
}
