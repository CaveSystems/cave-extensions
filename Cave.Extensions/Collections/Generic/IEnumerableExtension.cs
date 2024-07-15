using System.Collections.Generic;
using System.Linq;

namespace Cave.Collections.Generic;

/// <summary>Gets extensions to the IEnumerable interface.</summary>
// todo: move this to Cave.IEnumerableExtension in v4.0
public static class IEnumerableExtension
{
    #region Public Methods

    /// <summary>Converts to a list.</summary>
    /// <param name="items">The items.</param>
    /// <returns></returns>
    public static List<T> AsList<T>(this IEnumerable<T> items)
    {
        if (items is not List<T> result)
        {
            result = items.ToList();
        }

        return result;
    }

    /// <summary>Converts to a set.</summary>
    /// <param name="items">The items.</param>
    /// <returns></returns>
    public static IItemSet<T> AsSet<T>(this IEnumerable<T> items)
    {
        if (items is not IItemSet<T> result)
        {
            result = new Set<T>(items);
        }

        return result;
    }

    /// <summary>Converts to a set.</summary>
    /// <param name="items">The items.</param>
    /// <returns></returns>
    public static IItemSet<T> ToSet<T>(this IEnumerable<T> items) => new Set<T>(items);

    #endregion Public Methods
}
