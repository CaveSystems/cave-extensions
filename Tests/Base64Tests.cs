using Cave;
using NUnit.Framework;
using System;

namespace Test.BaseX
{
    [TestFixture]
    public class Base64Tests
    {
        [Test]
        public void Base64Test()
        {
            var rnd = new Random();
            foreach (var b in new Base64[] { Base64.Default, Base64.NoPadding, Base64.UrlChars })
            {
                for (var i = 0; i < 1000; i++)
                {
                    var value = ((ulong)rnd.Next() * (ulong)rnd.Next() * (ulong)rnd.Next()) + (ulong)rnd.Next();
                    var buf = new byte[(int)(value % 64)];
                    rnd.NextBytes(buf);

                    CollectionAssert.AreEqual(buf, b.Decode(b.Encode(buf)));
                    Assert.AreEqual(value.ToString(), b.DecodeUtf8(b.Encode(value.ToString())));
                    Assert.AreEqual(value, BitConverter.ToUInt64(b.Decode(b.Encode(BitConverter.GetBytes(value))), 0));
                }
            }
        }

        [Test]
        public void DecodeUtf8Test() => Assert.AreEqual("äöüÄÖÜ!", Base64.Default.DecodeUtf8("w6TDtsO8w4TDlsOcIQ=="));

        [Test]
        public void EncodeTest() => Assert.AreEqual("w6TDtsO8w4TDlsOcIQ==", Base64.Default.Encode("äöüÄÖÜ!"));

        [Test]
        public void ValueTest()
        {
            var i = Base64.Default.DecodeInt32(Base64.Default.Encode(int.MinValue));
            if (i != int.MinValue) throw new Exception();
            for (var n = 1; n < 0x10000000; n <<= 1)
            {
                i = Base64.Default.DecodeInt32(Base64.Default.Encode(n));
                Assert.AreEqual(n, i);
                i = Base64.Default.DecodeInt32(Base64.Default.Encode(-n));
                Assert.AreEqual(-n, i);
            }

            var l = Base64.Default.DecodeInt64(Base64.Default.Encode(long.MinValue));
            if (l != long.MinValue) throw new Exception();
            for (long n = 1; n < 0x1000000000000000L; n <<= 1)
            {
                l = Base64.Default.DecodeInt64(Base64.Default.Encode(n));
                Assert.AreEqual(n, l);
                l = Base64.Default.DecodeInt64(Base64.Default.Encode(-n));
                Assert.AreEqual(n, -l);
            }

            Assert.AreEqual(byte.MaxValue, Base64.Default.DecodeUInt8(Base64.Default.Encode(byte.MaxValue)));
            Assert.AreEqual(ushort.MaxValue, Base64.Default.DecodeUInt16(Base64.Default.Encode(ushort.MaxValue)));
            Assert.AreEqual(uint.MaxValue, Base64.Default.DecodeUInt32(Base64.Default.Encode(uint.MaxValue)));
            Assert.AreEqual(ulong.MaxValue, Base64.Default.DecodeUInt64(Base64.Default.Encode(ulong.MaxValue)));
        }
    }
}