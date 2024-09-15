using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cave;

/// <summary>Provides extensions to the <see cref="IHashingFunction"/> interface.</summary>
public static class IHashingFunctionExtension
{
    #region Public Methods

    /// <summary>Adds the hash (item.GetHashCode()) of the item to the combined hash (this function evaluates <see cref="IEnumerable"/> and byte[] instances).</summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <param name="hashingFunction">Hashing function to use.</param>
    /// <param name="item">Item to add</param>
    [MethodImpl((MethodImplOptions)0x0100)]
    public static unsafe void Add<T>(this IHashingFunction hashingFunction, T item)
    {
        switch (item)
        {
            case null: hashingFunction.Feed(0); break;
            case byte[] buffer: hashingFunction.Feed(buffer); break;
            case IEnumerable enumerable: hashingFunction.AddRange(enumerable); break;
            default: hashingFunction.Feed(item.GetHashCode()); break;
        }
    }

    /// <summary>Adds the hash (item.GetHashCode()) of each item to the combined hash (this function evaluates <see cref="IEnumerable"/> and byte[] instances).</summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <param name="hashingFunction">Hashing function to use.</param>
    /// <param name="items">Items to add</param>
    [MethodImpl((MethodImplOptions)0x0100)]
    public static void AddRange<T>(this IHashingFunction hashingFunction, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            hashingFunction.Add(item);
        }
    }

    /// <summary>Adds the hash (item.GetHashCode()) of each item to the combined hash (this evaluates <see cref="IEnumerable"/> and byte[] instances).</summary>
    /// <param name="hashingFunction">Hashing function to use.</param>
    /// <param name="items">Items to add</param>
    [MethodImpl((MethodImplOptions)0x0100)]
    public static void AddRange(this IHashingFunction hashingFunction, IEnumerable items)
    {
        foreach (var item in items)
        {
            hashingFunction.Add(item);
        }
    }

    #endregion Public Methods
}
