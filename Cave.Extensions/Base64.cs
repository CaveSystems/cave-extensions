using System;
using System.Collections.Generic;

namespace Cave
{
    /// <summary>
    /// Provides Base64 en-/decoding
    /// </summary>
    public class Base64 : BaseX
    {
        #region public static default instances
        /// <summary>
        /// Provides the default charset for base64 en-/decoding with padding
        /// </summary>
        public static Base64 Default
        {
            get
            {
                return new Base64(new CharacterDictionary("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"), '=');
            }
        }

        /// <summary>
        /// Provides the default charset for base64 en-/decoding without padding
        /// </summary>
        public static Base64 NoPadding
        {
            get
            {
                return new Base64(new CharacterDictionary("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"), null);
            }
        }

        /// <summary>
        /// Provides the url safe charset for base64 en-/decoding (no padding)
        /// </summary>
        public static Base64 UrlChars
        {
            get
            {
                return new Base64(new CharacterDictionary("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_"), null);
            }
        }

        #endregion

        char? m_Padding;
        const int BitCount = 6;

        /// <summary>Initializes a new instance of the <see cref="Base64"/> class.</summary>
        /// <param name="dict">The dictionary containing 64 ascii characters used for encoding.</param>
        /// <param name="pad">The padding (use null to skip padding).</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Base64(CharacterDictionary dict, char? pad) : base(dict, BitCount)
        {
            m_Padding = pad;
            if (m_Padding != null)
            {
                int paddingChar = (char)m_Padding;
                if ((paddingChar < 0) || (paddingChar > 127))
                {
                    throw new ArgumentException(string.Format("Invalid padding character!"), nameof(m_Padding));
                }
            }
        }

        #region public decoder interface

        /// <summary>
        /// Decodes a base64 data array
        /// </summary>
        /// <param name="data">The base64 data to decode</param>
        public override byte[] Decode(byte[] data)
        {
            if (m_Padding != null)
            {
                int paddingChar = (char)m_Padding;
                if ((paddingChar < 0) || (paddingChar > 127))
                {
                    throw new ArgumentException(string.Format("Invalid padding character!"), nameof(m_Padding));
                }
            }
            //decode data
            List<byte> result = new List<byte>(data.Length);
            int value = 0;
            int bits = 0;
            foreach (byte b in data)
            {
                if (b == m_Padding)
                {
                    break;
                }

                value <<= BitCount;
                bits += BitCount;
                value |= CharacterDictionary.GetValue((char)b);
                if (bits >= 8)
                {
                    bits -= 8;
                    int l_Out = value >> bits;
                    value = value & ~(0xFFFF << bits);
                    result.Add((byte)l_Out);
                }
            }
            return result.ToArray();
        }
        #endregion

        #region public encoder interface

        /// <summary>
        /// Encodes the specified data
        /// </summary>
        /// <param name="data">The data to encode</param>
        public override string Encode(byte[] data)
        {
            List<char> result = new List<char>(data.Length * 2);
            int value = 0;
            int bits = 0;
            foreach (byte b in data)
            {
                value = (value << 8) | b;
                bits += 8;
                while (bits >= BitCount)
                {
                    bits -= BitCount;
                    int outValue = value >> bits;
                    value = value & ~(0xFFFF << bits);
                    result.Add(CharacterDictionary.GetCharacter(outValue));
                }
            }
            if (bits > BitCount)
            {
                bits -= BitCount;
                int outValue = value >> bits;
                value = value & ~(0xFFFF << bits);
                result.Add(CharacterDictionary.GetCharacter(outValue));
            }
            if (bits > 0)
            {
                int shift = BitCount - bits;
                int outValue = value << shift;
                result.Add(CharacterDictionary.GetCharacter(outValue));
                bits -= BitCount;
            }
            if (m_Padding != null)
            {
                char padding = (char)m_Padding;
                while (bits % 8 != 0)
                {
                    result.Add(padding);
                    bits -= BitCount;
                }
            }
            return new string(result.ToArray());
        }
        #endregion
    }
}
