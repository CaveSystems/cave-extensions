using System;
using System.Collections.Generic;

namespace Cave
{
    /// <summary>Gets Base64 en-/decoding.</summary>
    public class Base64 : BaseX
    {
        const int BitCount = 6;

        /// <summary>
        /// Gets the padding character.
        /// </summary>
        public char? Padding { get; }

        /// <summary>Initializes a new instance of the <see cref="Base64" /> class.</summary>
        /// <param name="dict">The dictionary containing 64 ascii characters used for encoding.</param>
        /// <param name="padding">The padding (use null to skip padding).</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException">Invalid padding character.</exception>
        public Base64(CharacterDictionary dict, char? padding)
            : base(dict, BitCount)
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

        #region public decoder interface

        /// <summary>Decodes a base64 data array.</summary>
        /// <param name="data">The base64 data to decode.</param>
        public override byte[] Decode(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
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
                    var l_Out = value >> bits;
                    value &= ~(0xFFFF << bits);
                    result.Add((byte) l_Out);
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
                    value &= ~(0xFFFF << bits);
                    result.Add(CharacterDictionary.GetCharacter(outValue));
                }
            }

            if (bits > BitCount)
            {
                bits -= BitCount;
                var outValue = value >> bits;
                value &= ~(0xFFFF << bits);
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

        /// <summary>Gets the default charset for base64 en-/decoding with padding.</summary>
        public static Base64 Default => new Base64(new CharacterDictionary("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"), '=');

        /// <summary>Gets the default charset for base64 en-/decoding without padding.</summary>
        public static Base64 NoPadding => new Base64(new CharacterDictionary("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"), null);

        /// <summary>Gets the url safe charset for base64 en-/decoding (no padding).</summary>
        public static Base64 UrlChars => new Base64(new CharacterDictionary("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_"), null);

        #endregion
    }
}
