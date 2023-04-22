using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cave.Collections.Generic;

/// <summary>Gets a thread safe dictionary. This is much faster than SynchronizedDictionary if you got only two threads accessing values.</summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <seealso cref="System.Collections.Generic.IDictionary{TKey, TValue}" />
public class SynchronizedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    #region Fields

    readonly IDictionary<TKey, TValue> dict;

    #endregion

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}" /> class.</summary>
    public SynchronizedDictionary() => dict = new Dictionary<TKey, TValue>();

    /// <summary>Initializes a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}" /> class.</summary>
    /// <param name="dictionary">The dictionary.</param>
    public SynchronizedDictionary(IDictionary<TKey, TValue> dictionary) => dict = dictionary;

    #endregion

    #region Properties

    /// <inheritdoc />
    public bool IsEmpty => dict.Count == 0;

    /// <inheritdoc />
    public object SyncRoot { get; } = new();

    #endregion

    #region IDictionary<TKey,TValue> Members

    /// <inheritdoc />
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        lock (SyncRoot)
        {
            dict.Add(item);
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        lock (SyncRoot)
        {
            dict.Clear();
        }
    }

    /// <inheritdoc />
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        lock (SyncRoot)
        {
            return dict.Contains(item);
        }
    }

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        lock (SyncRoot)
        {
            dict.CopyTo(array, arrayIndex);
        }
    }

    /// <inheritdoc />
    public int Count
    {
        get
        {
            lock (SyncRoot)
            {
                return dict.Count;
            }
        }
    }

    /// <inheritdoc />
    public bool IsReadOnly
    {
        get
        {
            lock (SyncRoot)
            {
                return dict.IsReadOnly;
            }
        }
    }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        lock (SyncRoot)
        {
            return dict.Remove(item);
        }
    }

    /// <inheritdoc />
    public void Add(TKey key, TValue value)
    {
        lock (SyncRoot)
        {
            dict.Add(key, value);
        }
    }

    /// <inheritdoc />
    public bool ContainsKey(TKey key)
    {
        lock (SyncRoot)
        {
            return dict.ContainsKey(key);
        }
    }

    /// <summary>Gets or sets the value with the specified key.</summary>
    /// <value>The value.</value>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public TValue this[TKey key]
    {
        get
        {
            lock (SyncRoot)
            {
                return dict[key];
            }
        }
        set
        {
            lock (SyncRoot)
            {
                dict[key] = value;
            }
        }
    }

    /// <inheritdoc />
    public ICollection<TKey> Keys
    {
        get
        {
            lock (SyncRoot)
            {
                return dict.Keys.ToArray();
            }
        }
    }

    /// <inheritdoc />
    public bool Remove(TKey key)
    {
        lock (SyncRoot)
        {
            return dict.Remove(key);
        }
    }

    /// <inheritdoc />
    public bool TryGetValue(TKey key, out TValue value)
    {
        lock (SyncRoot)
        {
            return dict.TryGetValue(key, out value);
        }
    }

    /// <inheritdoc />
    public ICollection<TValue> Values
    {
        get
        {
            lock (SyncRoot)
            {
                return dict.Values.ToArray();
            }
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        lock (SyncRoot)
        {
            var items = new KeyValuePair<TKey, TValue>[dict.Count];
            CopyTo(items, 0);
            return items.GetEnumerator();
        }
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        lock (SyncRoot)
        {
            var items = new KeyValuePair<TKey, TValue>[dict.Count];
            CopyTo(items, 0);
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)items).GetEnumerator();
        }
    }

    #endregion

    #region Members

    /// <summary>Adds a range of items to the dictionary.</summary>
    /// <param name="items">The items.</param>
    public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        lock (SyncRoot)
        {
            foreach (var item in items)
            {
                dict.Add(item);
            }
        }
    }

    /// <summary>Adds a key/value pair to the Dictionary by using the specified function, if the key does not already exist.</summary>
    /// <param name="key">The key.</param>
    /// <param name="constructor">The constructor.</param>
    /// <returns></returns>
    public TValue GetOrAdd(TKey key, Func<TValue> constructor)
    {
        if (constructor == null)
        {
            throw new ArgumentNullException(nameof(constructor));
        }

        lock (SyncRoot)
        {
            if (!dict.TryGetValue(key, out var result))
            {
                result = constructor();
                dict[key] = result;
            }

            return result;
        }
    }

    /// <summary>Adds a key/value pair to the Dictionary by using the specified function, if the key does not already exist.</summary>
    /// <remarks>If the constructor for the item returns null, the item is not added.</remarks>
    /// <param name="key">The key.</param>
    /// <param name="constructor">The constructor.</param>
    /// <returns></returns>
    public TValue GetOrAddIgnoreNull(TKey key, Func<TValue> constructor)
    {
        if (constructor == null)
        {
            throw new ArgumentNullException(nameof(constructor));
        }

        lock (SyncRoot)
        {
            if (!dict.TryGetValue(key, out var result))
            {
                result = constructor();
                if (result != null)
                {
                    dict[key] = result;
                }
            }

            return result;
        }
    }

    /// <summary>Includes a range of items (replaces) at the dictionary.</summary>
    /// <param name="items">The items.</param>
    public void IncludeRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        lock (SyncRoot)
        {
            foreach (var item in items)
            {
                dict[item.Key] = item.Value;
            }
        }
    }

    /// <summary>Tries to add a new item to the dictionary.</summary>
    /// <param name="key">The key.</param>
    /// <param name="constructor">The constructor.</param>
    /// <returns>Returns true if the item was added, false otherwise.</returns>
    public bool TryAdd(TKey key, Func<TValue> constructor)
    {
        if (constructor == null)
        {
            throw new ArgumentNullException(nameof(constructor));
        }

        lock (SyncRoot)
        {
            if (dict.ContainsKey(key))
            {
                return false;
            }

            dict[key] = constructor();
        }

        return true;
    }

    /// <summary>Tries to add a new item to the dictionary.</summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>Returns true if the item was added, false otherwise.</returns>
    public bool TryAdd(TKey key, TValue value)
    {
        lock (SyncRoot)
        {
            if (dict.ContainsKey(key))
            {
                return false;
            }

            dict[key] = value;
        }

        return true;
    }

    /// <summary>Tries to remove the specified key.</summary>
    /// <param name="key">The key.</param>
    /// <returns>Returns true on successful remove or false is. </returns>
    public bool TryRemove(TKey key)
    {
        var removed = false;
        lock (SyncRoot)
        {
            if (dict.ContainsKey(key))
            {
                dict.Remove(key);
                removed = true;
            }
        }

        return removed;
    }

    /// <summary>Tries to remove the specified key.</summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>Returns true on successful remove or false otherwise.</returns>
    public bool TryRemove(TKey key, out TValue value)
    {
        var removed = false;
        lock (SyncRoot)
        {
            if (dict.TryGetValue(key, out value))
            {
                dict.Remove(key);
                removed = true;
            }
        }

        return removed;
    }

    #endregion
}
