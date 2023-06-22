using System;

#nullable enable

#pragma warning disable CA1710

namespace Cave;

/// <summary>Provides an unicode string interface.</summary>
public interface IUnicode : IComparable, IComparable<IUnicode>, IEquatable<IUnicode>
{
    #region Public Properties

    /// <summary>Gets the unicode codepoints.</summary>
    int[] Codepoints { get; }

    /// <summary>Gets the data bytes.</summary>
    byte[] Data { get; }

    #endregion Public Properties
}

/// <summary>Provides an unicode string interface.</summary>
public interface IUnicode<TUnicode> : IUnicode, IEquatable<string>, IComparable<string>
    where TUnicode : IUnicode
{
    #region Public Methods

    /// <summary>Creates the <see cref="IUnicode"/> instance from the specified <paramref name="text"/> by ignoring all errors and incomplete/invalid characters.</summary>
    /// <param name="text">Csharp string instance</param>
    /// <returns>Returns a new <see cref="IUnicode"/> instance</returns>
    TUnicode FromString(string text);

    /// <summary>
    /// Creates the <see cref="IUnicode"/> instance from the specified <paramref name="codepoints"/> by ignoring all errors and incomplete/invalid characters.
    /// </summary>
    /// <param name="codepoints">Codepoints</param>
    /// <param name="start">Start at the codepoint array</param>
    /// <param name="length">Number of entries from the codepoint array</param>
    /// <returns>Returns a new <see cref="IUnicode"/> instance</returns>
    TUnicode FromCodepoints(int[] codepoints, int start = 0, int length = -1);

    /// <summary>Creates the <see cref="IUnicode"/> instance from the specified <paramref name="data"/> by ignoring all errors and incomplete/invalid characters.</summary>
    /// <param name="data">Data to parse</param>
    /// <param name="start">Start at the data array</param>
    /// <param name="length">Number of bytes from the byte array</param>
    /// <returns>Returns a new <see cref="IUnicode"/> instance</returns>
    TUnicode FromArray(byte[] data, int start = 0, int length = -1);

    /// <summary>Retrieves a substring from this instance.</summary>
    /// <param name="start">Startposition at the <see cref="IUnicode.Codepoints"/>. (Optional, default: start at first codepoint)</param>
    /// <param name="length">Number of codepoints. (Optional, default: all until end of string)</param>
    /// <returns></returns>
    TUnicode Substring(int start = 0, int length = -1);

    /// <summary>Concatenates this instance with the specified instance.</summary>
    /// <param name="other">Second part of the new instance.</param>
    /// <returns>Returns a new instance containing the concatenation.</returns>
    TUnicode Concat(TUnicode other);

    #endregion Public Methods
}
