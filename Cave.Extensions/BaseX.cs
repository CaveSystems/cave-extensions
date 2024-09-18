using System;
using System.Numerics;

namespace Cave;

/// <summary>Provides abstract functions for all base x (1..128) conversions.</summary>
public abstract class BaseX : IBaseX
{
    #region Public Constructors

    /// <summary>Creates a new instance of the <see cref="BaseX"/> class.</summary>
    /// <param name="characters">Character set</param>
    /// <param name="obeyCasing">Obey case during decoding</param>
    /// <param name="padding">Padding character to use</param>
    public BaseX(string characters, bool obeyCasing, char? padding) : this(new CharacterDictionary(characters, obeyCasing), padding) { }

    /// <summary>Creates a new instance of the <see cref="BaseX"/> class.</summary>
    /// <param name="characterDictionary">Character set</param>
    /// <param name="padding">Padding character to use</param>
    public BaseX(CharacterDictionary characterDictionary, char? padding)
    {
        CharacterDictionary = characterDictionary;
        if (padding != null)
        {
            int paddingChar = (char)padding;
            if (paddingChar is < 0 or > 127)
            {
                throw new ArgumentOutOfRangeException(nameof(padding));
            }
            Padding = padding;
        }
    }

    #endregion Public Constructors

    #region Public Properties

    /// <inheritdoc/>
    public CharacterDictionary CharacterDictionary { get; }

    /// <inheritdoc/>
    public char? Padding { get; }

    #endregion Public Properties

    #region Public Methods

    /// <inheritdoc/>
    public abstract byte[] Decode(byte[] baseXdata);

    /// <inheritdoc/>
    public byte DecodeCharacter(char baseXcharacter) => CharacterDictionary.GetValue(baseXcharacter);

#if !NET20 && !NET35

    /// <inheritdoc/>
    public virtual BigInteger DecodeBigInteger(byte[] baseXdata)
    {
        var data = Decode(baseXdata);
        if (!BitConverter.IsLittleEndian) Array.Reverse(data);
        return new BigInteger(data);
    }

#endif

    /// <inheritdoc/>
    public virtual long DecodeValue(byte[] baseXdata)
    {
        var data = Decode(baseXdata);
        if (!BitConverter.IsLittleEndian) Array.Reverse(data);
        var neg = (data[^1] & 0x80) != 0;
        var offset = data.Length;
        Array.Resize(ref data, 8);
        if (neg) while (offset < data.Length) data[offset++] = 0xFF;
        return BitConverter.ToInt64(data, 0);
    }

    /// <inheritdoc/>
    public abstract string Encode(byte[] data);

    /// <inheritdoc/>
    public char EncodeCharacter(int baseXvalue) => CharacterDictionary.GetCharacter(baseXvalue);

#if !NET20 && !NET35

    /// <inheritdoc/>
    public virtual string EncodeBigInteger(BigInteger value)
    {
        var data = value.ToByteArray();
        if (!BitConverter.IsLittleEndian) Array.Reverse(data);
        return Encode(data);
    }
#endif

    /// <inheritdoc/>
    public virtual string EncodeValue(ulong value) => EncodeValue((long)value);

    /// <inheritdoc/>
    public virtual string EncodeValue(long value)
    {
        var data = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian) Array.Reverse(data);
        var length = data.Length;
        if (value < 0)
        {
            while (length > 1 && (data[length - 2] & 0x80) != 0 && data[length - 1] == 0xff) length--;
        }
        else
        {
            while (length > 1 && (data[length - 2] & 0x80) == 0 && data[length - 1] == 0) length--;
        }
        return Encode(data.GetRange(0, length));
    }

    #endregion Public Methods
}
