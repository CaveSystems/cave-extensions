namespace Cave.Collections.Generic
{
    /// <summary>
    /// Provides an interface for expiring items.
    /// </summary>
    public interface IExpiring
    {
        /// <summary>Gets a value indicating whether this instance is expired.</summary>
        /// <value>
        /// <c>true</c> if this instance is expired; otherwise, <c>false</c>.
        /// </value>
        bool IsExpired();
    }
}
