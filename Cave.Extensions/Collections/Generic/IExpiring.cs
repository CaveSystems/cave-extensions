namespace Cave.Collections.Generic;

/// <summary>Gets an interface for expiring items.</summary>
public interface IExpiring
{
    #region Public Methods

    /// <summary>Gets a value indicating whether this instance is expired.</summary>
    /// <returns>Returns true if this instance is expired; otherwise, false.</returns>
    bool IsExpired();

    #endregion Public Methods
}
