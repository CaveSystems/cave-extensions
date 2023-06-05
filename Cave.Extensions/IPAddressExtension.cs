using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Cave;

/// <summary>Gets extensions on <see cref="IPAddress" /> instances.</summary>
public static class IPAddressExtension
{
    #region Static

    /// <summary>The ipv4 multicast address.</summary>
    public static readonly IPAddress IPv4MulticastAddress = IPAddress.Parse("224.0.0.0");

    /// <summary>The ipv6 multicast address.</summary>
    public static readonly IPAddress IPv6MulticastAddress = IPAddress.Parse("FF00::");

    /// <summary>Returns a new address with the specified netmask.</summary>
    /// <param name="address">The address.</param>
    /// <param name="netmask">The netmask.</param>
    /// <returns>Returns an <see cref="IPAddress" /> instance.</returns>
    /// <exception cref="ArgumentNullException">address or netmask.</exception>
    /// <exception cref="ArgumentOutOfRangeException">AddressFamily of address and netmask do not match.</exception>
    public static IPAddress GetAddress(this IPAddress address, IPAddress netmask)
    {
        if (address == null)
        {
            throw new ArgumentNullException(nameof(address));
        }

        if (netmask == null)
        {
            throw new ArgumentNullException(nameof(netmask));
        }

        if (address.AddressFamily != netmask.AddressFamily)
        {
            throw new ArgumentOutOfRangeException(nameof(netmask), "AddressFamily of address and netmask do not match");
        }

        var result = address.GetAddressBytes();
        var addr = address.GetAddressBytes();
        var mask = netmask.GetAddressBytes();
        for (var i = 0; i < mask.Length; i++)
        {
            result[i] = (byte)(addr[i] & mask[i]);
        }

        return new(result);
    }

    /// <summary>Gets the local broadcast address for the specified <paramref name="address" /> and <paramref name="subnet" /> size combination.</summary>
    /// <param name="address">Address information.</param>
    /// <param name="subnet">The subnet size.</param>
    /// <returns>Returns a local broadcast address.</returns>
    public static IPAddress GetBroadcastAddress(this IPAddress address, int subnet) => GetSubnet(address, subnet).Broadcast;

    /// <summary>Gets the local broadcast address for the specified <paramref name="address" /> and <paramref name="netmask" /> combination.</summary>
    /// <param name="address">Address information.</param>
    /// <param name="netmask">Netmask.</param>
    /// <returns>Returns a local broadcast address.</returns>
    public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress netmask) => new IPNetwork(address, netmask).Broadcast;

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
    /// <summary>Gets the local broadcast address for the specified <see cref="UnicastIPAddressInformation" />.</summary>
    /// <param name="address">Address information.</param>
    /// <param name="subnet">Subnet length. (Required for ipv6 on framework &lt;= 4.0).</param>
    /// <returns>Returns a local broadcast address.</returns>
    public static IPAddress GetBroadcastAddress(this UnicastIPAddressInformation address, int subnet = -1)
    {
        if (address.Address.AddressFamily == AddressFamily.InterNetwork)
        {
            if (subnet > -1)
            {
                return GetBroadcastAddress(address.Address, subnet);
            }

            return GetBroadcastAddress(address.Address, address.IPv4Mask);
        }

        if (address.Address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            if (subnet > -1)
            {
                return GetBroadcastAddress(address.Address, subnet);
            }
#if NET20 || NET35 || NET40
            throw new NotSupportedException("Prefixlength unknown, subnet length is required! (Use >= net 4.5, netstandard 2.0)");
#else
            return GetBroadcastAddress(address.Address, address.PrefixLength);
#endif
        }

        throw new NotSupportedException($"AddressFamily {address.Address.AddressFamily} is not supported!");
    }
#endif

    /// <summary>Gets the name of the reverse lookup zone of an ipv4 or ipv6 address.</summary>
    /// <param name="address">The address.</param>
    /// <param name="subnet">The subnet.</param>
    /// <returns>Returns the reverse lookup zone name.</returns>
    public static string GetReverseLookupZone(this IPAddress address, int subnet = -1)
    {
        if (address == null)
        {
            throw new ArgumentNullException(nameof(address));
        }

        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
            var sb = new StringBuilder();
            var skipBits = subnet > -1 ? 32 - subnet : 0;
            var bytes = address.GetAddressBytes();
            for (var i = 3; i >= 0; i--)
            {
                if (skipBits >= 8)
                {
                    skipBits -= 8;
                    continue;
                }

                if (sb.Length > 0)
                {
                    sb.Append('.');
                }

                sb.Append(bytes[i]);
            }

            sb.Append(".in-addr.arpa.");
            return sb.ToString();
        }

        if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            var sb = new StringBuilder();
            var skipBits = subnet > -1 ? 128 - subnet : 0;
            var nibbles = address.GetAddressBytes().GetNibbles().Reverse();
            foreach (var nibble in nibbles)
            {
                if (skipBits >= 4)
                {
                    skipBits -= 4;
                    continue;
                }

                sb.Append($"{nibble:x}.");
            }

            if (sb.Length == 0)
            {
                return "0.ip6.arpa.";
            }

            sb.Append("ip6.arpa.");
            return sb.ToString();
        }

        throw new NotImplementedException($"AddressFamily {address.AddressFamily} is not implemented!");
    }

    /// <summary>Gets the name of the reverse lookup zone of an ipv4 or ipv6 address.</summary>
    /// <param name="address">The address.</param>
    /// <param name="mask">The subnet mask.</param>
    /// <returns>Returns the reverse lookup zone name.</returns>
    public static string GetReverseLookupZone(this IPAddress address, IPAddress mask) => GetReverseLookupZone(address, mask.GetSubnetBits());

    /// <summary>Gets the ipv4 <see cref="IPNetwork" /> instance for the specified address and subnet.</summary>
    /// <param name="address">The address within the subnet.</param>
    /// <param name="subnet">The subnet.</param>
    /// <returns>A new <see cref="IPNetwork" /> instance.</returns>
    public static IPNetwork GetSubnet(this IPAddress address, int subnet) => new(address, subnet);

    /// <summary>Gets the number of continious bits within the subnet (counted from the lsb).</summary>
    /// <param name="mask">The address mask.</param>
    /// <returns>The number of continious bits.</returns>
    public static int GetSubnetBits(this IPAddress mask)
    {
        if (mask == null)
        {
            throw new ArgumentNullException(nameof(mask));
        }

        if (mask.AddressFamily == AddressFamily.InterNetwork)
        {
            var maskBits = BitConverter.ToUInt32(mask.GetAddressBytes(), 0);
            var subnet = 0;
            while ((maskBits & 1) != 0)
            {
                subnet++;
                maskBits >>= 1;
            }

            return subnet;
        }

        if (mask.AddressFamily == AddressFamily.InterNetworkV6)
        {
            var subnet = 0;
            var nibbles = mask.GetAddressBytes().GetNibbles().Reverse();
            foreach (var nibble in nibbles)
            {
                for (var i = 0; i < 4; i++)
                {
                    if ((nibble & (1 << i)) == 0)
                    {
                        subnet++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (nibble != 0)
                {
                    break;
                }
            }

            return subnet;
        }

        throw new NotImplementedException($"AddressFamily {mask.AddressFamily} is not implemented!");
    }

    /// <summary>Returns a value indicating whether a ip address is a multicast address.</summary>
    /// <param name="address"> Instance of the IPAddress, that should be used. </param>
    /// <returns> true, if the given address is a multicast address; otherwise, false. </returns>
    public static bool IsMulticast(this IPAddress address)
    {
        if (address == null)
        {
            throw new ArgumentNullException(nameof(address));
        }

        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
            return address.GetSubnet(4).Address.Equals(IPv4MulticastAddress);
        }

        if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            return address.GetSubnet(8).Address.Equals(IPv6MulticastAddress);
        }

        throw new ArgumentException("Invalid AddressFamily!", nameof(address));
    }

    /// <summary>Modifies an <see cref="IPAddress" />.</summary>
    /// <param name="address">The base address.</param>
    /// <param name="modifier">The modifier function.</param>
    /// <returns>Returns the modified address.</returns>
    public static IPAddress Modify(this IPAddress address, Action<byte[]> modifier)
    {
        if (address == null)
        {
            throw new ArgumentNullException(nameof(address));
        }

        if (modifier == null)
        {
            throw new ArgumentNullException(nameof(modifier));
        }

        var bytes = address.GetAddressBytes();
        modifier(bytes);
        return new(bytes);
    }

#endregion
}
