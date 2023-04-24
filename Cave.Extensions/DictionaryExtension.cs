using System;
using System.Collections.Generic;

namespace Cave;

/// <summary>Gets extensions for <see cref="IDictionary{TKey,TValue}" /> instances.</summary>
public static class DictionaryExtension
{
    #region Static

    /// <summary>Tries to add an entry to the <see cref="IDictionary{TKey, TValue}" /> instance.</summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="dictionary">Dictionary instance to add key value pair to.</param>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>Returns true if the entry was added, false otherwise.</returns>
    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        if (dictionary.ContainsKey(key))
        {
            return false;
        }

        dictionary.Add(key, value);
        return true;
    }

    /// <summary>Tries to add an entry to the <see cref="IDictionary{TKey, TValue}" /> instance.</summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="dictionary">Dictionary instance to add key value pair to.</param>
    /// <param name="key">The key to add.</param>
    /// <param name="valueFunc">A function to retrieve the value for a specified key.</param>
    /// <returns>Returns true if the entry was added, false otherwise.</returns>
    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFunc)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        if (valueFunc == null)
        {
            throw new ArgumentNullException(nameof(valueFunc));
        }

        if (dictionary.ContainsKey(key))
        {
            return false;
        }

        var value = valueFunc != null ? valueFunc(key) : default;
        dictionary.Add(key, value);
        return true;
    }

    /// <summary>Tries to add a number of entries to the <see cref="IDictionary{TKey, TValue}" /> instance.</summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="dictionary">Dictionary instance to add key value pair to.</param>
    /// <param name="pairs">The key value pairs to add.</param>
    public static void TryAddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> pairs)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        if (pairs == null)
        {
            throw new ArgumentNullException(nameof(pairs));
        }

        foreach (var pair in pairs)
        {
            TryAdd(dictionary, pair.Key, pair.Value);
        }
    }

    /// <summary>Tries to add a number of entries to the <see cref="IDictionary{TKey, TValue}" /> instance.</summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="dictionary">Dictionary instance to add key value pair to.</param>
    /// <param name="keys">The keys to add.</param>
    /// <param name="valueFunc">A function to retrieve the value for a specified key.</param>
    public static void TryAddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys, Func<TKey, TValue> valueFunc)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        if (keys == null)
        {
            throw new ArgumentNullException(nameof(keys));
        }

        if (valueFunc == null)
        {
            throw new ArgumentNullException(nameof(valueFunc));
        }

        foreach (var key in keys)
        {
            TryAdd(dictionary, key, valueFunc);
        }
    }

    /// <summary>Tries to retrieve the value for the specified key.</summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="dictionary">Dictionary instance to add key value pair to.</param>
    /// <param name="key">The key to get.</param>
    /// <returns>Returns the value found or default(value).</returns>
    public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        dictionary.TryGetValue(key, out var value);
        return value;
    }

#if NET20
    /// <summary>Tries to retrieve the value for the specified key.</summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="dictionary">Dictionary instance to add key value pair to.</param>
    /// <param name="key">The key to get.</param>
    /// <param name="value">Returns the value found or default(value).</param>
    /// <returns>Returns the value found or default(value).</returns>
    public static bool TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }
        if (dictionary.ContainsKey(key))
        {
            value = dictionary[key];
            return true;
        }
        value = default;
        return false;
    }
#endif

    #endregion
}
