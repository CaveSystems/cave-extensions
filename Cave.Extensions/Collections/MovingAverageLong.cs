﻿using System.Collections;
using System.Collections.Generic;

namespace Cave.Collections
{
    /// <summary>Provides a simple moving average calculation.</summary>
    /// <seealso cref="IAverage{T}" />
    public class MovingAverageLong : IAverage<long>
    {
        readonly LinkedList<long> items = new();
        long total;

        #region IAverage<long> Members

        /// <summary>Adds the specified item.</summary>
        /// <param name="item">The item.</param>
        public void Add(long item)
        {
            total += item;
            items.AddLast(item);
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
        public long Average => total / items.Count;

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
        /// <see cref="Add(long)" />.
        /// </remarks>
        public int MaximumCount { get; set; }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<long> GetEnumerator() => items.GetEnumerator();

        #endregion
    }
}
