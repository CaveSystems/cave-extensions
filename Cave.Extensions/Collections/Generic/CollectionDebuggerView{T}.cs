using System.Collections.Generic;
using System.Diagnostics;

namespace Cave.Collections.Generic
{
    /// <summary>Provides a debug view for collections.</summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CollectionDebuggerView<T>
    {
        readonly ICollection<T> collection;

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="CollectionDebuggerView{T}" /> class.</summary>
        /// <param name="collection"></param>
        public CollectionDebuggerView(ICollection<T> collection) => this.collection = collection;

        #endregion

        #region Properties

        /// <summary>Gets all items.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IList<T> Items
        {
            get
            {
                var result = new T[collection.Count];
                collection.CopyTo(result, 0);
                return result;
            }
        }

        #endregion
    }
}
