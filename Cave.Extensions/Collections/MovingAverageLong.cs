﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Cave.Collections;

/// <summary>Provides a simple moving average calculation.</summary>
/// <seealso cref="IAverage{T}"/>
public class MovingAverageLong : IAverage<long>
{
    #region Private Fields

    readonly LinkedList<long> items = new();
    long total;

    #endregion Private Fields

    #region Public Properties

    /// <summary>Gets the average for the current items.</summary>
    /// <value>The average.</value>
    public long Average => total / Math.Max(1, items.Count);

    /// <summary>Gets the current item count.</summary>
    /// <value>The item count.</value>
    public int Count => items.Count;

    /// <summary>Gets or sets the maximum item count.</summary>
    /// <value>The maximum count.</value>
    /// <remarks>Setting this to zero or negative values disables the maximum item count. An update is done after next call to <see cref="Add(long)"/>.</remarks>
    public int MaximumCount { get; set; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Adds the specified item.</summary>
    /// <param name="item">The item.</param>
    public void Add(long item)
    {
        total += item;
        _ = items.AddLast(item);
        if (MaximumCount > 0)
        {
            while (items.Count > MaximumCount)
            {
                total -= items.First!.Value;
                items.RemoveFirst();
            }
        }
    }

    /// <summary>Clears this instance.</summary>
    public void Clear()
    {
        total = 0;
        items.Clear();
    }

    /// <inheritdoc/>
    public IEnumerator<long> GetEnumerator() => items.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

    #endregion Public Methods
}
