using System.Collections.Generic;
using System.Text;
using Cave.Collections;

#nullable enable

#pragma warning disable CA1710

namespace Cave;

/// <summary>Provides unicode base implementations</summary>
public abstract class Unicode : IUnicode
{
    /// <summary>Implements the operator !=.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(Unicode s1, Unicode s2) => s2 is null ? s1 is not null : !s1.Equals(s2);

    /// <inheritdoc/>
    public static bool operator <(Unicode left, Unicode right) => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator <=(Unicode left, Unicode right) => left is null || (left.CompareTo(right) <= 0);

    /// <summary>Implements the operator ==.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(Unicode s1, Unicode s2) => s1 is null ? s2 is null : s1.Equals(s2);

    /// <inheritdoc/>
    public static bool operator >(Unicode left, Unicode right) => left is not null && (left.CompareTo(right) > 0);

    /// <inheritdoc/>
    public static bool operator >=(Unicode left, Unicode right) => left is null ? right is null : left.CompareTo(right) >= 0;

    /// <summary>Creates a new empty instance.</summary>
    public Unicode() => Data = ArrayExtension.Empty<byte>();

    /// <summary>Creates a new instance</summary>
    /// <param name="data">Content</param>
    public Unicode(byte[] data) => Data = data;

    /// <summary>Gets the unicode codepoints.</summary>
    public abstract int[] Codepoints { get; }

    /// <summary>Gets the data bytes.</summary>
    public byte[] Data { get; }

    /// <inheritdoc/>
    public int CompareTo(object? obj) => obj is not IUnicode unicode ? 1 : CompareTo(unicode);

    /// <inheritdoc/>
    public int CompareTo(IUnicode? other) => other is null ? 1 : DefaultComparer.Combine(Codepoints, other.Codepoints);

    /// <inheritdoc/>
    public bool Equals(IUnicode? other) => other is not null && DefaultComparer.Equals(Codepoints, other.Codepoints);

    /// <inheritdoc/>
    public override int GetHashCode() => DefaultHashingFunction.Calculate(Data);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is IUnicode unicode && Equals(unicode);

    /// <summary>Converts the specified codepoints to a new string.</summary>
    /// <param name="codepoints">Unicode codepoints</param>
    /// <returns>Returns a new string instance.</returns>
    public static string ToString(int[] codepoints)
    {
        var sb = new StringBuilder(codepoints.Length * 2);
        for (var i = 0; i < codepoints.Length; i++)
        {
            _ = sb.Append(char.ConvertFromUtf32(codepoints[i]));
        }
        return sb.ToString();
    }

    /// <inheritdoc/>
    public override string ToString() => ToString(Codepoints);

    /// <inheritdoc/>
    public IEnumerator<char> GetEnumerator() => ((IEnumerable<char>)ToString()).GetEnumerator();
}

/// <summary>Provides unicode base implementations</summary>
/// <typeparam name="TUnicode"></typeparam>
public abstract class Unicode<TUnicode> : Unicode, IUnicode<TUnicode>
    where TUnicode : IUnicode
{
    /// <summary>Creates a new empty instance.</summary>
    public Unicode() : base() { }

    /// <summary>Creates a new instance</summary>
    /// <param name="data">Content</param>
    public Unicode(byte[] data) : base(data) { }

    /// <inheritdoc/>
    public abstract TUnicode FromString(string text);

    /// <inheritdoc/>
    public int CompareTo(string? other) => other is null ? 1 : CompareTo(FromString(other));

    /// <inheritdoc/>
    public bool Equals(string? other) => other is not null && Equals(FromString(other));

    /// <inheritdoc/>
    public abstract TUnicode FromCodepoints(int[] codepoints, int start = 0, int length = -1);

    /// <inheritdoc/>
    public abstract TUnicode FromArray(byte[] data, int start = 0, int length = -1);

    /// <inheritdoc/>
    public TUnicode Substring(int start = 0, int length = -1) => FromCodepoints(Codepoints, start, length);

    /// <inheritdoc/>
    public TUnicode Concat(TUnicode other) => FromCodepoints(Codepoints.Concat(other.Codepoints));
}
