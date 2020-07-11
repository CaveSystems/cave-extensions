using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cave.Collections.Generic
{
    /// <summary>
    /// Provides a thread safe dictionary. This is much faster than ConcurrentDictionary if you got only two threads accessing values.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="System.Collections.Generic.IDictionary{TKey, TValue}" />
    public class SynchronizedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        IDictionary<TKey, TValue> dict;

        /// <summary>Initializes a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}"/> class.</summary>
        public SynchronizedDictionary()
        {
            dict = new Dictionary<TKey, TValue>();
        }

        /// <summary>Initializes a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}"/> class.</summary>
        /// <param name="dictionary">The dictionary.</param>
        public SynchronizedDictionary(IDictionary<TKey, TValue> dictionary)
        {
            dict = dictionary;
        }

        /// <summary>Gets or sets the value with the specified key.</summary>
        /// <value>The value.</value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                lock (this)
                {
                    return dict[key];
                }
            }
            set
            {
                lock (this)
                {
                    dict[key] = value;
                }
            }
        }

        /// <summary>
        /// Ruft die Anzahl der Elemente ab, die in <see cref="T:System.Collections.Generic.ICollection`1" /> enthalten sind.
        /// </summary>
        public int Count
        {
            get
            {
                lock (this)
                {
                    return dict.Count;
                }
            }
        }

        /// <summary>
        /// Ruft einen Wert ab, der angibt, ob <see cref="T:System.Collections.Generic.ICollection`1" /> schreibgeschützt ist.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                lock (this)
                {
                    return dict.IsReadOnly;
                }
            }
        }

        /// <summary>
        /// Ruft eine <see cref="T:System.Collections.Generic.ICollection`1" />-Schnittstelle ab, die die Schlüssel von <see cref="T:System.Collections.Generic.IDictionary`2" /> enthält.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                lock (this)
                {
                    return dict.Keys.ToArray();
                }
            }
        }

        /// <summary>
        /// Ruft eine <see cref="T:System.Collections.Generic.ICollection`1" /> ab, die die Werte in <see cref="T:System.Collections.Generic.IDictionary`2" /> enthält.
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                lock (this)
                {
                    return dict.Values.ToArray();
                }
            }
        }

        /// <summary>Tries to add a new item to the dictionary.</summary>
        /// <param name="key">The key.</param>
        /// <param name="constructor">The constructor.</param>
        /// <returns>Returns true if the item was added, false otherwise.</returns>
        public bool TryAdd(TKey key, Func<TValue> constructor)
        {
            lock (this)
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
            lock (this)
            {
                if (dict.ContainsKey(key))
                {
                    return false;
                }

                dict[key] = value;
            }
            return true;
        }

        /// <summary>Fügt der <see cref="T:System.Collections.Generic.ICollection`1" /> ein Element hinzu.</summary>
        /// <param name="item">Das Objekt, das <see cref="T:System.Collections.Generic.ICollection`1" /> hinzugefügt werden soll.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            lock (this)
            {
                dict.Add(item);
            }
        }

        /// <summary>
        /// Fügt der <see cref="T:System.Collections.Generic.IDictionary`2" />-Schnittstelle ein Element mit dem angegebenen Schlüssel und Wert hinzu.
        /// </summary>
        /// <param name="key">Das Objekt, das als Schlüssel für das hinzuzufügende Element verwendet werden soll.</param>
        /// <param name="value">Das Objekt, das als Wert für das hinzuzufügende Element verwendet werden soll.</param>
        public void Add(TKey key, TValue value)
        {
            lock (this)
            {
                dict.Add(key, value);
            }
        }

        /// <summary>Entfernt alle Elemente aus <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        public void Clear()
        {
            lock (this)
            {
                dict.Clear();
            }
        }

        /// <summary>Bestimmt, ob <see cref="T:System.Collections.Generic.ICollection`1" /> einen bestimmten Wert enthält.</summary>
        /// <param name="item">Das im <see cref="T:System.Collections.Generic.ICollection`1" /> zu suchende Objekt.</param>
        /// <returns>
        /// true, wenn sich <paramref name="item" /> in <see cref="T:System.Collections.Generic.ICollection`1" /> befindet, andernfalls false.
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (this)
            {
                return dict.Contains(item);
            }
        }

        /// <summary>
        /// Ermittelt, ob <see cref="T:System.Collections.Generic.IDictionary`2" /> ein Element mit dem angegebenen Schlüssel enthält.
        /// </summary>
        /// <param name="key">Der im <see cref="T:System.Collections.Generic.IDictionary`2" /> zu suchende Schlüssel.</param>
        /// <returns>
        /// true, wenn das <see cref="T:System.Collections.Generic.IDictionary`2" /> ein Element mit dem Schlüssel enthält, andernfalls false.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            lock (this)
            {
                return dict.ContainsKey(key);
            }
        }

        /// <summary>
        /// Kopiert die Elemente von <see cref="T:System.Collections.Generic.ICollection`1" /> in ein <see cref="T:System.Array" />, beginnend bei einem bestimmten <see cref="T:System.Array" />-Index.
        /// </summary>
        /// <param name="array">Das eindimensionale <see cref="T:System.Array" />, das das Ziel der aus <see cref="T:System.Collections.Generic.ICollection`1" /> kopierten Elemente ist.Für <see cref="T:System.Array" /> muss eine nullbasierte Indizierung verwendet werden.</param>
        /// <param name="arrayIndex">Der nullbasierte Index in <paramref name="array" />, an dem das Kopieren beginnt.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (this)
            {
                dict.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// Entfernt das erste Vorkommen eines bestimmten Objekts aus <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">Das aus dem <see cref="T:System.Collections.Generic.ICollection`1" /> zu entfernende Objekt.</param>
        /// <returns>
        /// true, wenn <paramref name="item" /> erfolgreich aus <see cref="T:System.Collections.Generic.ICollection`1" /> gelöscht wurde, andernfalls false.Diese Methode gibt auch dann false zurück, wenn <paramref name="item" /> nicht in der ursprünglichen <see cref="T:System.Collections.Generic.ICollection`1" /> gefunden wurde.
        /// </returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (this)
            {
                return dict.Remove(item);
            }
        }

        /// <summary>
        /// Entfernt das Element mit dem angegebenen Schlüssel aus dem <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">Der Schlüssel des zu entfernenden Elements.</param>
        /// <returns>
        /// true, wenn das Element erfolgreich entfernt wurde, andernfalls false.Diese Methode gibt auch dann false zurück, wenn <paramref name="key" /> nicht im ursprünglichen <see cref="T:System.Collections.Generic.IDictionary`2" /> gefunden wurde.
        /// </returns>
        public bool Remove(TKey key)
        {
            lock (this)
            {
                return dict.Remove(key);
            }
        }

        /// <summary>Tries to remove the specified key.</summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns true on successful remove or false is. </returns>
        public bool TryRemove(TKey key)
        {
            bool removed = false;
            lock (this)
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
            bool removed = false;
            lock (this)
            {
                if (dict.TryGetValue(key, out value))
                {
                    dict.Remove(key);
                    removed = true;
                }
            }
            return removed;
        }

        /// <summary>Ruft den dem angegebenen Schlüssel zugeordneten Wert ab.</summary>
        /// <param name="key">Der Schlüssel, dessen Wert abgerufen werden soll.</param>
        /// <param name="value">Wenn diese Methode zurückgegeben wird, enthält sie den dem angegebenen Schlüssel zugeordneten Wert, wenn der Schlüssel gefunden wird, andernfalls enthält sie den Standardwert für den Typ des <paramref name="value" />-Parameters.Dieser Parameter wird nicht initialisiert übergeben.</param>
        /// <returns>
        /// true, wenn das Objekt, das <see cref="T:System.Collections.Generic.IDictionary`2" /> implementiert, ein Element mit dem angegebenen Schlüssel enthält, andernfalls false.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (this)
            {
                return dict.TryGetValue(key, out value);
            }
        }

        /// <summary>Adds a key/value pair to the Dictionary by using the specified function, if the key does not already exist.</summary>
        /// <param name="key">The key.</param>
        /// <param name="constructor">The constructor.</param>
        /// <returns></returns>
        public TValue GetOrAdd(TKey key, Func<TValue> constructor)
        {
            lock (this)
            {
                if (!dict.TryGetValue(key, out TValue result))
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
            lock (this)
            {
                if (!dict.TryGetValue(key, out TValue result))
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

        /// <summary>Adds a range of items to the dictionary.</summary>
        /// <param name="items">The items.</param>
        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            lock (this)
            {
                foreach (KeyValuePair<TKey, TValue> item in items)
                {
                    dict.Add(item);
                }
            }
        }

        /// <summary>Includes a range of items (replaces) at the dictionary.</summary>
        /// <param name="items">The items.</param>
        public void IncludeRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            lock (this)
            {
                foreach (KeyValuePair<TKey, TValue> item in items)
                {
                    dict[item.Key] = item.Value;
                }
            }
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <remarks>This function may have a big impact on memory usage, since it needs to create a flat copy of the whole dictionary!.</remarks>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            lock (this)
            {
                KeyValuePair<TKey, TValue>[] items = new KeyValuePair<TKey, TValue>[dict.Count];
                CopyTo(items, 0);
                return ((IEnumerable<KeyValuePair<TKey, TValue>>)items).GetEnumerator();
            }
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <remarks>This function may have a big impact on memory usage, since it needs to create a flat copy of the whole dictionary!.</remarks>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this)
            {
                KeyValuePair<TKey, TValue>[] items = new KeyValuePair<TKey, TValue>[dict.Count];
                CopyTo(items, 0);
                return items.GetEnumerator();
            }
        }
    }
}
