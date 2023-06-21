using System;

#pragma warning disable CA1710

namespace Cave;

/// <summary>Provides an unicode string interface.</summary>
public interface IUnicode : IComparable, IComparable<string>, IComparable<IUnicode>, IEquatable<string>, IEquatable<IUnicode>
{
    #region Public Properties

    /// <summary>Gets the unicode codepoints.</summary>
    int[] Codepoints { get; }

    /// <summary>Gets the data bytes.</summary>
    byte[] Data { get; }

    /// <summary>Gets the string length or csharp character count.</summary>
    /// <remarks>
    /// This is not the number of unicode characters since csharp uses utf16 with high and low surrogate pairs to represent non BMP (Basic Multilingual Plane) characters.
    /// </remarks>
    int Length { get; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Concatenates this instance with the specified string and returns the result.</summary>
    /// <param name="text">Text to add.</param>
    /// <returns>Returns a new instance containing the result of the concatenation.</returns>
    IUnicode Concat(string text);

    #endregion Public Methods
}
