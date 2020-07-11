using System.Collections;
using System.Collections.Generic;

namespace Cave.Collections
{
    /// <summary>Provides a simple moving average calculation.</summary>
    /// <seealso cref="IAverage{T}" />
    public class MovingAverageFloat : IAverage<float>
    {
        LinkedList<float> items = new LinkedList<float>();
        float total;

        /// <summary>Gets the average for the current items.</summary>
        /// <value>The average.</value>
        public float Average => total / items.Count;

        /// <summary>Gets or sets the maximum item count.</summary>
        /// <value>The maximum count.</value>
        /// <remarks>Setting this to zero or negative values disables the maximum item count. An update is done after next call to <see cref="Add(float)"/>.</remarks>
        public int MaximumCount { get; set; }

        /// <summary>Gets the current item count.</summary>
        /// <value>The item count.</value>
        public int Count => items.Count;

        /// <summary>Adds the specified item.</summary>
        /// <param name="item">The item.</param>
        public void Add(float item)
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

        /// <summary>Clears this instance.</summary>
        public void Clear()
        {
            total = 0;
            items.Clear();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<float> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}
