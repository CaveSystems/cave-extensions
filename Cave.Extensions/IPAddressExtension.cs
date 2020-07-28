using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Cave
{
    /// <summary>Gets extensions on <see cref="IPAddress" /> instances.</summary>
    public static class IPAddressExtension
    {
        /// <summary>Modifies an <see cref="IPAddress" />.</summary>
        /// <param name="address">The base address.</param>
        /// <param name="modifier">The modifier function.</param>
        /// <returns>Returns the modified address.</returns>
        public static IPAddress Modify(this IPAddress address, Action<byte[]> modifier)
        {
            var bytes = address.GetAddressBytes();
            modifier(bytes);
            return new IPAddress(bytes);
        }

        /// <summary>Gets the ipv4 <see cref="IPNetwork" /> instance for the specified address and subnet.</summary>
        /// <param name="address">The address within the subnet.</param>
        /// <param name="subnet">The subnet.</param>
        /// <returns>A new <see cref="IPNetwork" /> instance.</returns>
        public static IPNetwork GetSubnet(this IPAddress address, int subnet)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                var bytes = address.GetAddressBytes();
                var value = (long) BitConverter.ToUInt32(bytes, 0);
                value &= ~(0xFFFFFFFF >> subnet);
                return new IPNetwork(new IPAddress(value), subnet);
            }

            throw new NotImplementedException($"AddressFamily {address.AddressFamily} is not implemented!");
        }

        /// <summary>Gets the name of the reverse lookup zone of an ipv4 or ipv6 address.</summary>
        /// <param name="address">The address.</param>
        /// <param name="subnet">The subnet.</param>
        /// <returns>Returns the reverse lookup zone name.</returns>
        public static string GetReverseLookupZone(this IPAddress address, int subnet = -1)
        {
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

                sb.Append(".in-addr.arpa");
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

                    if (sb.Length > 0)
                    {
                        sb.Append('.');
                    }

                    sb.Append(nibble);
                }

                sb.Append(".ip6.arpa");
                return sb.ToString();
            }

            throw new NotImplementedException($"AddressFamily {address.AddressFamily} is not implemented!");
        }

        /// <summary>Gets the name of the reverse lookup zone of an ipv4 or ipv6 address.</summary>
        /// <param name="address">The address.</param>
        /// <param name="mask">The subnet mask.</param>
        /// <returns>Returns the reverse lookup zone name.</returns>
        public static string GetReverseLookupZone(this IPAddress address, IPAddress mask) => GetReverseLookupZone(address, mask.GetSubnetBits());

        /// <summary>Gets the number of continious bits within the subnet (counted from the lsb).</summary>
        /// <param name="mask">The address mask.</param>
        /// <returns>The number of continious bits.</returns>
        public static int GetSubnetBits(this IPAddress mask)
        {
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
    }
}
