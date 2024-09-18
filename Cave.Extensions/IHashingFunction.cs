namespace Cave;

/// <summary>Provides an interface for hashing functions</summary>
public interface IHashingFunction
{
    #region Public Methods

    /// <summary>Adds the hash to the combined hash.</summary>
    /// <param name="hash">Hash to add</param>
    void Feed(int hash);

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

    #endregion Public Methods
}
