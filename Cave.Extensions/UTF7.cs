using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable CA1710

namespace Cave;

/// <summary>
/// Provides a string encoded on the heap using utf7. This will reduce the memory usage by about 40-50% on most western languages /
/// ascii based character sets.
/// </summary>
public sealed class UTF7 : IEnumerable<char>, IComparable, IConvertible, IComparable<string>, IEquatable<string>, IComparable<UTF7>, IEquatable<UTF7>
{
    #region Static

    /// <summary>Provides extended UTF-7 decoding (rfc 3501)</summary>
    public static string Decode(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (!data.Contains((byte)'&'))
        {
            return ASCII.GetString(data);
        }
        var result = new StringBuilder();
        List<byte> code = null;
        for (var i = 0; i < data.Length; i++)
        {
            if (code != null)
            {
                if (data[i] == '-')
                {
                    var chunk = DecodeChunk(code.ToArray());
                    result.Append(chunk);
                    code = null;
                }
                else
                {
                    code.Add(data[i]);
                }
            }
            else
            {
                if (data[i] == '&')
                {
                    if (data[++i] == '-')
                    {
                        result.Append('&');
                    }
                    else
                    {
                        code = new()
                            { data[i] };
                    }
                }
                else
                {
                    result.Append(data[i]);
                }
            }
        }
        return result.ToString();
    }

    /// <summary>Provides extended UTF-7 encoding (rfc 3501)</summary>
    public static byte[] Encode(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }
        StringBuilder result = new();
        StringBuilder code = null;
        for (var i = 0; i < text.Length; i++)
        {
            var ch = text[i];
            var isCodeChar = ch is (< (char)0x20 or > (char)0x25) and (< (char)0x27 or > (char)0x7e);
            if (isCodeChar)
            {
                if (ch == '&')
                {
                    if (code != null)
                    {
                        var chunk = EncodeChunk(code.ToString());
                        result.Append($"&{chunk}-&-");
                        code = null;
                    }
                    else
                    {
                        result.Append("&-");
                    }
                }
                else
                {
                    if (code == null)
                    {
                        code = new();
                    }
                    code.Append(ch);
                }
            }
            else
            {
                if (code != null)
                {
                    var chunk = EncodeChunk(code.ToString());
                    result.Append($"&{chunk}-{ch}");
                    code = null;
                }
                else
                {
                    result.Append(ch);
                }
            }
        }
        if (code != null)
        {
            var chunk = EncodeChunk(code.ToString());
            result.Append("&" + chunk + "-");
        }
        return ASCII.GetBytes(result.ToString());
    }

    /// <summary>Implements the operator ==.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(UTF7 s1, UTF7 s2) => s1 is null ? s2 is null : s1.Equals(s2);

    /// <inheritdoc />
    public static bool operator >(UTF7 left, UTF7 right) => left is not null && (left.CompareTo(right) > 0);

    /// <inheritdoc />
    public static bool operator >=(UTF7 left, UTF7 right) => left is null ? right is null : left.CompareTo(right) >= 0;

    /// <summary>Performs an implicit conversion from <see cref="UTF7" /> to <see cref="string" />.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator string(UTF7 s) => s?.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string" /> to <see cref="UTF7" />.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF7(string s) => s == null ? null : new(s);

    /// <summary>Implements the operator !=.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(UTF7 s1, UTF7 s2) => s2 is null ? s1 is not null : !s1.Equals(s2);

    /// <inheritdoc />
    public static bool operator <(UTF7 left, UTF7 right) => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <inheritdoc />
    public static bool operator <=(UTF7 left, UTF7 right) => left is null || (left.CompareTo(right) <= 0);

    static string DecodeChunk(byte[] code)
    {
        var data = Base64.NoPadding.Decode(code);
        return Encoding.BigEndianUnicode.GetString(data);
    }

    static string EncodeChunk(string text)
    {
        var data = Encoding.BigEndianUnicode.GetBytes(text);
        return Base64.NoPadding.Encode(data);
    }

    #endregion

    #region Constructors

    /// <summary>Creates a new instance of the <see cref="UTF7" /> class.</summary>
    /// <param name="data">Content</param>
    public UTF7(byte[] data) => Data = data;

    /// <summary>Creates a new instance of the <see cref="UTF7" /> class.</summary>
    /// <param name="text">Content</param>
    public UTF7(string text) => Data = Encode(text);

    #endregion

    #region Properties

    /// <summary>Gets the data bytes.</summary>
    public byte[] Data { get; }

    /// <summary>Gets the length.</summary>
    /// <value>The length of the string.</value>
    public int Length => ToString().Length;

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

    #region IComparable<UTF7> Members

    /// <inheritdoc />
    public int CompareTo(UTF7 other) => ToString().CompareTo(other?.ToString());

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

    #region IEquatable<UTF7> Members

    /// <inheritdoc />
    public bool Equals(UTF7 other) => (Length == other.Length) && Data.SequenceEqual(other.Data);

    #endregion

    #region Overrides

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is UTF7 utf7 && Equals(utf7);

    /// <inheritdoc />
    public override int GetHashCode() => DefaultHashingFunction.Calculate(Data);

    /// <inheritdoc />
    public override string ToString() => Decode(Data);

    #endregion
}
