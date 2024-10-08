using System.Collections.Generic;
using System.Diagnostics;

namespace Cave.Collections.Generic;

/// <summary>Provides a debugger view for ICollection{KeyValuePair{TKey, TValue}} implementations.</summary>
/// <typeparam name="TKey">Type of key</typeparam>
/// <typeparam name="TValue">Type of value</typeparam>
public sealed class CollectionDebuggerView<TKey, TValue>
{
    #region Private Fields

    readonly ICollection<KeyValuePair<TKey, TValue>> collection;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="CollectionDebuggerView{TKey, TValue}"/> class.</summary>
    /// <param name="collection"></param>
    public CollectionDebuggerView(ICollection<KeyValuePair<TKey, TValue>> collection) => this.collection = collection;

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets all items.</summary>
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

    #endregion Public Properties
}
