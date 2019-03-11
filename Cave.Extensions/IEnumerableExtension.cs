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
            long value = 0;
            foreach (T item in items)
            {
                value |= predicate(item);
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
            ulong value = 0;
            foreach (T item in items)
            {
                value |= predicate(item);
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
            var value = 0;
            foreach (T item in items)
            {
                value |= predicate(item);
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
            uint value = 0;
            foreach (T item in items)
            {
                value |= predicate(item);
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
            ulong value = 0;
            foreach (var item in items)
            {
                value |= item;
            }
            return value;
        }

        /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary and result of the values in the sequence.</returns>
        public static ulong BinaryAnd(this IEnumerable<ulong> items)
        {
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
            var value = 0;
            foreach (var item in items)
            {
                value |= item;
            }
            return value;
        }

        /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary and result of the values in the sequence.</returns>
        public static int BinaryAnd(this IEnumerable<int> items)
        {
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
            uint value = 0;
            foreach (var item in items)
            {
                value |= item;
            }
            return value;
        }

        /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
        /// <param name="items">The items.</param>
        /// <returns>The binary and result of the values in the sequence.</returns>
        public static uint BinaryAnd(this IEnumerable<uint> items)
        {
            var value = 0xffffffff;
            foreach (var item in items)
            {
                value &= item;
            }
            return value;
        }
        #endregion
    }
}
