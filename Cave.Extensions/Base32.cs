using System;
using System.Collections.Generic;

namespace Cave
{
    /// <summary>Gets Base32 en-/decoding.</summary>
    public class Base32 : BaseX
    {
        const int BitCount = 5;

        /// <summary>Initializes a new instance of the <see cref="Base32" /> class.</summary>
        /// <param name="dictionary">The dictionary containing 64 ascii characters used for encoding.</param>
        /// <param name="padding">The padding (use null to skip padding).</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Base32(CharacterDictionary dictionary, char? padding)
            : base(dictionary, BitCount)
        {
            Padding = padding;
            if (Padding != null)
            {
                int paddingChar = (char) Padding;
                if ((paddingChar < 0) || (paddingChar > 127))
                {
                    throw new ArgumentOutOfRangeException(nameof(padding));
                }
            }
        }

        /// <summary>Gets the used padding character or null.</summary>
        public char? Padding { get; }

        #region public decoder interface

        /// <summary>Decodes a Base32 data array.</summary>
        /// <param name="data">The Base32 data to decode.</param>
        public override byte[] Decode(byte[] data)
        {
            if (CharacterDictionary == null)
            {
                throw new InvalidOperationException($"Property {nameof(CharacterDictionary)} has to be set!");
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (Padding != null)
            {
                int paddingChar = (char) Padding;
                if ((paddingChar < 0) || (paddingChar > 127))
                {
                    throw new InvalidOperationException("Invalid padding character!");
                }
            }

            // decode data
            var result = new List<byte>(data.Length);
            var value = 0;
            var bits = 0;
            foreach (var b in data)
            {
                if (b == Padding)
                {
                    break;
                }

                value <<= BitCount;
                bits += BitCount;
                value |= CharacterDictionary.GetValue((char) b);
                if (bits >= 8)
                {
                    bits -= 8;
                    var outValue = value >> bits;
                    value = value & ~(0xFFFF << bits);
                    result.Add((byte) outValue);
                }
            }

            return result.ToArray();
        }

        #endregion

        #region public encoder interface

        /// <summary>Encodes the specified data.</summary>
        /// <param name="data">The data to encode.</param>
        public override string Encode(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            var result = new List<char>(data.Length * 2);
            var value = 0;
            var bits = 0;
            foreach (var b in data)
            {
                value = (value << 8) | b;
                bits += 8;
                while (bits >= BitCount)
                {
                    bits -= BitCount;
                    var outValue = value >> bits;
                    value = value & ~(0xFFFF << bits);
                    result.Add(CharacterDictionary.GetCharacter(outValue));
                }
            }

            if (bits >= BitCount)
            {
                bits -= BitCount;
                var outValue = value >> bits;
                value = value & ~(0xFFFF << bits);
                result.Add(CharacterDictionary.GetCharacter(outValue));
            }

            if (bits > 0)
            {
                var shift = BitCount - bits;
                var outValue = value << shift;
                result.Add(CharacterDictionary.GetCharacter(outValue));
                bits -= BitCount;
            }

            if (Padding != null)
            {
                var padding = (char) Padding;
                while ((bits % 8) != 0)
                {
                    result.Add(padding);
                    bits -= BitCount;
                }
            }

            return new string(result.ToArray());
        }

        #endregion

        #region public static default instances

        /// <summary>Gets the otp charset for Base32 en-/decoding (no padding).</summary>
        public static Base32 OTP => new Base32(new CharacterDictionary("abcdefghijklmnopqrstuvwxyz234567"), null);

        /// <summary>Gets the default (uppercase) charset for base32 en-/decoding with padding.</summary>
        public static Base32 Default => new Base32(new CharacterDictionary("0123456789ABCDEFGHIJKLMNOPQRSTUV"), '=');

        /// <summary>Gets the default (uppercase) charset for Base32 en-/decoding without padding.</summary>
        public static Base32 NoPadding => new Base32(new CharacterDictionary("0123456789ABCDEFGHIJKLMNOPQRSTUV"), null);

        /// <summary>Gets the url safe dictatable (no i,l,v,0) charset for Base32 en-/decoding (no padding).</summary>
        public static Base32 Safe => new Base32(new CharacterDictionary("abcdefghjkmnopqrstuwxyz123456789"), null);

        #endregion
    }
}
