namespace Cave.Collections.Generic
{
    /// <summary>Gets an interface for values with key.</summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface IKeyValue<TKey>
        where TKey : struct
    {
        /// <summary>Gets the unique key / identifier for this instance.</summary>
        /// <value>The key / identifier.</value>
        TKey Key { get; }
    }
}
