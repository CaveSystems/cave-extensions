using System.Numerics;
using System.Text;

namespace Cave;

/// <summary>Provides the an interface for all base x (1..128) conversions.</summary>
public interface IBaseX
{
    #region Public Properties

    /// <summary>Gets the character dictionary.</summary>
    /// <value>The character dictionary.</value>
    CharacterDictionary CharacterDictionary { get; }

    /// <summary>Gets the used padding after each BaseX encoded string. Use null to disable padding.</summary>
    char? Padding { get; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Decodes the specified BaseX data.</summary>
    /// <param name="baseXdata">BaseX data array</param>
    /// <returns>Returns the decoded data</returns>
    byte[] Decode(byte[] baseXdata);

    /// <summary>Decodes the specified BaseX character to a value.</summary>
    /// <param name="baseXcharacter">BaseX character value</param>
    /// <returns>Returns the decoded value</returns>
    byte DecodeCharacter(char baseXcharacter);

#if !NET20 && !NET35
    BigInteger DecodeValue(byte[] baseXdata);
#else

    long DecodeValue(byte[] baseXdata);

#endif

    /// <summary>Encodes the specified data.</summary>
    /// <param name="data">The data to encode.</param>
    /// <returns>Returns the encoded string.</returns>
    string Encode(byte[] data);

    /// <summary>Encodes the BaseX value to a single character.</summary>
    /// <param name="baseXvalue">The value to encode.</param>
    /// <returns>Returns the encoded string.</returns>
    char EncodeCharacter(int baseXvalue);

#if !NET20 && !NET35
    string EncodeValue(BigInteger value);
#else

    string EncodeValue(long value);

    string EncodeValue(ulong value);

#endif

    #endregion Public Methods
}
