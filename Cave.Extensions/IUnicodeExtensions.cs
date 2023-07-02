#nullable enable

#pragma warning disable CA1710

namespace Cave;

/// <summary>Provides extensions to the <see cref="IUnicode"/> interface.</summary>
public static class IUnicodeExtensions
{
    /// <summary>Checks whether the unicode instance starts with a byte order mark or not.</summary>
    /// <param name="unicode">Unicode instance.</param>
    /// <returns>Returns true if the byte order mark is present, false otherwise.</returns>
    public static bool StartsWithByteOrderMark(this IUnicode unicode) => unicode.Data.StartsWith(unicode.ByteOrderMark);

    /// <summary>
    /// Returns an instance starting with the byte order mark. If no byte order mark is present a new instance with bom will be returned, otherwise the original
    /// instance will be returned.
    /// </summary>
    /// <param name="unicode">Unicode instance.</param>
    /// <returns>Returns an instance starting with the byte order mark.</returns>
    public static IUnicode AddByteOrderMark(this IUnicode unicode) => unicode.StartsWithByteOrderMark() ? unicode : unicode.FromArray(unicode.ByteOrderMark.Concat(unicode.Data));

    /// <summary>
    /// Returns an instance without byte order mark. If a byte order mark is present a new instance without bom will be returned, otherwise the original
    /// instance will be returned.
    /// </summary>
    /// <param name="unicode">Unicode instance.</param>
    /// <returns>Returns an instance without byte order mark.</returns>
    public static IUnicode RemoveByteOrderMark(this IUnicode unicode) => !unicode.StartsWithByteOrderMark() ? unicode : unicode.FromArray(unicode.Data, unicode.ByteOrderMark.Length);
}
