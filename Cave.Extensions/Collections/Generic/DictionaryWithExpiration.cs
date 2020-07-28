using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cave.Collections.Generic
{
    /// <summary>Gets a dictionary with expiring items.</summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="System.Collections.Generic.IDictionary{TKey, TValue}" />
    public class DictionaryWithExpiration<TKey, TValue> : IDictionary<TKey, TValue>
        where TValue : IExpiring
    {
        readonly Dictionary<TKey, TValue> items = new Dictionary<TKey, TValue>();
        long nextCheckTicks;
        long ticksBetweenChecks = TimeSpan.TicksPerSecond;

        /// <summary>Gets the next check date time.</summary>
        /// <value>The next check date time.</value>
        public DateTime NextCheck => new DateTime(nextCheckTicks, DateTimeKind.Utc).ToLocalTime();

        /// <summary>Gets or sets the time between checks.</summary>
        /// <value>The time between checks.</value>
        public TimeSpan TimeBetweenChecks { get => new TimeSpan(ticksBetweenChecks); set => ticksBetweenChecks = value.Ticks; }

        /// <summary>Gets or sets the TValue with the specified key.</summary>
        /// <value>The TValue.</value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Ruft die Anzahl der Elemente ab, die in <see cref="T:System.Collections.Generic.ICollection`1" /> enthalten
        ///     sind.
        /// </summary>
        public int Count
        {
            get
            {
                Expire();
                return items.Count;
            }
        }

        /// <summary>Returns false.</summary>
        public bool IsReadOnly => false;

        /// <summary>
        ///     Ruft eine <see cref="T:System.Collections.Generic.ICollection`1" />-Schnittstelle ab, die die Schlüssel von
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" /> enthält.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                Expire();
                return items.Keys;
            }
        }

        /// <summary>
        ///     Ruft eine <see cref="T:System.Collections.Generic.ICollection`1" /> ab, die die Werte in
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" /> enthält.
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                Expire();
                return items.Values;
            }
        }

        /// <summary>Fügt der <see cref="T:System.Collections.Generic.ICollection`1" /> ein Element hinzu.</summary>
        /// <param name="item">Das Objekt, das <see cref="T:System.Collections.Generic.ICollection`1" /> hinzugefügt werden soll.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Expire();
            items.Add(item.Key, item.Value);
        }

        /// <summary>
        ///     Fügt der <see cref="T:System.Collections.Generic.IDictionary`2" />-Schnittstelle ein Element mit dem
        ///     angegebenen Schlüssel und Wert hinzu.
        /// </summary>
        /// <param name="key">Das Objekt, das als Schlüssel für das hinzuzufügende Element verwendet werden soll.</param>
        /// <param name="value">Das Objekt, das als Wert für das hinzuzufügende Element verwendet werden soll.</param>
        public void Add(TKey key, TValue value)
        {
            Expire();
            items.Add(key, value);
        }

        /// <summary>Entfernt alle Elemente aus <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        public void Clear() { items.Clear(); }

        /// <summary>Bestimmt, ob <see cref="T:System.Collections.Generic.ICollection`1" /> einen bestimmten Wert enthält.</summary>
        /// <param name="item">Das im <see cref="T:System.Collections.Generic.ICollection`1" /> zu suchende Objekt.</param>
        /// <returns>
        ///     true, wenn sich <paramref name="item" /> in <see cref="T:System.Collections.Generic.ICollection`1" />
        ///     befindet, andernfalls false.
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            Expire();
            return ((IDictionary<TKey, TValue>) items).Contains(item);
        }

        /// <summary>
        ///     Ermittelt, ob <see cref="T:System.Collections.Generic.IDictionary`2" /> ein Element mit dem angegebenen
        ///     Schlüssel enthält.
        /// </summary>
        /// <param name="key">Der im <see cref="T:System.Collections.Generic.IDictionary`2" /> zu suchende Schlüssel.</param>
        /// <returns>
        ///     true, wenn das <see cref="T:System.Collections.Generic.IDictionary`2" /> ein Element mit dem Schlüssel
        ///     enthält, andernfalls false.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            Expire();
            return items.ContainsKey(key);
        }

        /// <summary>
        ///     Kopiert die Elemente von <see cref="T:System.Collections.Generic.ICollection`1" /> in ein
        ///     <see cref="T:System.Array" />, beginnend bei einem bestimmten <see cref="T:System.Array" />-Index.
        /// </summary>
        /// <param name="array">
        ///     Das eindimensionale <see cref="T:System.Array" />, das das Ziel der aus
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> kopierten Elemente ist.Für <see cref="T:System.Array" />
        ///     muss eine nullbasierte Indizierung verwendet werden.
        /// </param>
        /// <param name="arrayIndex">Der nullbasierte Index in <paramref name="array" />, an dem das Kopieren beginnt.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Expire();
            ((IDictionary<TKey, TValue>) items).CopyTo(array, arrayIndex);
        }

        /// <summary>Gibt einen Enumerator zurück, der die Auflistung durchläuft.</summary>
        /// <returns>
        ///     Ein <see cref="T:System.Collections.Generic.IEnumerator`1" />, der zum Durchlaufen der Auflistung verwendet
        ///     werden kann.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IDictionary<TKey, TValue>) items).GetEnumerator();

        /// <summary>
        ///     Entfernt das erste Vorkommen eines bestimmten Objekts aus
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">Das aus dem <see cref="T:System.Collections.Generic.ICollection`1" /> zu entfernende Objekt.</param>
        /// <returns>
        ///     true, wenn <paramref name="item" /> erfolgreich aus <see cref="T:System.Collections.Generic.ICollection`1" />
        ///     gelöscht wurde, andernfalls false.Diese Methode gibt auch dann false zurück, wenn <paramref name="item" /> nicht in
        ///     der ursprünglichen <see cref="T:System.Collections.Generic.ICollection`1" /> gefunden wurde.
        /// </returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            Expire();
            return ((IDictionary<TKey, TValue>) items).Remove(item);
        }

        /// <summary>
        ///     Entfernt das Element mit dem angegebenen Schlüssel aus dem
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">Der Schlüssel des zu entfernenden Elements.</param>
        /// <returns>
        ///     true, wenn das Element erfolgreich entfernt wurde, andernfalls false.Diese Methode gibt auch dann false
        ///     zurück, wenn <paramref name="key" /> nicht im ursprünglichen
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" /> gefunden wurde.
        /// </returns>
        public bool Remove(TKey key)
        {
            Expire();
            return items.Remove(key);
        }

        /// <summary>Ruft den dem angegebenen Schlüssel zugeordneten Wert ab.</summary>
        /// <param name="key">Der Schlüssel, dessen Wert abgerufen werden soll.</param>
        /// <param name="value">
        ///     Wenn diese Methode zurückgegeben wird, enthält sie den dem angegebenen Schlüssel zugeordneten Wert,
        ///     wenn der Schlüssel gefunden wird, andernfalls enthält sie den Standardwert für den Typ des
        ///     <paramref name="value" />-Parameters.Dieser Parameter wird nicht initialisiert übergeben.
        /// </param>
        /// <returns>
        ///     true, wenn das Objekt, das <see cref="T:System.Collections.Generic.IDictionary`2" /> implementiert, ein
        ///     Element mit dem angegebenen Schlüssel enthält, andernfalls false.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            Expire();
            return items.TryGetValue(key, out value);
        }

        /// <summary>Gibt einen Enumerator zurück, der eine Auflistung durchläuft.</summary>
        /// <returns>
        ///     Ein <see cref="T:System.Collections.IEnumerator" />-Objekt, das zum Durchlaufen der Auflistung verwendet
        ///     werden kann.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<TKey, TValue>) items).GetEnumerator();

        /// <summary>Checks all items for expiration.</summary>
        public void Expire()
        {
            var now = DateTime.UtcNow.Ticks;
            if (now >= nextCheckTicks)
            {
                foreach (var item in items.ToArray())
                {
                    if (item.Value.IsExpired())
                    {
                        if (!items.Remove(item.Key))
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                }

                nextCheckTicks = now + ticksBetweenChecks;
            }
        }
    }
}
