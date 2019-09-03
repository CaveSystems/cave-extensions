﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Cave
{
    /// <summary>
    /// Provides an ipv4 subnet definition.
    /// </summary>
    public class IPNetwork
    {
        /// <summary>
        /// Parses an <see cref="IPNetwork"/> instance from the specified string.
        /// </summary>
        /// <param name="text">String to parse.</param>
        /// <returns>Returns a new <see cref="IPNetwork"/> instance.</returns>
        public static IPNetwork Parse(string text)
        {
            var parts = text.Trim().Split(new[] { ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                throw new ArgumentException("IPNetwork requires format <address>/<mask> or <address>/<subnet>!");
            }

            var addr = IPAddress.Parse(parts[0]);
            if (int.TryParse(parts[1], out int subnet))
            {
                return new IPNetwork(addr, subnet);
            }
            else
            {
                var mask = IPAddress.Parse(parts[1]);
                return new IPNetwork(addr, mask);
            }
        }

        /// <summary>
        /// Gets the name of the reverse lookup zone.
        /// </summary>
        public string ReverseLookupZone
        {
            get
            {
                if (Subnet > 0)
                {
                    return Address.GetReverseLookupZone(Subnet);
                }
                else
                {
                    return Address.GetReverseLookupZone(Mask);
                }
            }
        }

        /// <summary>
        /// Gets the base ip address.
        /// </summary>
        public IPAddress Address { get; }

        /// <summary>
        /// Gets the subnet mask.
        /// </summary>
        public IPAddress Mask { get; }

        /// <summary>
        /// Gets the subnet or -1 if the mask has to be used.
        /// </summary>
        public int Subnet { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPNetwork"/> class.
        /// </summary>
        /// <param name="address">Base address.</param>
        /// <param name="subnet">Subnet.</param>
        public IPNetwork(IPAddress address, int subnet)
        {
            Address = address.AddressFamily == AddressFamily.InterNetwork ? address : throw new ArgumentOutOfRangeException(nameof(address));
            if (subnet < 0 || subnet > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(subnet));
            }

            long value = 0xFFFFFFFF >> (32 - subnet);
            Mask = new IPAddress(value);
            Subnet = subnet;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPNetwork"/> class.
        /// </summary>
        /// <param name="address">Base address.</param>
        /// <param name="mask">Subnet mask.</param>
        public IPNetwork(IPAddress address, IPAddress mask)
        {
            Address = address;
            Mask = mask;
            var addressBytes = Address.GetAddressBytes();
            var maskBytes = Mask.GetAddressBytes();

            if (maskBytes.Length != addressBytes.Length)
            {
                throw new ArgumentException("Address length does not match!");
            }

            for (int i = 0; i < maskBytes.Length; i++)
            {
                if ((addressBytes[i] & maskBytes[i]) != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(mask), "BaseAddress does not match mask!");
                }
            }

            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                if (maskBytes.Length != 4)
                {
                    throw new ArgumentOutOfRangeException(nameof(mask), "Mask bytes do not match AddressFamily.InterNetwork.");
                }

                long maskValue = BitConverter.ToUInt32(maskBytes, 0);
                bool CheckMask(int bit) => (maskValue & (1 << bit)) != 0;

                int subnet = 0;
                while (subnet < 32 && CheckMask(subnet))
                {
                    subnet++;
                }

                for (int i = subnet; i < 32; i++)
                {
                    if (CheckMask(i))
                    {
                        subnet = -1;
                        break;
                    }
                }
                Subnet = subnet > -1 ? 32 - subnet : subnet;
            }
        }

        /// <summary>
        /// Gets a string {ipaddress}/{subnet} or {ipaddress}/{mask}.
        /// This can be parsed by <see cref="Parse(string)"/>.
        /// </summary>
        /// <returns>Parsable string describing this instance.</returns>
        public override string ToString() => $"{Address}/{(Subnet > -1 ? Subnet.ToString() : Mask.ToString())}";
    }
}
