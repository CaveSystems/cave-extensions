#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20

using System.Collections.Generic;

namespace System.Linq
{
    public interface ILookup<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>
    {
        #region Properties

        int Count { get; }
        IEnumerable<TElement> this[TKey key] { get; }

        #endregion Properties

        #region Members

        bool Contains(TKey key);

        #endregion Members
    }
}

#endif
