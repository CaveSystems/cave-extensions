using System;
using System.Collections.Generic;

namespace Cave
{
    /// <summary>Some additional linq extensions.</summary>
    public static class IEnumerableExtension
    {
        #region long, predicate

        /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static long BinaryOr<T>(this IEnumerable<T> items, Func<T, long> predicate)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            long value = 0;
            foreach (T item in items)
            {
                value |= predicate(item);
            }
            return value;
        }

        /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static long BinaryXor<T>(this IEnumerable<T> items, Func<T, long> predicate)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            long value = 0;
            foreach (T item in items)
            {
                value ^= predicate(item);
            }
            return value;
        }

        /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The binary and result of the values in the sequence.</returns>
        public static long BinaryAnd<T>(this IEnumerable<T> items, Func<T, long> predicate)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            long value = -1;
            foreach (T item in items)
            {
                value &= predicate(item);
            }
            return value;
        }
        #endregion

        #region ulong, predicate

        /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static ulong BinaryOr<T>(this IEnumerable<T> items, Func<T, ulong> predicate)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            ulong value = 0;
            foreach (T item in items)
            {
                value |= predicate(item);
            }
            return value;
        }

        /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static ulong BinaryXor<T>(this IEnumerable<T> items, Func<T, ulong> predicate)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            ulong value = 0;
            foreach (T item in items)
            {
                value ^= predicate(item);
            }
            return value;
        }

        /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The binary and result of the values in the sequence.</returns>
        public static ulong BinaryAnd<T>(this IEnumerable<T> items, Func<T, ulong> predicate)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var value = 0xffffffffffffffff;
            foreach (T item in items)
            {
                value &= predicate(item);
            }
            return value;
        }
        #endregion

        #region int, predicate

        /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static int BinaryOr<T>(this IEnumerable<T> items, Func<T, int> predicate)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var value = 0;
            foreach (T item in items)
            {
                value |= predicate(item);
            }
            return value;
        }

        /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static int BinaryXor<T>(this IEnumerable<T> items, Func<T, int> predicate)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var value = 0;
            foreach (T item in items)
            {
                value ^= predicate(item);
            }
            return value;
        }

        /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The binary and result of the values in the sequence.</returns>
        public static int BinaryAnd<T>(this IEnumerable<T> items, Func<T, int> predicate)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var value = -1;
            foreach (T item in items)
            {
                value &= predicate(item);
            }
            return value;
        }
        #endregion

        #region uint, predicate

        /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static uint BinaryOr<T>(this IEnumerable<T> items, Func<T, uint> predicate)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            uint value = 0;
            foreach (T item in items)
            {
                value |= predicate(item);
            }
            return value;
        }

        /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static uint BinaryXor<T>(this IEnumerable<T> items, Func<T, uint> predicate)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            uint value = 0;
            foreach (T item in items)
            {
                value ^= predicate(item);
            }
            return value;
        }

        /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The binary and result of the values in the sequence.</returns>
        public static uint BinaryAnd<T>(this IEnumerable<T> items, Func<T, uint> predicate)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var value = 0xffffffff;
            foreach (T item in items)
            {
                value &= predicate(item);
            }
            return value;
        }

        #endregion

        #region long

        /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static long BinaryOr(this IEnumerable<long> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            long value = 0;
            foreach (var item in items)
            {
                value |= item;
            }
            return value;
        }

        /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static long BinaryXor(this IEnumerable<long> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            long value = 0;
            foreach (var item in items)
            {
                value |= item;
            }
            return value;
        }

        /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary and result of the values in the sequence.</returns>
        public static long BinaryAnd(this IEnumerable<long> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            long value = -1;
            foreach (var item in items)
            {
                value &= item;
            }
            return value;
        }
        #endregion

        #region ulong

        /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static ulong BinaryOr(this IEnumerable<ulong> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            ulong value = 0;
            foreach (var item in items)
            {
                value |= item;
            }
            return value;
        }

        /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static ulong BinaryXor(this IEnumerable<ulong> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            ulong value = 0;
            foreach (var item in items)
            {
                value ^= item;
            }
            return value;
        }

        /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary and result of the values in the sequence.</returns>
        public static ulong BinaryAnd(this IEnumerable<ulong> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var value = 0xffffffffffffffff;
            foreach (var item in items)
            {
                value &= item;
            }
            return value;
        }
        #endregion

        #region int

        /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static int BinaryOr(this IEnumerable<int> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var value = 0;
            foreach (var item in items)
            {
                value |= item;
            }
            return value;
        }

        /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static int BinaryXor(this IEnumerable<int> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var value = 0;
            foreach (var item in items)
            {
                value ^= item;
            }
            return value;
        }

        /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary and result of the values in the sequence.</returns>
        public static int BinaryAnd(this IEnumerable<int> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var value = -1;
            foreach (var item in items)
            {
                value &= item;
            }
            return value;
        }
        #endregion

        #region uint

        /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static uint BinaryOr(this IEnumerable<uint> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            uint value = 0;
            foreach (var item in items)
            {
                value |= item;
            }
            return value;
        }

        /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary or result of the values in the sequence.</returns>
        public static uint BinaryXor(this IEnumerable<uint> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            uint value = 0;
            foreach (var item in items)
            {
                value ^= item;
            }
            return value;
        }

        /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary and result of the values in the sequence.</returns>
        public static uint BinaryAnd(this IEnumerable<uint> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var value = 0xffffffff;
            foreach (var item in items)
            {
                value &= item;
            }
            return value;
        }
        #endregion

        /// <summary>
        /// Calculates the hash for all properties of the specified items.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The items.</param>
        /// <returns>Returns the hashcode for all items.</returns>
        public static long CalculatePropertyHash<T>(this IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            long result = 0;
            var properties = typeof(T).GetProperties();
            foreach (var item in items)
            {
                foreach (var property in properties)
                {
                    result = result.BitwiseRotateLeft();
#if NET20 || NET35 || NET40
                    result ^= property.GetValue(item, null)?.GetHashCode() ?? 0;
#else
                    result ^= property.GetValue(item)?.GetHashCode() ?? 0;
#endif
                }
            }
            return result;
        }
    }
}
