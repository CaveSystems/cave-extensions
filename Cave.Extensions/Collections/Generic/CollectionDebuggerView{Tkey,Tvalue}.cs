using System.Collections.Generic;
using System.Diagnostics;

namespace Cave.Collections.Generic
{
    /// <summary>
    /// Provides a debugger view for ICollection{KeyValuePair{TKey, TValue}} implementations.
    /// </summary>
    /// <typeparam name="TKey">Type of key</typeparam>
    /// <typeparam name="TValue">Type of value</typeparam>
    public sealed class CollectionDebuggerView<TKey, TValue>
    {
        ICollection<KeyValuePair<TKey, TValue>> collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionDebuggerView{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="collection"></param>
        public CollectionDebuggerView(ICollection<KeyValuePair<TKey, TValue>> collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IList<KeyValuePair<TKey, TValue>> Items
        {
            get
            {
                var result = new KeyValuePair<TKey, TValue>[collection.Count];
                collection.CopyTo(result, 0);
                return result;
            }
        }
    }
}
