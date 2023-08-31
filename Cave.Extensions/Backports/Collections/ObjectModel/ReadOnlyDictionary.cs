#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20 || NET35

using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace System.Collections.ObjectModel;

[Serializable]
[DebuggerDisplay("Count = {Count}")]
public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
{
    #region Static

    static bool IsCompatibleKey(object key) => key is not null ? key is TKey : throw new ArgumentNullException(nameof(key));

    #endregion Static

    #region Constructors

    public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) => Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

    #endregion Constructors

    #region Properties

    public TValue this[TKey key] => Dictionary[key];

    public ICollection<TKey> Keys => new ReadOnlyCollectionWrapper<TKey>(Dictionary.Keys);

    public ICollection<TValue> Values => new ReadOnlyCollectionWrapper<TValue>(Dictionary.Values);

    protected IDictionary<TKey, TValue> Dictionary { get; }

    #endregion Properties

    #region IDictionary Members

    void ICollection.CopyTo(Array array, int index) => ((ICollection)Dictionary).CopyTo(array, index);

    public bool IsSynchronized => false;

    [field: NonSerialized]
    public object SyncRoot { get; } = new();

    void IDictionary.Add(object key, object value) => throw new ReadOnlyException();

    void IDictionary.Clear() => throw new ReadOnlyException();

    bool IDictionary.Contains(object key) => IsCompatibleKey(key) && ContainsKey((TKey)key);

    IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)Dictionary).GetEnumerator();

    bool IDictionary.IsFixedSize => true;

    bool IDictionary.IsReadOnly => true;

    object IDictionary.this[object key]
    {
        get => ((IDictionary)Dictionary)[key];
        set => throw new ReadOnlyException();
    }

    ICollection IDictionary.Keys => ((IDictionary)Dictionary).Keys;

    void IDictionary.Remove(object key) => throw new ReadOnlyException();

    ICollection IDictionary.Values => ((IDictionary)Dictionary).Values;

    #endregion IDictionary Members

    #region IDictionary<TKey,TValue> Members

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => throw new ReadOnlyException();

    void ICollection<KeyValuePair<TKey, TValue>>.Clear() => throw new ReadOnlyException();

    public bool Contains(KeyValuePair<TKey, TValue> item) => Dictionary.Contains(item);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => Dictionary.CopyTo(array, arrayIndex);

    public int Count => Dictionary.Count;

    public bool IsReadOnly => true;

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => throw new ReadOnlyException();

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => throw new ReadOnlyException();

    public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get => Dictionary[key];
        set => throw new ReadOnlyException();
    }

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

    bool IDictionary<TKey, TValue>.Remove(TKey key) => throw new ReadOnlyException();

    public bool TryGetValue(TKey key, out TValue value) => Dictionary.TryGetValue(key, out value);

    ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Dictionary).GetEnumerator();

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Dictionary.GetEnumerator();

    #endregion IDictionary<TKey,TValue> Members
}

#endif
