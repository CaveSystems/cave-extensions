using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cave.Collections
{
    /// <summary>Gets a simple moving average calculation.</summary>
    /// <seealso cref="IAverage{T}" />
    public class ExpiringMovingAverageLong : IAverage<long>
    {
        #region Nested type: Item

        class Item
        {
            public DateTime DateTime;
            public long Long;
        }

        #endregion

        readonly LinkedList<Item> items = new();
        long total;

        #region Properties

        /// <summary>Gets or sets the maximum age of the items.</summary>
        /// <value>The maximum age.</value>
        /// <remarks>Setting this to zero or negative values disables the maximum age. An update is done after next call to <see cref="Add(long)" />.</remarks>
        public TimeSpan MaximumAge { get; set; }

        #endregion

        #region IAverage<long> Members

        /// <summary>Adds the specified item.</summary>
        /// <param name="item">The item.</param>
        public void Add(long item)
        {
            items.AddLast(new Item
            {
                DateTime = DateTime.UtcNow,
                Long = item
            });
            total += item;
            if (MaximumCount > 0)
            {
                while (items.Count > MaximumCount)
                {
                    total -= items.First.Value.Long;
                    items.RemoveFirst();
                }
            }

            if (MaximumAge > TimeSpan.Zero)
            {
                var keepAfter = DateTime.UtcNow - MaximumAge;
                while (items.First.Value.DateTime < keepAfter)
                {
                    total -= items.First.Value.Long;
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
            items.Clear();
            total = 0;
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
        IEnumerator IEnumerable.GetEnumerator() => items.Select(i => i.Long).GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<long> GetEnumerator() => items.Select(i => i.Long).GetEnumerator();

        #endregion
    }
}
