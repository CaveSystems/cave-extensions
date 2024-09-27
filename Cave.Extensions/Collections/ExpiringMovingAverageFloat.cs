using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cave.Collections;

/// <summary>Gets a simple moving average calculation.</summary>
/// <seealso cref="IAverage{T}"/>
public class ExpiringMovingAverageFloat : IAverage<float>
{
    #region Private Classes

    sealed class Item
    {
        #region Public Fields

        public DateTime DateTime;
        public float Float;

        #endregion Public Fields
    }

    #endregion Private Classes

    #region Private Fields

    readonly LinkedList<Item> items = new();
    float total;

    #endregion Private Fields

    #region Public Properties

    /// <summary>Gets the average for the current items.</summary>
    /// <value>The average.</value>
    public float Average => total / items.Count;

    /// <summary>Gets the current item count.</summary>
    /// <value>The item count.</value>
    public int Count => items.Count;

    /// <summary>Gets the duration of the items.</summary>
    /// <value>The duration.</value>
    public TimeSpan Duration => items.Count > 1 ? items.Last!.Value.DateTime - items.First!.Value.DateTime : TimeSpan.Zero;

    /// <summary>Gets or sets the maximum age of the items.</summary>
    /// <value>The maximum age.</value>
    /// <remarks>Setting this to zero or negative values disables the maximum age. An update is done after next call to <see cref="Add(float)"/>.</remarks>
    public TimeSpan MaximumAge { get; set; }

    /// <summary>Gets or sets the maximum item count.</summary>
    /// <value>The maximum count.</value>
    /// <remarks>Setting this to zero or negative values disables the maximum item count. An update is done after next call to <see cref="Add(float)"/>.</remarks>
    public int MaximumCount { get; set; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Adds the specified item.</summary>
    /// <param name="item">The item.</param>
    public void Add(float item)
    {
        _ = items.AddLast(new Item { DateTime = DateTime.UtcNow, Float = item });
        total += item;
        if (MaximumCount > 0)
        {
            while (items.Count > MaximumCount)
            {
                total -= items.First!.Value.Float;
                items.RemoveFirst();
            }
        }

        if (MaximumAge > TimeSpan.Zero)
        {
            var keepAfter = DateTime.UtcNow - MaximumAge;
            while (items.First!.Value.DateTime < keepAfter)
            {
                total -= items.First.Value.Float;
                items.RemoveFirst();
            }
        }
    }

    /// <summary>Clears this instance.</summary>
    public void Clear()
    {
        items.Clear();
        total = 0;
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<float> GetEnumerator() => items.Select(i => i.Float).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => items.Select(i => i.Float).GetEnumerator();

    #endregion Public Methods
}
