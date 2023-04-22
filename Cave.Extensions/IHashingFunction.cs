namespace Cave;

/// <summary>Provides an interface for hashing functions</summary>
public interface IHashingFunction
{
    #region Members

    /// <summary>Adds the hash (item.GetHashCode()) to the combined hash.</summary>
    /// <typeparam name="T">Items type</typeparam>
    /// <param name="item">Item to add</param>
    void Add<T>(T item);

    /// <summary>Gets the combined hashcode.</summary>
    /// <returns></returns>
    int ToHashCode();

    #endregion
}
