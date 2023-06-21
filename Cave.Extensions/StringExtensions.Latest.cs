#if !(NETCOREAPP1_0 || NETCOREAPP1_1 || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER))

using System.Globalization;
using System.Runtime.CompilerServices;

namespace Cave;

partial class StringExtensions
{
    #region Public Methods

    /// <summary><see cref="char.ToLower(char, CultureInfo)"/></summary>
    /// <param name="c">Character</param>
    /// <param name="culture">Culture to use</param>
    /// <returns>Returns the lowercase character</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static char ToLower(this char c, CultureInfo culture) => char.ToLower(c, culture);

    /// <summary><see cref="char.ToUpper(char, CultureInfo)"/></summary>
    /// <param name="c">Character</param>
    /// <param name="culture">Culture to use</param>
    /// <returns>Returns the uppercase character</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static char ToUpper(this char c, CultureInfo culture) => char.ToUpper(c, culture);

    #endregion Public Methods
}

#endif
