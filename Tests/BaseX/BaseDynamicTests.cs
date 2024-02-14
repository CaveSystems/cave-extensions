using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Cave;
using Cave.Collections;
using NUnit.Framework;

namespace Test.BaseX;

[TestFixture]
public class BaseDynamicTests
{
    #region Public Methods

    [Test]
    public void TestRoundtrips()
    {
        for (int i = 2; i <= ASCII.Strings.Printable.Length; i++)
        {
            var characters = ASCII.Strings.Printable.Substring(0, i);
            var b = new BaseDynamic(characters, false, null);

            for (int n = 0; n < 100; n++)
            {
                var value = n.GetHashCode();
                var encoded = b.Encode(value);
                var decoded = b.DecodeValue(encoded);
                Assert.AreEqual(value, decoded);
                Assert.IsFalse(encoded.Except(characters).Any());
            }
        }
    }

    [Test]
    public void TestValues()
    {
        for (int i = 2; i <= ASCII.Strings.Printable.Length; i++)
        {
            var characters = ASCII.Strings.Printable.Substring(0, i);
            var b = new BaseDynamic(characters, false, null);

            for (int n = 1; n < i; n++)
            {
                var value = n;
                var encoded = new[]
                {
                    b.Encode((byte)value),
                    b.Encode((sbyte)value),
                    b.Encode((short)value),
                    b.Encode((ushort)value),
                    b.Encode((int)value),
                    b.Encode((uint)value),
                    b.Encode((long)value),
                    b.Encode((ulong)value)
                };
                var decoded = b.DecodeValue(encoded[0]);
                Assert.AreEqual(value, decoded);
                Assert.IsTrue(encoded.All(a => a.Length == 1), $"Not all encoded values are of length 1! ({encoded.Select(e => e.Length).Join(',')})");
                Assert.IsTrue(encoded.All(a => a.Equals(encoded[0])), $"Not all encoded values match!\n{encoded.JoinNewLine()}");
            }
        }
    }

    #endregion Public Methods
}
