using System.Linq;
using Cave;
using NUnit.Framework;

namespace Test;

public class BufferExtensionsTests
{
    #region Public Methods

    [Test]
    public void SwapEndianTest()
    {
        var buffer = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, }.AsReadOnly();
        var buffer16 = new byte[] { 2, 1, 4, 3, 6, 5, 8, 7, 10, 9, 12, 11, 14, 13, 16, 15 };
        var buffer32 = new byte[] { 4, 3, 2, 1, 8, 7, 6, 5, 12, 11, 10, 9, 16, 15, 14, 13 };
        var buffer64 = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1, 16, 15, 14, 13, 12, 11, 10, 9 };

        buffer16.SwapEndian16();
        CollectionAssert.AreEqual(buffer, buffer16);
        buffer32.SwapEndian32();
        CollectionAssert.AreEqual(buffer, buffer32);
        buffer64.SwapEndian64();
        CollectionAssert.AreEqual(buffer, buffer64);

        unchecked
        {
            const short v1 = (short)0xFEED;
            const short v2 = (short)0xEDFE;
            var test = v2.SwapEndian();
            Assert.AreEqual(v1, test);
        }
        unchecked
        {
            const int v1 = (int)0xF1E2E3D4;
            const int v2 = (int)0xD4E3E2F1;
            var test = v2.SwapEndian();
            Assert.AreEqual(v1, test);
        }
        unchecked
        {
            const long v1 = (long)0xF101E202E303D404;
            const long v2 = (long)0x04D403E302E201F1;
            var test = v2.SwapEndian();
            Assert.AreEqual(v1, test);
        }
    }

    #endregion Public Methods
}
