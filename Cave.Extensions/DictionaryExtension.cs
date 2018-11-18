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
#endregion License
#region Authors & Contributors
/*
   Author:
     Andreas Rohleder <andreas@rohleder.cc>

   Contributors:
 */
#endregion

using System;
using System.Collections.Generic;

namespace Cave
{
    /// <summary>
    /// Provides extensions for <see cref="IDictionary{TKey, TValue}"/> instances
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// Tries to add an entry to the <see cref="IDictionary{TKey, TValue}"/> instance.
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary instance to add key value pair to</param>
        /// <param name="key">The key to add</param>
        /// <param name="value">The value to add</param>
        /// <returns>Returns true if the entry was added, false otherwise</returns>
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                return false;
            }

            dictionary.Add(key, value);
            return true;
        }

        /// <summary>
        /// Tries to add an entry to the <see cref="IDictionary{TKey, TValue}"/> instance.
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary instance to add key value pair to</param>
        /// <param name="key">The key to add</param>
        /// <param name="valueFunc">A function to retrieve the value for a specified key</param>
        /// <returns>Returns true if the entry was added, false otherwise</returns>
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFunc)
        {
            if (dictionary.ContainsKey(key))
            {
                return false;
            }

            dictionary.Add(key, valueFunc(key));
            return true;
        }

        /// <summary>
        /// Tries to add a number of entries to the <see cref="IDictionary{TKey, TValue}"/> instance.
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary instance to add key value pair to</param>
        /// <param name="pairs">The key value pairs to add</param>
        /// <returns>Returns true if the entry was added, false otherwise</returns>
        public static void TryAddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            foreach (var pair in pairs)
            {
                TryAdd(dictionary, pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Tries to add a number of entries to the <see cref="IDictionary{TKey, TValue}"/> instance.
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary instance to add key value pair to</param>
        /// <param name="keys">The keys to add</param>
        /// <param name="valueFunc">A function to retrieve the value for a specified key</param>
        /// <returns>Returns true if the entry was added, false otherwise</returns>
        public static void TryAddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys, Func<TKey, TValue> valueFunc)
        {
            foreach(var key in keys)
            {
                TryAdd(dictionary, key, valueFunc);
            }
        }

        /// <summary>
        /// Tries to retrieve the value for the specified key
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary instance to add key value pair to</param>
        /// <param name="key">The key to get</param>
        /// <returns>Returns the value found or default(value)</returns>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            dictionary.TryGetValue(key, out TValue value);
            return value;
        }
    }
}
