using System.Collections;
using System.Collections.Generic;

namespace Cave.Collections;

/// <summary>Provides a simple moving average calculation.</summary>
/// <seealso cref="IAverage{T}" />
public class MovingAverageFloat : IAverage<float>
{
    #region Fields

    readonly LinkedList<float> items = new();
    float total;

    #endregion

    #region IAverage<float> Members

    /// <summary>Adds the specified item.</summary>
    /// <param name="item">The item.</param>
    public void Add(float item)
    {
        total += item;
        _ = items.AddLast(item);
        if (MaximumCount > 0)
        {
            while (items.Count > MaximumCount)
            {
                total -= items.First.Value;
                items.RemoveFirst();
            }
        }
    }

    /// <summary>Gets the average for the current items.</summary>
    /// <value>The average.</value>
    public float Average => total / items.Count;

    /// <summary>Clears this instance.</summary>
    public void Clear()
    {
        total = 0;
        items.Clear();
    }

    /// <summary>Gets the current item count.</summary>
    /// <value>The item count.</value>
    public int Count => items.Count;

    /// <summary>Gets or sets the maximum item count.</summary>
    /// <value>The maximum count.</value>
    /// <remarks>
    /// Setting this to zero or negative values disables the maximum item count. An update is done after next call to
    /// <see cref="Add(float)" />.
    /// </remarks>
    public int MaximumCount { get; set; }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<float> GetEnumerator() => items.GetEnumerator();

    #endregion
}
