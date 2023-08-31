#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Collections.Generic;

public interface ISet<T> : ICollection<T>
{
    #region Members

    new bool Add(T item);

    void ExceptWith(IEnumerable<T> other);

    void IntersectWith(IEnumerable<T> other);

    bool IsProperSubsetOf(IEnumerable<T> other);

    bool IsProperSupersetOf(IEnumerable<T> other);

    bool IsSubsetOf(IEnumerable<T> other);

    bool IsSupersetOf(IEnumerable<T> other);

    bool Overlaps(IEnumerable<T> other);

    bool SetEquals(IEnumerable<T> other);

    void SymmetricExceptWith(IEnumerable<T> other);

    void UnionWith(IEnumerable<T> other);

    #endregion Members
}
