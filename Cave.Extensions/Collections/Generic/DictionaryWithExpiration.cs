using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CA1710
#pragma warning disable CS8601

namespace Cave.Collections.Generic;

/// <summary>Gets a dictionary with expiring items.</summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <seealso cref="IDictionary{TKey,TValue}"/>
public class DictionaryWithExpiration<TKey, TValue> : IDictionary<TKey, TValue>
    where TKey : notnull
    where TValue : IExpiring
{
    #region Private Fields

    readonly Dictionary<TKey, TValue> items = new();
    long nextCheckTicks;
    long ticksBetweenChecks = TimeSpan.TicksPerSecond;

    #endregion Private Fields

    #region Public Properties

    /// <inheritdoc/>
    public int Count
    {
        get
        {
            Expire();
            return items.Count;
        }
    }

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public ICollection<TKey> Keys
    {
        get
        {
            Expire();
            return items.Keys;
        }
    }

    /// <summary>Gets the next check date time.</summary>
    /// <value>The next check date time.</value>
    public DateTime NextCheck => new DateTime(nextCheckTicks, DateTimeKind.Utc).ToLocalTime();

    /// <summary>Gets or sets the time between checks.</summary>
    /// <value>The time between checks.</value>
    public TimeSpan TimeBetweenChecks { get => new(ticksBetweenChecks); set => ticksBetweenChecks = value.Ticks; }

    /// <inheritdoc/>
    public ICollection<TValue> Values
    {
        get
        {
            Expire();
            return items.Values;
        }
    }

    #endregion Public Properties

    #region Public Indexers

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get
        {
            Expire();
            return items[key];
        }
        set
        {
            items[key] = value;
            Expire();
        }
    }

    #endregion Public Indexers

    #region Public Methods

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Expire();
        items.Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        Expire();
        items.Add(key, value);
    }

    /// <inheritdoc/>
    public void Clear() => items.Clear();

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        Expire();
        return ((IDictionary<TKey, TValue>)items).Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        Expire();
        return items.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        Expire();
        ((IDictionary<TKey, TValue>)items).CopyTo(array, arrayIndex);
    }

    /// <summary>Checks all items for expiration.</summary>
    public void Expire()
    {
        var now = DateTime.UtcNow.Ticks;
        if (now < nextCheckTicks)
        {
            return;
        }
        foreach (var item in items.ToList())
        {
            if (!item.Value.IsExpired())
            {
                continue;
            }
            if (!items.Remove(item.Key))
            {
                throw new KeyNotFoundException();
            }
        }

        nextCheckTicks = now + ticksBetweenChecks;
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IDictionary<TKey, TValue>)items).GetEnumerator();

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        Expire();
        return ((IDictionary<TKey, TValue>)items).Remove(item);
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        Expire();
        return items.Remove(key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, out TValue value)
    {
        Expire();
        return items.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<TKey, TValue>)items).GetEnumerator();

    #endregion Public Methods
}
