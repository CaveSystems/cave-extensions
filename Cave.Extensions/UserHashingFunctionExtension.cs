using System.Collections.Generic;

namespace Cave;

/// <summary>Provides extensions to the <see cref="IHashingFunction" /> interface.</summary>
public static class UserHashingFunctionExtension
{
    #region Static

    /// <summary>Adds the hash (item.GetHashCode()) of each item to the combined hash.</summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <param name="hashingFunction">Hashing function to use.</param>
    /// <param name="items">Items to add</param>
    public static void AddRange<T>(this IHashingFunction hashingFunction, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            hashingFunction.Add(item);
        }
    }

    #endregion
}
