using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cave.Collections
{
    /// <summary>Gets a simple moving average calculation.</summary>
    /// <seealso cref="IAverage{T}" />
    [SuppressMessage("Naming", "CA1710")]
    public class ExpiringMovingAverageFloat : IAverage<float>
    {
        readonly LinkedList<Item> items = new LinkedList<Item>();
        float total;

        /// <summary>Gets or sets the maximum age of the items.</summary>
        /// <value>The maximum age.</value>
        /// <remarks>
        ///     Setting this to zero or negative values disables the maximum age. An update is done after next call to
        ///     <see cref="Add(float)" />.
        /// </remarks>
        public TimeSpan MaximumAge { get; set; }

        /// <summary>Gets the duration of the items.</summary>
        /// <value>The duration.</value>
        public TimeSpan Duration
        {
            get
            {
                if (items.Count > 1)
                {
                    return items.Last.Value.DateTime - items.First.Value.DateTime;
                }

                return TimeSpan.Zero;
            }
        }

        /// <summary>Gets the average for the current items.</summary>
        /// <value>The average.</value>
        public float Average => total / items.Count;

        /// <summary>Gets or sets the maximum item count.</summary>
        /// <value>The maximum count.</value>
        /// <remarks>
        ///     Setting this to zero or negative values disables the maximum item count. An update is done after next call to
        ///     <see cref="Add(float)" />.
        /// </remarks>
        public int MaximumCount { get; set; }

        /// <summary>Gets the current item count.</summary>
        /// <value>The item count.</value>
        public int Count => items.Count;

        /// <summary>Adds the specified item.</summary>
        /// <param name="item">The item.</param>
        public void Add(float item)
        {
            items.AddLast(new Item { DateTime = DateTime.UtcNow, Float = item });
            total += item;
            if (MaximumCount > 0)
            {
                while (items.Count > MaximumCount)
                {
                    total -= items.First.Value.Float;
                    items.RemoveFirst();
                }
            }

            if (MaximumAge > TimeSpan.Zero)
            {
                var keepAfter = DateTime.UtcNow - MaximumAge;
                while (items.First.Value.DateTime < keepAfter)
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

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => items.Select(i => i.Float).GetEnumerator();

        class Item
        {
            public DateTime DateTime;
            public float Float;
        }
    }
}
