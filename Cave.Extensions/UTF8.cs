using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable CA1710

namespace Cave;

/// <summary>
/// Provides a string encoded on the heap using utf8. This will reduce the memory usage by about 40-50% on most western languages /
/// ascii based character sets.
/// </summary>
#if NETSTANDARD2_0_OR_GREATER || NET20_OR_GREATER || NET5_0_OR_GREATER
public sealed class UTF8 : IEnumerable<char>, IEnumerable, ICloneable, IComparable, IConvertible, IComparable<string>, IEquatable<string>, IComparable<UTF8>, IEquatable<UTF8>
#else
public sealed class UTF8 : IComparable, IComparable<string>, IEquatable<string>, IComparable<UTF8>, IEquatable<UTF8>
#endif
{
    /// <summary>Performs an implicit conversion from <see cref="UTF8" /> to <see cref="string" />.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator string(UTF8 s) => s?.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string" /> to <see cref="UTF8" />.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF8(string s) => s == null ? null : new(s);

    /// <summary>Implements the operator ==.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(UTF8 s1, UTF8 s2) => s1 is null ? s2 is null : s1.Equals(s2);

    /// <summary>Implements the operator !=.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(UTF8 s1, UTF8 s2) => s2 is null ? s1 is not null : !s1.Equals(s2);

    UTF8(byte[] data, int length)
    {
        Length = length;
        this.data = data;
    }

    /// <summary>Creates a new instance of the <see cref="UTF8" /> class.</summary>
    /// <param name="data">Content</param>
    public UTF8(byte[] data) : this((byte[])data.Clone(), Encoding.UTF8.GetCharCount(data)) { }

    /// <summary>Creates a new instance of the <see cref="UTF8" /> class.</summary>
    /// <param name="text">Content</param>
    public UTF8(string text)
    {
        Length = text.Length;
        data = Encoding.UTF8.GetBytes(text);
    }

    readonly byte[] data;

    /// <summary>Gets the data bytes.</summary>
    public IList<byte> Data => data.AsReadOnly();

    /// <summary>Gets the length.</summary>
    /// <value>The length of the string.</value>
    public int Length { get; }

    /// <inheritdoc />
    public override string ToString() => Encoding.UTF8.GetString(data);

    /// <inheritdoc />
    public override int GetHashCode() => ToString().GetHashCode();

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is UTF8 utf8 && Equals(utf8);

    /// <inheritdoc />
    public bool Equals(UTF8 other) => (Length == other.Length) && data.SequenceEqual(other.data);

    /// <inheritdoc />
    public int CompareTo(object obj)
    {
        if (null == obj)
        {
            return 1;
        }
        return ToString().CompareTo(obj.ToString());
    }

#if NETSTANDARD2_0_OR_GREATER || NET20_OR_GREATER || NET5_0_OR_GREATER
    /// <inheritdoc />
    public IEnumerator<char> GetEnumerator() => ToString().GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ToString().GetEnumerator();

    /// <inheritdoc />
    public TypeCode GetTypeCode() => ToString().GetTypeCode();
#endif

    /// <inheritdoc />
    public object Clone() => new UTF8(data, Length);

    /// <inheritdoc />
    public bool ToBoolean(IFormatProvider provider) => ((IConvertible)ToString()).ToBoolean(provider);

    /// <inheritdoc />
    public byte ToByte(IFormatProvider provider) => ((IConvertible)ToString()).ToByte(provider);

    /// <inheritdoc />
    public char ToChar(IFormatProvider provider) => ((IConvertible)ToString()).ToChar(provider);

    /// <inheritdoc />
    public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)ToString()).ToDateTime(provider);

    /// <inheritdoc />
    public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)ToString()).ToDecimal(provider);

    /// <inheritdoc />
    public double ToDouble(IFormatProvider provider) => ((IConvertible)ToString()).ToDouble(provider);

    /// <inheritdoc />
    public short ToInt16(IFormatProvider provider) => ((IConvertible)ToString()).ToInt16(provider);

    /// <inheritdoc />
    public int ToInt32(IFormatProvider provider) => ((IConvertible)ToString()).ToInt32(provider);

    /// <inheritdoc />
    public long ToInt64(IFormatProvider provider) => ((IConvertible)ToString()).ToInt64(provider);

    /// <inheritdoc />
    public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)ToString()).ToSByte(provider);

    /// <inheritdoc />
    public float ToSingle(IFormatProvider provider) => ((IConvertible)ToString()).ToSingle(provider);

    /// <inheritdoc />
    public string ToString(IFormatProvider provider) => ToString();

    /// <inheritdoc />
    public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)ToString()).ToType(conversionType, provider);

    /// <inheritdoc />
    public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)ToString()).ToUInt16(provider);

    /// <inheritdoc />
    public uint ToUInt32(IFormatProvider provider) => ((IConvertible)ToString()).ToUInt32(provider);

    /// <inheritdoc />
    public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)ToString()).ToUInt64(provider);

    /// <inheritdoc />
    public int CompareTo(string other) => ToString().CompareTo(other);

    /// <inheritdoc />
    public bool Equals(string other) => ToString().Equals(other);

    /// <inheritdoc />
    public int CompareTo(UTF8 other) => ToString().CompareTo(other?.ToString());

    /// <inheritdoc />
    public static bool operator <(UTF8 left, UTF8 right) => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <inheritdoc />
    public static bool operator <=(UTF8 left, UTF8 right) => left is null || (left.CompareTo(right) <= 0);

    /// <inheritdoc />
    public static bool operator >(UTF8 left, UTF8 right) => left is not null && (left.CompareTo(right) > 0);

    /// <inheritdoc />
    public static bool operator >=(UTF8 left, UTF8 right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
