using System.Linq;
using System.Net;
using Cave;
using NUnit.Framework;

namespace Test.Extensions;

[TestFixture]
class IPExtensionTests
{
    [Test]
    public void IPNetwork_IPv4_10()
    {
        const int net = 10;
        var expected = new IPNetwork(IPAddress.Parse("192.128.0.0"), net);
        var test = IPAddress.Parse("192.168.255.255").GetSubnet(net);
        Assert.AreEqual(expected, test);
        Assert.AreEqual("128.192.in-addr.arpa.", expected.ReverseLookupZone);
        Assert.AreEqual("128.192.in-addr.arpa.", test.ReverseLookupZone);
        Assert.AreEqual("255.192.0.0", expected.Mask.ToString());
        Assert.AreEqual("255.192.0.0", test.Mask.ToString());
        Assert.AreEqual("192.191.255.255", test.Broadcast.ToString());
        Assert.AreEqual(net, expected.Subnet);
        Assert.AreEqual(net, test.Subnet);
        var test2 = new IPNetwork(IPAddress.Parse("192.168.255.255"), expected.Mask);
        Assert.AreEqual(expected, test2);
        Assert.AreEqual(net, test2.Subnet);
        Assert.AreEqual(4194303, test.AddressesCount);
        Assert.AreEqual(4194303, test.Addresses.Count());
        Assert.AreEqual(32 - net, test.AddressesBitCount);
    }

    [Test]
    public void IPNetwork_IPv4_11()
    {
        const int net = 11;
        var expected = new IPNetwork(IPAddress.Parse("192.160.0.0"), net);
        var test = IPAddress.Parse("192.168.255.255").GetSubnet(net);
        Assert.AreEqual(expected, test);
        Assert.AreEqual("160.192.in-addr.arpa.", expected.ReverseLookupZone);
        Assert.AreEqual("160.192.in-addr.arpa.", test.ReverseLookupZone);
        Assert.AreEqual("255.224.0.0", expected.Mask.ToString());
        Assert.AreEqual("255.224.0.0", test.Mask.ToString());
        Assert.AreEqual("192.191.255.255", test.Broadcast.ToString());
        Assert.AreEqual(net, expected.Subnet);
        Assert.AreEqual(net, test.Subnet);
        var test2 = new IPNetwork(IPAddress.Parse("192.168.255.255"), expected.Mask);
        Assert.AreEqual(expected, test2);
        Assert.AreEqual(net, test2.Subnet);
        Assert.AreEqual(2097151, test.AddressesCount);
        Assert.AreEqual(2097151, test.Addresses.Count());
        Assert.AreEqual(32 - net, test.AddressesBitCount);
    }

    [Test]
    public void IPNetwork_IPv4_12()
    {
        const int net = 12;
        var expected = new IPNetwork(IPAddress.Parse("192.160.0.0"), net);
        var test = IPAddress.Parse("192.168.255.255").GetSubnet(net);
        Assert.AreEqual(expected, test);
        Assert.AreEqual("160.192.in-addr.arpa.", expected.ReverseLookupZone);
        Assert.AreEqual("160.192.in-addr.arpa.", test.ReverseLookupZone);
        Assert.AreEqual("255.240.0.0", expected.Mask.ToString());
        Assert.AreEqual("255.240.0.0", test.Mask.ToString());
        Assert.AreEqual("192.175.255.255", test.Broadcast.ToString());
        Assert.AreEqual(net, expected.Subnet);
        Assert.AreEqual(net, test.Subnet);
        var test2 = new IPNetwork(IPAddress.Parse("192.168.255.255"), expected.Mask);
        Assert.AreEqual(expected, test2);
        Assert.AreEqual(net, test2.Subnet);
        Assert.AreEqual(1048575, test.AddressesCount);
        Assert.AreEqual(1048575, test.Addresses.Count());
        Assert.AreEqual(32 - net, test.AddressesBitCount);
    }

    [Test]
    public void IPNetwork_IPv4_13()
    {
        const int net = 13;
        var expected = new IPNetwork(IPAddress.Parse("192.168.0.0"), net);
        var test = IPAddress.Parse("192.168.255.255").GetSubnet(net);
        Assert.AreEqual(expected, test);
        Assert.AreEqual("168.192.in-addr.arpa.", expected.ReverseLookupZone);
        Assert.AreEqual("168.192.in-addr.arpa.", test.ReverseLookupZone);
        Assert.AreEqual("255.248.0.0", expected.Mask.ToString());
        Assert.AreEqual("255.248.0.0", test.Mask.ToString());
        Assert.AreEqual("192.175.255.255", test.Broadcast.ToString());
        Assert.AreEqual(net, expected.Subnet);
        Assert.AreEqual(net, test.Subnet);
        var test2 = new IPNetwork(IPAddress.Parse("192.168.255.255"), expected.Mask);
        Assert.AreEqual(expected, test2);
        Assert.AreEqual(net, test2.Subnet);
        Assert.AreEqual(524287, test.AddressesCount);
        Assert.AreEqual(524287, test.Addresses.Count());
        Assert.AreEqual(32 - net, test.AddressesBitCount);
    }

    [Test]
    public void IPNetwork_IPv4_14()
    {
        const int net = 14;
        var expected = new IPNetwork(IPAddress.Parse("192.168.0.0"), net);
        var test = IPAddress.Parse("192.168.255.255").GetSubnet(net);
        Assert.AreEqual(expected, test);
        Assert.AreEqual("168.192.in-addr.arpa.", expected.ReverseLookupZone);
        Assert.AreEqual("168.192.in-addr.arpa.", test.ReverseLookupZone);
        Assert.AreEqual("255.252.0.0", expected.Mask.ToString());
        Assert.AreEqual("255.252.0.0", test.Mask.ToString());
        Assert.AreEqual("192.171.255.255", test.Broadcast.ToString());
        Assert.AreEqual(net, expected.Subnet);
        Assert.AreEqual(net, test.Subnet);
        var test2 = new IPNetwork(IPAddress.Parse("192.168.255.255"), expected.Mask);
        Assert.AreEqual(expected, test2);
        Assert.AreEqual(net, test2.Subnet);
        Assert.AreEqual(262143, test.AddressesCount);
        Assert.AreEqual(262143, test.Addresses.Count());
        Assert.AreEqual(32 - net, test.AddressesBitCount);
    }

    [Test]
    public void IPNetwork_IPv4_15()
    {
        const int net = 15;
        var expected = new IPNetwork(IPAddress.Parse("192.168.0.0"), net);
        var test = IPAddress.Parse("192.168.255.255").GetSubnet(net);
        Assert.AreEqual(expected, test);
        Assert.AreEqual("168.192.in-addr.arpa.", expected.ReverseLookupZone);
        Assert.AreEqual("168.192.in-addr.arpa.", test.ReverseLookupZone);
        Assert.AreEqual("255.254.0.0", expected.Mask.ToString());
        Assert.AreEqual("255.254.0.0", test.Mask.ToString());
        Assert.AreEqual("192.169.255.255", test.Broadcast.ToString());
        Assert.AreEqual(net, expected.Subnet);
        Assert.AreEqual(net, test.Subnet);
        var test2 = new IPNetwork(IPAddress.Parse("192.168.255.255"), expected.Mask);
        Assert.AreEqual(expected, test2);
        Assert.AreEqual(net, test2.Subnet);
        Assert.AreEqual(131071, test.AddressesCount);
        Assert.AreEqual(131071, test.Addresses.Count());
        Assert.AreEqual(32 - net, test.AddressesBitCount);
    }

    [Test]
    public void IPNetwork_IPv4_16()
    {
        const int net = 16;
        var expected = new IPNetwork(IPAddress.Parse("192.168.0.0"), net);
        var test = IPAddress.Parse("192.168.255.255").GetSubnet(net);
        Assert.AreEqual(expected, test);
        Assert.AreEqual("168.192.in-addr.arpa.", expected.ReverseLookupZone);
        Assert.AreEqual("168.192.in-addr.arpa.", test.ReverseLookupZone);
        Assert.AreEqual("255.255.0.0", expected.Mask.ToString());
        Assert.AreEqual("255.255.0.0", test.Mask.ToString());
        Assert.AreEqual("192.168.255.255", test.Broadcast.ToString());
        Assert.AreEqual(net, expected.Subnet);
        Assert.AreEqual(net, test.Subnet);
        var test2 = new IPNetwork(IPAddress.Parse("192.168.255.255"), expected.Mask);
        Assert.AreEqual(expected, test2);
        Assert.AreEqual(net, test2.Subnet);
        Assert.AreEqual(65535, test.AddressesCount);
        Assert.AreEqual(65535, test.Addresses.Count());
        Assert.AreEqual(32 - net, test.AddressesBitCount);
    }

    [Test]
    public void IPNetwork_IPv4_17_24()
    {
        var address = 128;
        for (var net = 17; net <= 24; net++, address |= address >> 1)
        {
            var expected = new IPNetwork(IPAddress.Parse($"192.168.{address}.0"), net);
            var test = IPAddress.Parse("192.168.255.255").GetSubnet(net);
            Assert.AreEqual(expected, test);
            var test2 = new IPNetwork(IPAddress.Parse("192.168.255.255"), expected.Mask);
            Assert.AreEqual(expected, test2);
            Assert.AreEqual(net, test2.Subnet);
            var addrCount = (1 << (32 - net)) - 1;
            Assert.AreEqual(addrCount, test.AddressesCount);
            Assert.AreEqual(addrCount, test.Addresses.Count());
            Assert.AreEqual(32 - net, test.AddressesBitCount);
        }
    }

    [Test]
    public void IPNetwork_IPv4_25_32()
    {
        var addr = 128;
        for (var net = 25; net <= 32; net++, addr |= addr >> 1)
        {
            var expected = new IPNetwork(IPAddress.Parse($"192.168.0.{addr}"), net);
            var test = IPAddress.Parse("192.168.0.255").GetSubnet(net);
            Assert.AreEqual(expected, test);
            var test2 = new IPNetwork(IPAddress.Parse("192.168.0.255"), expected.Mask);
            Assert.AreEqual(expected, test2);
            Assert.AreEqual(net, test2.Subnet);
            var addrCount = (1 << (32 - net)) - 1;
            if (addrCount == 0)
            {
                addrCount = 1;
            }
            Assert.AreEqual(addrCount, test.AddressesCount);
            Assert.AreEqual(addrCount, test.Addresses.Count());
            Assert.AreEqual(32 - net, test.AddressesBitCount);
        }
    }

    [Test]
    public void IPNetwork_IPv4_8()
    {
        const int net = 8;
        var expected = new IPNetwork(IPAddress.Parse("192.0.0.0"), net);
        var test = IPAddress.Parse("192.168.255.255").GetSubnet(net);
        Assert.AreEqual(expected, test);
        Assert.AreEqual("192.in-addr.arpa.", expected.ReverseLookupZone);
        Assert.AreEqual("192.in-addr.arpa.", test.ReverseLookupZone);
        Assert.AreEqual("255.0.0.0", expected.Mask.ToString());
        Assert.AreEqual("255.0.0.0", test.Mask.ToString());
        Assert.AreEqual("192.255.255.255", test.Broadcast.ToString());
        Assert.AreEqual(net, expected.Subnet);
        Assert.AreEqual(net, test.Subnet);
        var test2 = new IPNetwork(IPAddress.Parse("192.168.255.255"), expected.Mask);
        Assert.AreEqual(expected, test2);
        Assert.AreEqual(net, test2.Subnet);
        Assert.AreEqual(16777215, test.AddressesCount);
        Assert.AreEqual(16777215, test.Addresses.Count());
        Assert.AreEqual(32 - net, test.AddressesBitCount);
    }

    [Test]
    public void IPNetwork_IPv4_9()
    {
        const int net = 9;
        var expected = new IPNetwork(IPAddress.Parse("192.128.0.0"), net);
        var test = IPAddress.Parse("192.168.255.255").GetSubnet(net);
        Assert.AreEqual(expected, test);
        Assert.AreEqual("128.192.in-addr.arpa.", expected.ReverseLookupZone);
        Assert.AreEqual("128.192.in-addr.arpa.", test.ReverseLookupZone);
        Assert.AreEqual("255.128.0.0", expected.Mask.ToString());
        Assert.AreEqual("255.128.0.0", test.Mask.ToString());
        Assert.AreEqual("192.255.255.255", test.Broadcast.ToString());
        Assert.AreEqual(net, expected.Subnet);
        Assert.AreEqual(net, test.Subnet);
        var test2 = new IPNetwork(IPAddress.Parse("192.168.255.255"), expected.Mask);
        Assert.AreEqual(expected, test2);
        Assert.AreEqual(net, test2.Subnet);
        Assert.AreEqual(8388607, test.AddressesCount);
        Assert.AreEqual(8388607, test.Addresses.Count());
        Assert.AreEqual(32 - net, test.AddressesBitCount);
    }

    [Test]
    public void IPNetwork_IPv4_HoleySubnet()
    {
        var network = IPAddress.Parse("255.170.170.0");
        var result = IPAddress.Parse("192.170.170.0");
        var test = new IPNetwork(IPAddress.Parse("192.255.255.255"), network);
        Assert.AreEqual(network, test.Mask);
        Assert.AreEqual(result, test.Address);
        Assert.AreEqual(-1, test.Subnet);
        Assert.AreEqual($"{result}/{network}", test.ToString());
    }

    [Test]
    public void IPNetwork_IPv6()
    {
        {
            var test = new IPNetwork(IPAddress.Parse("fe80::"), 16);
            Assert.AreEqual("0.8.e.f.ip6.arpa.", test.ReverseLookupZone);
            Assert.AreEqual("ffff::", test.Mask.ToString());
            Assert.AreEqual(16, test.Subnet);
            Assert.AreEqual(128 - 16, test.AddressesBitCount);
            Assert.AreEqual(0, test.AddressesCount);
        }
        {
            var test = new IPNetwork(IPAddress.Parse("fe80::"), 12);
            Assert.AreEqual("8.e.f.ip6.arpa.", test.ReverseLookupZone);
            Assert.AreEqual("fff0::", test.Mask.ToString());
            Assert.AreEqual(12, test.Subnet);
            Assert.AreEqual(128 - 12, test.AddressesBitCount);
            Assert.AreEqual(0, test.AddressesCount);
        }
        {
            var test = new IPNetwork(IPAddress.Parse("fe80::"), 8);
            Assert.AreEqual("e.f.ip6.arpa.", test.ReverseLookupZone);
            Assert.AreEqual("ff00::", test.Mask.ToString());
            Assert.AreEqual(8, test.Subnet);
            Assert.AreEqual(128 - 8, test.AddressesBitCount);
            Assert.AreEqual(0, test.AddressesCount);
        }
        {
            var test = new IPNetwork(IPAddress.Parse("fe80::"), 4);
            Assert.AreEqual("f.ip6.arpa.", test.ReverseLookupZone);
            Assert.AreEqual("f000::", test.Mask.ToString());
            Assert.AreEqual(4, test.Subnet);
            Assert.AreEqual(128 - 4, test.AddressesBitCount);
            Assert.AreEqual(0, test.AddressesCount);
        }
        {
            var test = new IPNetwork(IPAddress.Parse("fe80::"), 1);
            Assert.AreEqual("8.ip6.arpa.", test.ReverseLookupZone);
            Assert.AreEqual("8000::", test.Mask.ToString());
            Assert.AreEqual(1, test.Subnet);
            Assert.AreEqual(128 - 1, test.AddressesBitCount);
            Assert.AreEqual(0, test.AddressesCount);
        }
    }

    [Test]
    public void IPNetwork_IPv6_HoleySubnet()
    {
        var network = IPAddress.Parse("ffff:f0f0:0f0f:248a:1010::");
        var result = IPAddress.Parse($"{0xca5e & 0xffff:x4}:{0xca5e & 0xf0f0:x4}:{0xca5e & 0x0f0f:x4}:{0xca5e & 0xf248a:x4}:{0xca5e & 0x1010:x4}::");
        var test = new IPNetwork(IPAddress.Parse("ca5e:ca5e:ca5e:ca5e:ca5e:ca5e:ca5e:ca5e"), network);
        Assert.AreEqual(network, test.Mask);
        Assert.AreEqual(result, test.Address);
        Assert.AreEqual(-1, test.Subnet);
        Assert.AreEqual($"{result}/{network}", test.ToString());
    }
}
