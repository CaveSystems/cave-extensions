using System;
using System.Collections.Generic;

#pragma warning disable CA1710

namespace Cave;

/// <summary>
/// Provides an unicode string interface.
/// </summary>
public interface IUnicode : IEnumerable<char>, IEnumerable<int>, IComparable, IConvertible, IComparable<string>, IEquatable<string>
{
    /// <summary>Gets the data bytes.</summary>
    byte[] Data { get; }

    /// <summary>Gets the unicode codepoints.</summary>
    int[] Codepoints { get; }

    /// <summary>Gets the string length or csharp character count.</summary>
    /// <remarks>
    /// This is not the number of unicode characters since csharp uses utf16 with high and low surrogate pairs to represent non BMP (Basic
    /// Multilingual Plane) characters.
    /// </remarks>
    int Length { get; }
}
