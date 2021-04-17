using System;
using System.Collections.Generic;

namespace Cave.Collections.Generic
{
    /// <summary>Gets extensions for dictionaryies.</summary>
    public static class DictionaryExtensions
    {
        #region Static

        /// <summary>Tries to get the value associated with the specified key.</summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        #endregion
    }
}
