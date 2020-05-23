using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cave
{
    /// <summary>
    /// Provides extensions to the IList and IList{T} interfaces.
    /// </summary>
    public static class IListExtension
    {
        /// <summary>
        /// Returns a read-only wrapper for the specified list.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <param name="items">The items to wrap in a read-only wrapper.</param>
        /// <returns>A read-only wrapper for the specified array.</returns>
        public static IList<T> AsReadOnly<T>(this IList<T> items) => new ReadOnlyCollection<T>(items);

        /// <summary>
        /// Returns a read-only wrapper for the specified list.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <param name="items">The items to wrap in a read-only wrapper.</param>
        /// <returns>A read-only wrapper for the specified array.</returns>
        public static IList<T> AsReadOnly<T>(this T[] items) => new ReadOnlyCollection<T>(items);
    }
}
