using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cave.Collections;

/// <summary>Gets extensions to the IEnumerable interface.</summary>
public static class IEnumerableExtension
{
    #region Public Methods

    /// <summary>Converts to a list.</summary>
    /// <param name="enumerable">The <see cref="IEnumerable"/> instance to convert.</param>
    /// <returns>Returns a new <see cref="ArrayList"/> instance.</returns>
    [SuppressMessage("Style", "IDE0058:Expression value is never used")]
    public static ArrayList ToArrayList(this IEnumerable enumerable)
    {
        var result = new ArrayList();
        foreach (var item in enumerable)
        {
            result.Add(item);
        }
        return result;
    }

    /// <summary>Converts to an array of <see cref="object"/>.</summary>
    /// <param name="enumerable">The <see cref="IEnumerable"/> instance to convert.</param>
    /// <returns>Returns a new array of <see cref="object"/></returns>
    public static object[] ToObjectArray(this IEnumerable enumerable) => enumerable.Cast<object>().ToArray();

    #endregion Public Methods
}
