using System;
using System.Collections.Generic;

namespace Cave;

/// <summary>Gets a ascii character dictionary (this is used for example at the <see cref="Base64"/> implementation).</summary>
public sealed class CharacterDictionary
{
    #region Private Fields

    readonly char[] charset;
    readonly byte[] values = new byte[128];

    #endregion Private Fields

    #region Private Constructors

    CharacterDictionary(CharacterDictionary cloneData)
    {
        charset = (char[])cloneData.charset.Clone();
        values = (byte[])cloneData.values.Clone();
    }

    #endregion Private Constructors

    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="CharacterDictionary"/> class.</summary>
    /// <param name="charset">Characters to use as charset.</param>
    /// <param name="obeyCasing">Obey character case</param>
    public CharacterDictionary(string charset, bool obeyCasing)
    {
        ObeyCasing = obeyCasing;
        if (charset == null)
        {
            throw new ArgumentNullException(nameof(charset));
        }

        if (charset.Length > 128)
        {
            throw new ArgumentOutOfRangeException(nameof(charset), "Less than or equal to 128 characters expected!");
        }

        for (var i = 0; i < 128; i++)
        {
            values[i] = 0xFF;
        }

        this.charset = charset.ToCharArray();
        for (var i = 0; i < this.charset.Length; i++)
        {
            values[this.charset[i]] = (byte)i;
        }
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the length.</summary>
    /// <value>The length.</value>
    public int Length => charset.Length;

    /// <summary>Gets a value indicating whether the character case has to be obeyed.</summary>
    public bool ObeyCasing { get; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Clones the <see cref="CharacterDictionary"/>.</summary>
    /// <returns>Returns a copy.</returns>
    public CharacterDictionary Clone() => new(this);

    /// <summary>Gets the character for the specified value.</summary>
    /// <param name="value">The value to look up.</param>
    /// <returns>Returns the character for the value.</returns>
    public char GetCharacter(int value) => charset[value];

    /// <summary>Gets a copy of all valid characters.</summary>
    /// <returns>Returns a new array of char</returns>
    public char[] GetValidChars() => (char[])charset.Clone();

    /// <summary>Gets the value for the specified character.</summary>
    /// <param name="character">The <see cref="char"/> to look up.</param>
    /// <returns>Returns the value (index) for the char.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the symbol could not be found.</exception>
    public byte GetValue(char character)
    {
        var result = values[character];
        if (result > 128 && !ObeyCasing)
        {
            result = char.IsUpper(character) ? values[char.ToLowerInvariant(character)] : values[char.ToUpperInvariant(character)];
        }
        if (result > 128)
        {
            throw new KeyNotFoundException($"Invalid symbol '{character}'!");
        }
        return result;
    }

    /// <summary>Checks whether an encoded character has a value.</summary>
    /// <param name="character">Character to check</param>
    /// <returns>Returns true is the character is valie, false otherwise</returns>
    public bool HasValue(char character)
    {
        if (character < 0 || character > 128) return false;
        var result = values[character];
        if (result > 128 && !ObeyCasing)
        {
            result = char.IsUpper(character) ? values[char.ToLowerInvariant(character)] : values[char.ToUpperInvariant(character)];
        }
        return (result <= 128);
    }

    /// <summary>Tries to get the value for the given character. If no values is available defaultValue will be returned.</summary>
    /// <param name="character">The character.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns></returns>
    public int TryGetValue(char character, int defaultValue)
    {
        if (character >= values.Length)
        {
            return defaultValue;
        }

        var result = values[character];
        if (result > 128 && !ObeyCasing)
        {
            result = char.IsUpper(character) ? values[char.ToLowerInvariant(character)] : values[char.ToUpperInvariant(character)];
        }
        return result > 128 ? defaultValue : result;
    }

    #endregion Public Methods
}
