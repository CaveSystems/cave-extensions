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
public sealed class UTF8 : IEnumerable<char>, IComparable, IConvertible, IComparable<string>, IEquatable<string>, IComparable<UTF8>, IEquatable<UTF8>
#if !NETCOREAPP1_0_OR_GREATER && !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
, ICloneable
#endif
{
    #region Static

    /// <summary>Implements the operator ==.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(UTF8 s1, UTF8 s2) => s1 is null ? s2 is null : s1.Equals(s2);

    /// <inheritdoc />
    public static bool operator >(UTF8 left, UTF8 right) => left is not null && (left.CompareTo(right) > 0);

    /// <inheritdoc />
    public static bool operator >=(UTF8 left, UTF8 right) => left is null ? right is null : left.CompareTo(right) >= 0;

    /// <summary>Performs an implicit conversion from <see cref="UTF8" /> to <see cref="string" />.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator string(UTF8 s) => s?.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string" /> to <see cref="UTF8" />.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF8(string s) => s == null ? null : new(s);

    /// <summary>Implements the operator !=.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(UTF8 s1, UTF8 s2) => s2 is null ? s1 is not null : !s1.Equals(s2);

    /// <inheritdoc />
    public static bool operator <(UTF8 left, UTF8 right) => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <inheritdoc />
    public static bool operator <=(UTF8 left, UTF8 right) => left is null || (left.CompareTo(right) <= 0);

    #endregion

    #region Fields

    readonly byte[] data;

    #endregion

    #region Constructors

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

    #endregion

    #region Properties

    /// <summary>Gets the data bytes.</summary>
    public IList<byte> Data => data.AsReadOnly();

    /// <summary>Gets the length.</summary>
    /// <value>The length of the string.</value>
    public int Length { get; }

    #endregion

    #region ICloneable Members

    /// <inheritdoc />
    public object Clone() => new UTF8(data, Length);

    #endregion

    #region IComparable Members

    /// <inheritdoc />
    public int CompareTo(object obj)
    {
        if (null == obj)
        {
            return 1;
        }
        return ToString().CompareTo(obj.ToString());
    }

    #endregion

    #region IComparable<string> Members

    /// <inheritdoc />
    public int CompareTo(string other) => ToString().CompareTo(other);

    #endregion

    #region IComparable<UTF8> Members

    /// <inheritdoc />
    public int CompareTo(UTF8 other) => ToString().CompareTo(other?.ToString());

    #endregion

    #region IConvertible Members

    /// <inheritdoc />
    public TypeCode GetTypeCode() => TypeCode.String;

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

    #endregion

    #region IEnumerable<char> Members

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

#if NETCOREAPP1_0 || NETCOREAPP1_1 || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
    /// <inheritdoc />
    public IEnumerator<char> GetEnumerator() => ((IEnumerable<char>)ToString()).GetEnumerator();
#else
    /// <inheritdoc />
    public IEnumerator<char> GetEnumerator() => ToString().GetEnumerator();
#endif

    #endregion

    #region IEquatable<string> Members

    /// <inheritdoc />
    public bool Equals(string other) => ToString().Equals(other);

    #endregion

    #region IEquatable<UTF8> Members

    /// <inheritdoc />
    public bool Equals(UTF8 other) => (Length == other.Length) && data.SequenceEqual(other.data);

    #endregion

    #region Overrides

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is UTF8 utf8 && Equals(utf8);

    /// <inheritdoc />
    public override int GetHashCode() => ToString().GetHashCode();

    /// <inheritdoc />
    public override string ToString() => Encoding.UTF8.GetString(data);

    #endregion
}
