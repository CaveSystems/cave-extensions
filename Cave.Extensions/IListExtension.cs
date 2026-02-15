using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cave;

/// <summary>Gets extensions to the IList and IList{T} interfaces.</summary>
public static class IListExtension
{
    #region Public Methods

    /// <summary>Returns a read-only wrapper for the specified list.</summary>
    /// <typeparam name="T">The type of the elements of the array.</typeparam>
    /// <param name="items">The items to wrap in a read-only wrapper.</param>
    /// <returns>A read-only wrapper for the specified array.</returns>
    public static IList<T> AsReadOnly<T>(this IList<T> items) => items.IsReadOnly ? items : new ReadOnlyCollection<T>(items);

    /// <summary>Returns a read-only wrapper for the specified list.</summary>
    /// <typeparam name="T">The type of the elements of the array.</typeparam>
    /// <param name="items">The items to wrap in a read-only wrapper.</param>
    /// <returns>A read-only wrapper for the specified array.</returns>
    public static IList<T> AsReadOnly<T>(this T[] items) => new ReadOnlyCollection<T>(items);

    /// <summary>Returns a reverse iterator for the specified source.</summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <param name="source">Source list</param>
    /// <returns>Returns a reverse iteration</returns>
    public static IEnumerable<TSource> Reverse<TSource>(this IList<TSource> source)
    {
        for (var i = source.Count - 1; i >= 0; i--)
        {
            yield return source[i];
        }
    }

    #endregion Public Methods
}
