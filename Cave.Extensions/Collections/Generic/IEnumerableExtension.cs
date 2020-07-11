using System.Collections.Generic;
using System.Linq;

namespace Cave.Collections.Generic
{
    /// <summary>
    /// Provides extensions to the IEnumerable interface.
    /// </summary>
    public static class IEnumerableExtension
    {
        /// <summary>Converts to a set.</summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static IItemSet<T> AsSet<T>(this IEnumerable<T> items)
        {
            IItemSet<T> result = items as IItemSet<T>;
            if (result == null)
            {
                result = new Set<T>(items);
            }

            return result;
        }

        /// <summary>Converts to a set.</summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static IItemSet<T> ToSet<T>(this IEnumerable<T> items)
        {
            return new Set<T>(items);
        }

        /// <summary>Converts to a list.</summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static List<T> AsList<T>(this IEnumerable<T> items)
        {
            List<T> result = items as List<T>;
            if (result == null)
            {
                result = items.ToList();
            }

            return result;
        }
    }
}
