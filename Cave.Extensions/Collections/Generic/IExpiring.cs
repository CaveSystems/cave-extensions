﻿namespace Cave.Collections.Generic;

/// <summary>Gets an interface for expiring items.</summary>
public interface IExpiring
{
    #region Public Methods

    /// <summary>Gets a value indicating whether this instance is expired.</summary>
    /// <value><c>true</c> if this instance is expired; otherwise, <c>false</c>.</value>
    bool IsExpired();

    #endregion Public Methods
}
