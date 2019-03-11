using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cave
{
    /// <summary>
    /// Provides extensions to byte[], array and IEnumerable instances.
    /// </summary>
    public static class ArrayExtension
    {
        /// <summary>
        /// Retrieves a number of elements from the array as new array instance.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="data">Source array.</param>
        /// <param name="index">Element index.</param>
        /// <param name="count">Number of elements to copy.</param>
        /// <returns>Returns a new array instance.</returns>
        public static T[] GetRange<T>(this T[] data, int index, int count)
        {
            var result = new T[count];
            Array.Copy(data, index, result, 0, count);
            return result;
        }

        /// <summary>
        /// Retrieves a number of elements from the array as new array instance.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="data">Source array.</param>
        /// <param name="index">Element index.</param>
        /// <returns>Returns a new array instance.</returns>
        public static T[] GetRange<T>(this T[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            var result = new T[data.Length - index];
            Array.Copy(data, index, result, 0, result.Length);
            return result;
        }

        /// <summary>Shuffles items with the specified seed. The same seed will always result in the same order.</summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="items">The items to shuffle.</param>
        /// <param name="seed">The seed.</param>
        /// <returns>List of shuffled items.</returns>
        public static List<T> Shuffle<T>(this IEnumerable<T> items, int seed = 0)
        {
            unchecked
            {
                if (seed == 0)
                {
                    seed = (int)DateTime.UtcNow.Ticks;
                }

                var result = items.ToList();
                var count = result.Count;
                for (var i = 0; i < count; i++)
                {
                    var n = Math.Abs((i ^ seed).GetHashCode()) % count;
                    T t = result[i];
                    result[i] = result[n];
                    result[n] = t;
                }
                return result;
            }
        }

        /// <summary>
        /// Retrieves a number of elements from the array as new array instance.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="data">Source array.</param>
        /// <param name="index">Element index.</param>
        /// <param name="count">Number of elements to copy.</param>
        /// <returns>Returns a new array instance.</returns>
        public static T[] GetRange<T>(this IList<T> data, int index, int count)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            var result = new T[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = data[index++];
            }

            return result;
        }

        /// <summary>
        /// Retrieves a number of elements from the array as new array instance.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="data">Source array.</param>
        /// <param name="index">Element index.</param>
        /// <returns>Returns a new array instance.</returns>
        public static T[] GetRange<T>(this IList<T> data, int index)
        {
            return GetRange(data, index, data.Count - index);
        }

        /// <summary>
        /// Retrieves a number of elements from the array as new array instance.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="data">Source array.</param>
        /// <param name="index">Element index.</param>
        /// <param name="count">Number of elements to copy.</param>
        /// <returns>Returns a new array instance.</returns>
        public static IEnumerable<T> SubRange<T>(this IEnumerable<T> data, int index, int count)
        {
            return data.Where((v, i) => i >= index && i < index + count);
        }

        /// <summary>
        /// Retrieves a number of elements from the array as new array instance.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="data">Source array.</param>
        /// <param name="index">Element index.</param>
        /// <returns>Returns a new array instance.</returns>
        public static IEnumerable<T> SubRange<T>(this IEnumerable<T> data, int index)
        {
            return data.Where((v, i) => i >= index);
        }

        /// <summary>Concatenates elements.</summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>Array of concated items.</returns>
        public static T[] Concat<T>(this T[] t1, params T[] t2)
        {
            var result = new T[t1.Length + t2.Length];
            Array.Copy(t1, 0, result, 0, t1.Length);
            Array.Copy(t2, 0, result, t1.Length, t2.Length);
            return result;
        }

        /// <summary>Concatenates elements.</summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>Array of concated items.</returns>
        public static T[] Concat<T>(this T[] t1, params T[][] t2)
        {
            var count = t1.Length;
            for (var i = 0; i < t2.Length; i++)
            {
                count += t2[i].Length;
            }
            var result = new T[count];
            Array.Copy(t1, 0, result, 0, t1.Length);
            var start = t1.Length;
            for (var i = 0; i < t2.Length; i++)
            {
                Array.Copy(t2[i], 0, result, start, t2[i].Length);
                start += t2[i].Length;
            }
            return result;
        }

        /// <summary>Checks whether a range of bytes matches the comparand.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="comparand">The comparand.</param>
        /// <returns>True if the range matches.</returns>
        public static bool RangeEquals(this byte[] bytes, int offset, int count, byte[] comparand)
        {
            if (offset < 0 || count < 0 || bytes.Length < offset + count)
            {
                return false;
            }

            for (int i = 0, j = offset; i < count; i++, j++)
            {
                if (bytes[j] != comparand[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Checks whether data starts with the specified pattern or not.</summary>
        /// <param name="data">The data.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="encoding">The encoding (defaults to <see cref="Encoding.UTF8"/>).</param>
        /// <returns>True if data starts with the pattern.</returns>
        public static bool StartsWith(this byte[] data, string pattern, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            var bytes = encoding.GetBytes(pattern);
            return StartsWith(data, bytes);
        }

        /// <summary>Checks whether data starts with the specified pattern or not.</summary>
        /// <param name="data">The data.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns>True if data starts with the pattern.</returns>
        public static bool StartsWith(this byte[] data, byte[] pattern)
        {
            if (pattern.Length > data.Length)
            {
                return false;
            }

            for (var i = 0; i < pattern.Length; i++)
            {
                if (pattern[i] != data[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Finds the startindex of the first occurence of the specified pattern.</summary>
        /// <param name="data">The data.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns>The startindex.</returns>
        public static int IndexOf(this byte[] data, byte[] pattern)
        {
            var matchIndex = 0;
            for (var i = 0; i < data.Length; i++)
            {
                if (data[i] == pattern[matchIndex++])
                {
                    // last pattern byte reached ?
                    if (matchIndex + 1 == pattern.Length)
                    {
                        // yes
                        return i - matchIndex;
                    }
                }
                else
                {
                    // no match, reset
                    matchIndex = 0;
                }
            }
            return -1;
        }

        /// <summary>Replaces the specified pattern.</summary>
        /// <param name="data">The data.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacer">The replacer.</param>
        /// <returns>The replaced Data.</returns>
        /// <exception cref="ArgumentException">Pattern not found.</exception>
        public static byte[] ReplaceFirst(this byte[] data, byte[] pattern, byte[] replacer)
        {
            var i = data.IndexOf(pattern);
            if (i < 0)
            {
                return data;
            }

            var result = new byte[data.Length - pattern.Length + replacer.Length];
            Buffer.BlockCopy(data, 0, result, 0, i);
            Buffer.BlockCopy(replacer, 0, result, i, replacer.Length);
            var offs = i + pattern.Length + 1;
            Buffer.BlockCopy(data, offs, result, i + replacer.Length, data.Length - offs);
            return result;
        }

        /// <summary>Replaces the specified pattern.</summary>
        /// <param name="data">The data.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacers">The replacers.</param>
        /// <returns>The replaced Data.</returns>
        /// <exception cref="ArgumentException">Pattern not found.</exception>
        public static byte[] ReplaceFirst(this byte[] data, byte[] pattern, params byte[][] replacers)
        {
            var i = data.IndexOf(pattern);
            if (i < 0)
            {
                return data;
            }

            var replacersLength = replacers.Select(r => r.Length).Sum();
            var result = new byte[data.Length - pattern.Length + replacersLength];
            Buffer.BlockCopy(data, 0, result, 0, i);
            {
                var offs = i;
                foreach (var replacer in replacers)
                {
                    Buffer.BlockCopy(replacer, 0, result, offs, replacer.Length);
                    offs += replacer.Length;
                }
            }
            {
                var offs = i + pattern.Length + 1;
                Buffer.BlockCopy(data, offs, result, i + replacersLength, data.Length - offs);
            }
            return result;
        }

        /// <summary>
        /// Performs an <see cref="Array.IndexOf{T}(T[], T)"/> call and returns the result.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="array">The one-dimensional, zero-based array to search.</param>
        /// <param name="value">The object to locate in array.</param>
        /// <returns>The zero-based index of the first occurrence of value in the entire array, if found; otherwise, –1.</returns>
        public static int IndexOf<T>(this T[] array, T value)
        {
            return Array.IndexOf(array, value);
        }
    }
}
