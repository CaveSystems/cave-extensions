#region CopyRight 2018
/*
    Copyright (c) 2007-2018 Andreas Rohleder (andreas@rohleder.cc)
    All rights reserved
*/
#endregion
#region License LGPL-3
/*
    This program/library/sourcecode is free software; you can redistribute it
    and/or modify it under the terms of the GNU Lesser General Public License
    version 3 as published by the Free Software Foundation subsequent called
    the License.

    You may not use this program/library/sourcecode except in compliance
    with the License. The License is included in the LICENSE file
    found at the installation directory or the distribution package.

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the
    "Software"), to deal in the Software without restriction, including
    without limitation the rights to use, copy, modify, merge, publish,
    distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to
    the following conditions:

    The above copyright notice and this permission notice shall be included
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion
#region Authors & Contributors
/*
   Author:
     Andreas Rohleder <andreas@rohleder.cc>

   Contributors:
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Cave
{
	/// <summary>Some additional linq extensions</summary>
	public static class IEnumerableExtension
    {
		#region long, predicate
		/// <summary>Computes the binary or result of a sequence of numeric values.</summary>
		/// <typeparam name="T"></typeparam>
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
		/// <typeparam name="T"></typeparam>
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
		/// <typeparam name="T"></typeparam>
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
		/// <typeparam name="T"></typeparam>
		/// <param name="items">The items.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns>The binary and result of the values in the sequence.</returns>
		public static ulong BinaryAnd<T>(this IEnumerable<T> items, Func<T, ulong> predicate)
		{
			ulong value = 0xffffffffffffffff;
			foreach (T item in items)
			{
				value &= predicate(item);
			}
			return value;
		}
		#endregion

		#region int, predicate
		/// <summary>Computes the binary or result of a sequence of numeric values.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items">The items.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns>The binary or result of the values in the sequence.</returns>
		public static int BinaryOr<T>(this IEnumerable<T> items, Func<T, int> predicate)
		{
			int value = 0;
			foreach (T item in items)
			{
				value |= predicate(item);
			}
			return value;
		}

		/// <summary>Computes the binary and result of a sequence of numeric values.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items">The items.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns>The binary and result of the values in the sequence.</returns>
		public static int BinaryAnd<T>(this IEnumerable<T> items, Func<T, int> predicate)
		{
			int value = -1;
			foreach (T item in items)
			{
				value &= predicate(item);
			}
			return value;
		}
		#endregion

		#region uint, predicate
		/// <summary>Computes the binary or result of a sequence of numeric values.</summary>
		/// <typeparam name="T"></typeparam>
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
		/// <typeparam name="T"></typeparam>
		/// <param name="items">The items.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns>The binary and result of the values in the sequence.</returns>
		public static uint BinaryAnd<T>(this IEnumerable<T> items, Func<T, uint> predicate)
		{
			uint value = 0xffffffff;
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
			foreach (long item in items)
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
			foreach (long item in items)
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
			foreach (ulong item in items)
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
			ulong value = 0xffffffffffffffff;
			foreach (ulong item in items)
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
			int value = 0;
			foreach (int item in items)
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
			int value = -1;
			foreach (int item in items)
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
			foreach (uint item in items)
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
			uint value = 0xffffffff;
			foreach (uint item in items)
			{
				value &= item;
			}
			return value;
		}
		#endregion
	}
}
