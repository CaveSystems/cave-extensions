using System.Collections.Generic;
using System.Diagnostics;

namespace Cave.Collections.Generic
{
    /// <summary>
    /// Provides a debug view for collections.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CollectionDebuggerView<T>
    {
        ICollection<T> collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionDebuggerView{T}"/> class.
        /// </summary>
        /// <param name="collection"></param>
        public CollectionDebuggerView(ICollection<T> collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] result = new T[collection.Count];
                collection.CopyTo(result, 0);
                return result;
            }
        }
    }
}
