#if NET20 || NET35
#pragma warning disable CS1591

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Concurrent;

public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    public bool TryRemove(TKey key, out TValue value)
    {
        lock (dictionary)
        {
            if (dictionary.TryGetValue(key, out value))
            {
                dictionary.Remove(key);
                return true;
            }
            return false;
        }
    }

    public bool IsEmpty => LockedGet(() => dictionary.Count == 0);

    readonly IDictionary<TKey, TValue> dictionary;

    public ConcurrentDictionary() => dictionary = new Dictionary<TKey, TValue>();

    public ConcurrentDictionary(int capacity) => dictionary = new Dictionary<TKey, TValue>(capacity);

    public ConcurrentDictionary(IEqualityComparer<TKey> comparer) => dictionary = new Dictionary<TKey, TValue>(comparer);

    public ConcurrentDictionary(int capacity, IEqualityComparer<TKey> comparer) => dictionary = new Dictionary<TKey, TValue>(capacity, comparer);

    public ConcurrentDictionary(IDictionary<TKey, TValue> dictionary) => this.dictionary = new Dictionary<TKey, TValue>(dictionary);

    public ConcurrentDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) => this.dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);

    TResult LockedGet<TResult>(Func<TResult> func)
    {
        lock (dictionary) return func();
    }

    void Locked(Action action)
    {
        lock (dictionary) action();
    }

    public TValue this[TKey key] { get => LockedGet(() => dictionary[key]); set => Locked(() => dictionary[key] = value); }

    public ICollection<TKey> Keys => LockedGet<ICollection<TKey>>(() => dictionary.Keys);

    public ICollection<TValue> Values => LockedGet<ICollection<TValue>>(() => dictionary.Values);

    public int Count => LockedGet<int>(() => dictionary.Count);

    public bool IsReadOnly => dictionary.IsReadOnly;

    public void Add(TKey key, TValue value) => Locked(() => dictionary.Add(key, value));

    public void Add(KeyValuePair<TKey, TValue> item) => Locked(() => dictionary.Add(item));

    public void Clear() => Locked(dictionary.Clear);

    public bool Contains(KeyValuePair<TKey, TValue> item) => LockedGet(() => dictionary.Contains(item));

    public bool ContainsKey(TKey key) => LockedGet(() => dictionary.ContainsKey(key));

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => Locked(() => dictionary.CopyTo(array, arrayIndex));

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => LockedGet(() => dictionary.ToList().GetEnumerator());

    public bool Remove(TKey key) => LockedGet(() => dictionary.Remove(key));

    public bool Remove(KeyValuePair<TKey, TValue> item) => LockedGet(() => dictionary.Remove(item));

    public bool TryGetValue(TKey key, out TValue value)
    {
        lock (dictionary)
        {
            return dictionary.TryGetValue(key, out value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)dictionary).GetEnumerator();
}

#endif
