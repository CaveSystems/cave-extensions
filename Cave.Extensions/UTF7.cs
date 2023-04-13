using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cave.Text
{
    /// <summary>
    /// Provides a string encoded on the heap using utf7. This will reduce the memory usage by about 40-50% on most western languages / ascii based
    /// character sets.
    /// </summary>
#if NETSTANDARD2_0_OR_GREATER || NET20_OR_GREATER || NET5_0_OR_GREATER
    public sealed class UTF7 : IEnumerable<char>, IEnumerable, IComparable, IConvertible, IComparable<string>, IEquatable<string>, IComparable<UTF7>, IEquatable<UTF7>
#else
    public sealed class UTF7 : IComparable, IComparable<string>, IEquatable<string>, IComparable<UTF7>, IEquatable<UTF7>
#endif
    {
        /// <summary>Performs an implicit conversion from <see cref="UTF7"/> to <see cref="string"/>.</summary>
        /// <param name="s">The string.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(UTF7 s) => s?.ToString();

        /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="UTF7"/>.</summary>
        /// <param name="s">The string.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator UTF7(string s) => s == null ? null : new(s);

        /// <summary>Implements the operator ==.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(UTF7 s1, UTF7 s2) => s1 is null ? s2 is null : s1.Equals(s2);

        /// <summary>Implements the operator !=.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(UTF7 s1, UTF7 s2) => s2 is null ? s1 is not null : !s1.Equals(s2);

        private UTF7(byte[] data, int length)
        {
            Length = length;
            this.data = data;
        }

        /// <summary>Creates a new instance of the <see cref="UTF8"/> class.</summary>
        /// <param name="data">Content</param>
        public UTF7(byte[] data) : this((byte[])data.Clone(), Encoding.UTF8.GetCharCount(data)) { }

        /// <summary>Creates a new instance of the <see cref="UTF8"/> class.</summary>
        /// <param name="text">Content</param>
        public UTF7(string text)
        {
            Length = text.Length;
            data = EncodeExtendedUTF7(text);
        }

        readonly byte[] data;

        /// <summary>Gets the length.</summary>
        /// <value>The length of the string.</value>
        public int Length { get; private set; }

        /// <inheritdoc/>
        public override string ToString() => Encoding.UTF8.GetString(data);

        /// <inheritdoc/>
        public override int GetHashCode() => ToString().GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is UTF7 utf7 && Equals(utf7);

        /// <inheritdoc/>
        public bool Equals(UTF7 other) => Length == other.Length && data.SequenceEqual(other.data);

        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            if (null == obj) return 1;
            return ToString().CompareTo(obj.ToString());
        }

#if NETSTANDARD2_0_OR_GREATER || NET20_OR_GREATER || NET5_0_OR_GREATER

        /// <inheritdoc/>
        public IEnumerator<char> GetEnumerator() => ToString().GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ToString().GetEnumerator();

        /// <inheritdoc/>
        public TypeCode GetTypeCode() => ToString().GetTypeCode();
#endif

        /// <inheritdoc/>
        public object Clone() => new UTF7(data, Length);

        /// <inheritdoc/>
        public bool ToBoolean(IFormatProvider provider) => ((IConvertible)ToString()).ToBoolean(provider);

        /// <inheritdoc/>
        public byte ToByte(IFormatProvider provider) => ((IConvertible)ToString()).ToByte(provider);

        /// <inheritdoc/>
        public char ToChar(IFormatProvider provider) => ((IConvertible)ToString()).ToChar(provider);

        /// <inheritdoc/>
        public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)ToString()).ToDateTime(provider);

        /// <inheritdoc/>
        public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)ToString()).ToDecimal(provider);

        /// <inheritdoc/>
        public double ToDouble(IFormatProvider provider) => ((IConvertible)ToString()).ToDouble(provider);

        /// <inheritdoc/>
        public short ToInt16(IFormatProvider provider) => ((IConvertible)ToString()).ToInt16(provider);

        /// <inheritdoc/>
        public int ToInt32(IFormatProvider provider) => ((IConvertible)ToString()).ToInt32(provider);

        /// <inheritdoc/>
        public long ToInt64(IFormatProvider provider) => ((IConvertible)ToString()).ToInt64(provider);

        /// <inheritdoc/>
        public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)ToString()).ToSByte(provider);

        /// <inheritdoc/>
        public float ToSingle(IFormatProvider provider) => ((IConvertible)ToString()).ToSingle(provider);

        /// <inheritdoc/>
        public string ToString(IFormatProvider provider) => ToString();

        /// <inheritdoc/>
        public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)ToString()).ToType(conversionType, provider);

        /// <inheritdoc/>
        public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)ToString()).ToUInt16(provider);

        /// <inheritdoc/>
        public uint ToUInt32(IFormatProvider provider) => ((IConvertible)ToString()).ToUInt32(provider);

        /// <inheritdoc/>
        public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)ToString()).ToUInt64(provider);

        /// <inheritdoc/>
        public int CompareTo(string other) => ToString().CompareTo(other);

        /// <inheritdoc/>
        public bool Equals(string other) => ToString().Equals(other);

        /// <inheritdoc/>
        public int CompareTo(UTF7 other) => ToString().CompareTo(other?.ToString());

        /// <inheritdoc/>
        public static bool operator <(UTF7 left, UTF7 right) => left is null ? right is not null : left.CompareTo(right) < 0;

        /// <inheritdoc/>
        public static bool operator <=(UTF7 left, UTF7 right) => left is null || left.CompareTo(right) <= 0;

        /// <inheritdoc/>
        public static bool operator >(UTF7 left, UTF7 right) => left is not null && left.CompareTo(right) > 0;

        /// <inheritdoc/>
        public static bool operator >=(UTF7 left, UTF7 right) => left is null ? right is null : left.CompareTo(right) >= 0;

        static string EncodeUTF7Chunk(string text)
        {
            var data = Encoding.BigEndianUnicode.GetBytes(text);
            return Base64.NoPadding.Encode(data);
        }

        static string DecodeUTF7Chunk(byte[] code)
        {
            var data = Base64.NoPadding.Decode(code);
            return Encoding.BigEndianUnicode.GetString(data);
        }

        /// <summary>
        /// Provides extended UTF-7 decoding (rfc 3501)
        /// </summary>
        public static string DecodeExtendedUTF7(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (!data.Contains((byte)'&')) return ASCII.GetString(data);
            var result = new StringBuilder();
            List<byte> code = null;
            for (var i = 0; i < data.Length; i++)
            {
                if (code != null)
                {
                    if (data[i] == '-')
                    {
                        var chunk = DecodeUTF7Chunk(code.ToArray());
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
                            code = new List<byte> { data[i] };
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

        /// <summary>
        /// Provides extended UTF-7 encoding (rfc 3501)
        /// </summary>
        public static byte[] EncodeExtendedUTF7(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
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
                            var chunk = EncodeUTF7Chunk(code.ToString());
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
                        if (code == null) code = new StringBuilder();
                        code.Append(ch);
                    }
                }
                else
                {
                    if (code != null)
                    {
                        var chunk = EncodeUTF7Chunk(code.ToString());
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
                var chunk = EncodeUTF7Chunk(code.ToString());
                result.Append("&" + chunk + "-");
            }
            return ASCII.GetBytes(result.ToString());
        }
    }
}
