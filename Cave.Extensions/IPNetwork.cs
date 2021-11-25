using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Cave
{
    /// <summary>
    /// Gets an ipv4 subnet definition.
    /// </summary>
    public class IPNetwork : IEquatable<IPNetwork>
    {
        #region Static

        #region Private Methods

        static IEnumerable<byte> GetBits(byte b)
        {
            for (var i = 0; i < 8; i++)
            {
                yield return (byte)((b >> i) & 1);
            }
        }

        static IEnumerable<byte> GetBytes(IEnumerable<byte> bits)
        {
            var i = 0;
            byte current = 0;
            foreach (var bit in bits)
            {
                current |= (byte)(bit << i);
                if (++i == 8)
                {
                    i = 0;
                    yield return current;
                    current = 0;
                }
            }
        }

        static IPAddress GetMask(int subnet, AddressFamily addressFamily)
        {
            var result = addressFamily switch
            {
                AddressFamily.InterNetwork => new byte[4],
                AddressFamily.InterNetworkV6 => new byte[16],
                _ => throw new ArgumentOutOfRangeException(nameof(addressFamily)),
            };
            var byteCount = subnet / 8;
            var bitCount = subnet % 8;
            for (var i = 0; i < byteCount; i++)
            {
                result[i] = 255;
            }

            for (byte i = 128; bitCount > 0; bitCount--, i >>= 1)
            {
                result[byteCount] |= i;
            }

            return new IPAddress(result);
        }

        #endregion Private Methods

        #endregion Static

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IPNetwork"/> class.
        /// </summary>
        /// <param name="address">Base address.</param>
        /// <param name="subnet">Subnet.</param>
        public IPNetwork(IPAddress address, int subnet) : this(address, GetMask(subnet, address?.AddressFamily ?? throw new ArgumentNullException(nameof(address))))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPNetwork"/> class.
        /// </summary>
        /// <param name="address">Base address.</param>
        /// <param name="mask">Subnet mask.</param>
        public IPNetwork(IPAddress address, IPAddress mask)
        {
            var addressBytes = address?.GetAddressBytes() ?? throw new ArgumentNullException(nameof(address));
            var broadcastBytes = (byte[])addressBytes.Clone();
            var maskBytes = mask?.GetAddressBytes() ?? throw new ArgumentNullException(nameof(mask));
            if (maskBytes.Length != addressBytes.Length)
            {
                throw new ArgumentException("Address length does not match!");
            }

            var bitCounter = 0;
            var bitsMixed = false;
            var bitsDone = false;
            for (var i = 0; i < maskBytes.Length; i++)
            {
                var maskByte = maskBytes[i];
                if (!bitsMixed)
                {
                    if (maskByte == 0xFF)
                    {
                        if (bitsDone)
                        {
                            bitsMixed = true;
                        }
                        else
                        {
                            bitCounter += 8;
                        }
                    }
                    else if (maskByte == 0)
                    {
                        bitsDone = true;
                    }
                    else
                    {
                        for (var bit = 128; bit > 0; bit >>= 1)
                        {
                            if ((maskByte & bit) == bit)
                            {
                                if (bitsDone)
                                {
                                    bitsMixed = true;
                                    break;
                                }

                                bitCounter++;
                            }
                            else
                            {
                                bitsDone = true;
                            }
                        }
                    }
                }

                addressBytes[i] &= maskByte;
                broadcastBytes[i] |= (byte)~maskByte;
            }

            if (bitsMixed)
            {
                Subnet = -1;
            }
            else
            {
                Subnet = bitCounter;
            }

            Address = new IPAddress(addressBytes);
            Mask = new IPAddress(maskBytes);
            Broadcast = new IPAddress(broadcastBytes);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the base ip address.
        /// </summary>
        public IPAddress Address { get; }

        /// <summary>
        /// Gets the broadcast address.
        /// </summary>
        public IPAddress Broadcast { get; }

        /// <summary>
        /// Gets the subnet mask.
        /// </summary>
        public IPAddress Mask { get; }

        /// <summary>
        /// Gets the name of the reverse lookup zone.
        /// </summary>
        public string ReverseLookupZone => Subnet > 0 ? Address.GetReverseLookupZone(Subnet) : Address.GetReverseLookupZone(Mask);

        /// <summary>
        /// Gets the subnet or -1 if the mask has to be used.
        /// </summary>
        public int Subnet { get; }

        /// <summary>
        /// Gets an enumerator for all addresses present at the network.
        /// </summary>
        public IEnumerable<IPAddress> Addresses
        {
            get
            {
                if (AddressesBitCount == 0)
                {
                    yield return Address;
                }
                else
                {
                    var mask = Mask.GetAddressBytes().Reverse().SelectMany(GetBits).ToArray();
                    var value = Address.GetAddressBytes().Reverse().SelectMany(GetBits).ToArray();
#if DEBUG
                    var test = new IPAddress(GetBytes(value).Reverse().ToArray());
                    if (!Equals(Address, test)) throw new Exception("Adress roundtrip test failed!");
#endif
                    while (true)
                    {
                        var done = false;
                        for (var n = 0; n < mask.Length; n++)
                        {
                            if (mask[n] == 1)
                            {
                                continue;
                            }
                            if (value[n] == 0)
                            {
                                value[n] = 1;
                                done = true;
                                break;
                            }
                            value[n] = 0;
                        }
                        if (!done) break;

                        var address = GetBytes(value).Reverse().ToArray();
                        yield return new IPAddress(address);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of addresses present at the network. This returns 0 for networks with subnets greater than 64 bits.
        /// </summary>
        /// <remarks>This is a fast calculation for all addresses present at <see cref="Addresses"/>.</remarks>
        public ulong AddressesCount
        {
            get
            {
                var bits = AddressesBitCount;
                if (bits == 0) return 1;
                if (bits == 64) return ulong.MaxValue;
                if (bits > 64) return 0;
                return (1UL << bits) - 1;
            }
        }

        /// <summary>
        /// Gets the number of bits used for addresses at the network.
        /// </summary>
        public int AddressesBitCount => Mask.GetAddressBytes().SelectMany(GetBits).Count(b => b == 0);

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Parses an <see cref="IPNetwork"/> instance from the specified string.
        /// </summary>
        /// <param name="text">String to parse.</param>
        /// <returns>Returns a new <see cref="IPNetwork"/> instance.</returns>
        public static IPNetwork Parse(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var parts = text.Trim().Split(new[]
            {
                ' ',
                '/'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                throw new ArgumentException("IPNetwork requires format <address>/<mask> or <address>/<subnet>!");
            }

            var addr = IPAddress.Parse(parts[0]);
            if (int.TryParse(parts[1], out var subnet))
            {
                return new IPNetwork(addr, subnet);
            }

            var mask = IPAddress.Parse(parts[1]);
            return new IPNetwork(addr, mask);
        }

        /// <inheritdoc/>
        public bool Equals(IPNetwork other) => (other != null) && Equals(other.Address, Address) && Equals(other.Mask, Mask) && (other.Subnet == Subnet);

        /// <inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as IPNetwork);

        /// <inheritdoc/>
        public override int GetHashCode() => ToString().GetHashCode();

        /// <summary>
        /// Gets a string {ipaddress}/{subnet} or {ipaddress}/{mask}. This can be parsed by <see cref="Parse(string)"/>.
        /// </summary>
        /// <returns>Parsable string describing this instance.</returns>
        public override string ToString() => $"{Address}/{(Subnet > -1 ? Subnet : (object)Mask)}";

        #endregion Public Methods
    }
}
