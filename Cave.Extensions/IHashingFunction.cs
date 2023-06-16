using System;

namespace Cave;

/// <summary>Provides an interface for hashing functions</summary>
public interface IHashingFunction
{
    #region Members

    /// <summary>Adds the hash (item.GetHashCode()) to the combined hash.</summary>
    /// <typeparam name="T">Items type</typeparam>
    /// <param name="item">Item to add</param>
    void Add<T>(T item);

    /// <summary>Feeds the specified binary data to the hashing function.</summary>
    /// <param name="data">Data to add</param>
    void Feed(byte[] data);

    /// <summary>Feeds specified binary data to the hashing function.</summary>
    /// <param name="data">Data to add</param>
    /// <param name="length"></param>
    unsafe void Feed(byte* data, int length);

    /// <summary>Gets the combined hashcode.</summary>
    /// <returns></returns>
    int ToHashCode();

    #endregion
}
