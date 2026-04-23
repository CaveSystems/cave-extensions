#if NET20
#pragma warning disable SA1600 // No comments for backports
#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

using System.Collections.Generic;

namespace System.Linq;

public interface IGrouping<TKey, TElement> : IEnumerable<TElement>
{
    #region Properties

    TKey Key { get; }

    #endregion Properties
}

#endif
