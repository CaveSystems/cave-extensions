using System;
using System.IO;
using System.Text;

namespace Cave;

/// <summary>Gets abstract functions for all fixed base (32, 64, ...) conversions.</summary>
public abstract class BaseX
{
    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="BaseX" /> class.</summary>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="bitCount">The bit count.</param>
    /// <exception cref="ArgumentOutOfRangeException">Invalid dictionary length or bit count.</exception>
    protected BaseX(CharacterDictionary dictionary, int bitCount)
    {
        if (bitCount is < 1 or > 32)
        {
            throw new ArgumentOutOfRangeException(nameof(bitCount), "BitCount in range 1..32 required!");
        }

        CharacterDictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        BitsPerCharacter = bitCount;
        if (dictionary.Length != (1 << bitCount))
        {
            throw new ArgumentOutOfRangeException(nameof(dictionary), "Invalid dictionary length!");
        }
    }

    #endregion

    #region Properties

    /// <summary>Gets the bits per character.</summary>
    /// <value>The bits per character.</value>
    public int BitsPerCharacter { get; }

    /// <summary>Gets the character dictionary.</summary>
    /// <value>The character dictionary.</value>
    public CharacterDictionary CharacterDictionary { get; }

    #endregion

    #region Members

    /// <summary>Decodes a data string.</summary>
    /// <param name="value">The data to decode.</param>
    public byte[] Decode(string value) => Decode(Encoding.ASCII.GetBytes(value));

    /// <summary>Decodes the specified data.</summary>
    /// <param name="data">The data.</param>
    /// <returns>Returns the decoded data.</returns>
    public abstract byte[] Decode(byte[] data);

    /// <summary>Decodes the character.</summary>
    /// <param name="character">The character.</param>
    /// <returns>Returns the value for the specified character.</returns>
    public int DecodeCharacter(char character) => CharacterDictionary.GetValue(character);

    /// <summary>Decodes the int16.</summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public short DecodeInt16(string value) => unchecked((short)DecodeUInt32(value));

    /// <summary>Decodes the int32.</summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public int DecodeInt32(string value) => unchecked((int)DecodeUInt32(value));

    /// <summary>Decodes the int64.</summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public long DecodeInt64(string value) => unchecked((long)DecodeUInt64(value));

    /// <summary>Decodes the int8.</summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public sbyte DecodeInt8(string value) => unchecked((sbyte)DecodeUInt32(value));

    /// <summary>Decodes the u int16.</summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public ushort DecodeUInt16(string value) => unchecked((ushort)DecodeUInt32(value));

    /// <summary>Decodes the u int32.</summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public uint DecodeUInt32(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        uint result = 0;
        var bitPosition = 0;
        foreach (var character in value)
        {
            var bits = (uint)DecodeCharacter(character);
            bits <<= bitPosition;
            result |= bits;
            bitPosition += BitsPerCharacter;
        }

        return result;
    }

    /// <summary>Decodes the u int64.</summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public ulong DecodeUInt64(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        ulong result = 0;
        var bitPosition = 0;
        foreach (var character in value)
        {
            var bits = (ulong)DecodeCharacter(character);
            bits <<= bitPosition;
            result |= bits;
            bitPosition += BitsPerCharacter;
        }

        return result;
    }

    /// <summary>Decodes the u int8.</summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public byte DecodeUInt8(string value) => unchecked((byte)DecodeUInt32(value));

    /// <summary>Decodes a data string and converts the result to utf8.</summary>
    /// <param name="value">The data to decode.</param>
    public string DecodeUtf8(string value) => Encoding.UTF8.GetString(Decode(value));

    /// <summary>Decodes data and converts the result to utf8.</summary>
    /// <param name="value">The data to decode.</param>
    public string DecodeUtf8(byte[] value) => Encoding.UTF8.GetString(Decode(value));

    /// <summary>Encodes the specified value.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Returns the BaseX encoded string.</returns>
    public string Encode(byte value) => Encode((uint)value);

    /// <summary>Encodes the specified value.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Returns the BaseX encoded string.</returns>
    public string Encode(sbyte value) => unchecked(Encode((uint)value));

    /// <summary>Encodes the specified value.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Returns the BaseX encoded string.</returns>
    public string Encode(ushort value) => Encode((uint)value);

    /// <summary>Encodes the specified value.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Returns the BaseX encoded string.</returns>
    public string Encode(short value) => unchecked(Encode((uint)value));

    /// <summary>Encodes the specified value.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Returns the BaseX encoded string.</returns>
    public string Encode(int value) => unchecked(Encode((uint)value));

    /// <summary>Encodes the specified value.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Returns the BaseX encoded string.</returns>
    public string Encode(uint value)
    {
        var result = new StringBuilder();
        var bits = BitsPerCharacter;
        var mask = 0xFFFFFFFF >> (32 - bits);
        while (value > 0)
        {
            var character = value & mask;
            result.Append(EncodeCharacter((char)character));
            value >>= bits;
        }

        return result.ToString();
    }

    /// <summary>Encodes the specified value.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Returns the BaseX encoded string.</returns>
    public string Encode(long value) => unchecked(Encode((ulong)value));

    /// <summary>Encodes the specified value.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Returns the BaseX encoded string.</returns>
    public string Encode(ulong value)
    {
        var result = new StringBuilder();
        var bits = BitsPerCharacter;
        ulong mask = 0xFFFFFFFF >> (32 - bits);
        while (value > 0)
        {
            var character = value & mask;
            result.Append(EncodeCharacter((char)character));
            value >>= bits;
        }

        return result.ToString();
    }

    /// <summary>Encodes the specified string.</summary>
    /// <param name="data">The data to encode.</param>
    /// <returns>Returns the BaseX encoded string.</returns>
    public string Encode(string data) => Encode(Encoding.UTF8.GetBytes(data));

    /// <summary>Encodes the specified data.</summary>
    /// <param name="data">The data.</param>
    /// <returns>Returns the BaseX encoded string.</returns>
    public abstract string Encode(byte[] data);

    /// <summary>Encodes the specified value in range 0..X.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Returns the BaseX encoded character.</returns>
    public char EncodeCharacter(int value) => CharacterDictionary.GetCharacter(value);

    #endregion
}
