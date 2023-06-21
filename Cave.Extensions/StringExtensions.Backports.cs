#if !NET5_0_OR_GREATER

using System.Runtime.CompilerServices;

namespace Cave;

partial class StringExtensions
{
    #region Public Methods

    /// <summary>Returns a value indicating whether a specified character occurs within this string.</summary>
    /// <param name="text">The text.</param>
    /// <param name="c">The character to seek.</param>
    /// <remarks>This method performs an ordinal (case-sensitive and culture-insensitive) comparison.</remarks>
    /// <returns>true if the value parameter occurs within this string; otherwise, false.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static bool Contains(this string text, char c) => text.IndexOf(c) >= 0;

    #endregion Public Methods
}

#endif
