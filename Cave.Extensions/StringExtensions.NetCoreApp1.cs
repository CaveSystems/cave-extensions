#if NETCOREAPP1_0 || NETCOREAPP1_1 || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Cave;

partial class StringExtensions
{
    /// <summary><see cref="char.ToLower(char)"/></summary>
    /// <param name="c">Character</param>
    /// <param name="culture">Culture to use</param>
    /// <returns>Returns the lowercase character</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static char ToLower(this char c, CultureInfo culture) => char.ToLower(c);

    /// <summary><see cref="char.ToUpper(char)"/></summary>
    /// <param name="c">Character</param>
    /// <param name="culture">Culture to use</param>
    /// <returns>Returns the uppercase character</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static char ToUpper(this char c, CultureInfo culture) => char.ToUpper(c);

    /// <summary><see cref="String.ToLower()"/></summary>
    /// <param name="s">String</param>
    /// <param name="culture">Culture to use</param>
    /// <returns>Returns the lowercase character</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ToLower(this string s, CultureInfo culture) => s.ToLower(culture);

    /// <summary><see cref="String.ToUpper()"/></summary>
    /// <param name="s">String</param>
    /// <param name="culture">Culture to use</param>
    /// <returns>Returns the uppercase character</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ToUpper(this string s, CultureInfo culture) => s.ToUpper(culture);
}
#endif
