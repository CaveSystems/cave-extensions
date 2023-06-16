using System;
using System.Diagnostics;
using System.Text;
using Cave;
using Cave.CodeGen;
using NUnit.Framework;

namespace Test;

[TestFixture]
public class TestHashCode
{
    static void Check(IChecksum<uint> checksum, byte[] data, uint value)
    {
        checksum.Reset();
        checksum.Update(data);
        Assert.AreEqual(value, checksum.Value);
    }

    [Test]
    public void Crc32aTest()
    {
        var crc32 = new FastCrc32();
        //check expected table against calculated table
        CollectionAssert.AreEqual(CRC32.BZIP2.Table, FastCrc32.Table);
        //check crc test value
        Check(crc32, Encoding.ASCII.GetBytes("123456789"), 0xfc891918);
        Check(crc32, Encoding.ASCII.GetBytes("Check123!"), 0x292C603E);
    }

    const int HashCount = 12345678;

#if NET5_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER
    [Test]
    public void RpmHashCodeFramework()
    {
        HashCode hash = new();
        var watch = Stopwatch.StartNew();
        for (var i = 0; i < HashCount; i++)
        {
            hash.Add(i);
        }
        watch.Stop();
        Console.WriteLine($"HashCode: {HashCount} checksums: {HashCount * 1000.0 / watch.ElapsedMilliseconds:N3} /s");
    }
#endif

    [Test]
    public void RpmXxHash32()
    {
        XxHash32 hash = new();
        var watch = Stopwatch.StartNew();
        for (var i = 0; i < HashCount; i++)
        {
            hash.Add(i);
        }
        watch.Stop();
        Console.WriteLine($"XxHash32: {HashCount} checksums: {(HashCount * 1000.0) / watch.ElapsedMilliseconds:N3} /s");
    }

    [Test]
    public void RpmFastCrc32()
    {
        var crc = new FastCrc32();
        var watch = Stopwatch.StartNew();
        for (var i = 0; i < HashCount; i++)
        {
            crc.Add(i);
        }
        watch.Stop();
        Console.WriteLine($"FastCrc32: {HashCount} checksums: {(HashCount * 1000.0) / watch.ElapsedMilliseconds:N3} /s");
    }
}
