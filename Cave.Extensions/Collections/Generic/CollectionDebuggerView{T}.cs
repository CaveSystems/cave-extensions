using System.Collections.Generic;
using System.Diagnostics;

namespace Cave.Collections.Generic;

/// <summary>Provides a debug view for collections.</summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public sealed class CollectionDebuggerView<T>
{
    #region Private Fields

    readonly ICollection<T> collection;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="CollectionDebuggerView{T}"/> class.</summary>
    /// <param name="collection">The collection to provide a debug view for.</param>
    public CollectionDebuggerView(ICollection<T> collection) => this.collection = collection;

    #endregion Public Constructors

    #region Public Properties

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

    #endregion Public Properties
}
