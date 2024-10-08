﻿using System.Collections.Generic;

namespace Cave.Collections;

/// <summary>Interface for average calculations.</summary>
public interface IAverage<T> : IEnumerable<T>
{
    #region Public Properties

    /// <summary>Gets the average for the current items.</summary>
    /// <value>The average.</value>
    T Average { get; }

    /// <summary>Gets the current item count.</summary>
    /// <value>The item count.</value>
    int Count { get; }

    /// <summary>Gets or sets the maximum item count.</summary>
    /// <remarks>Setting this to zero or negative values disables the maximum item count. An update is done after next call to <see cref="Add(T)"/>.</remarks>
    /// <value>The maximum count.</value>
    int MaximumCount { get; set; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Adds the specified item.</summary>
    /// <param name="item">The item.</param>
    void Add(T item);

    /// <summary>Clears this instance.</summary>
    void Clear();

    #endregion Public Methods
}
