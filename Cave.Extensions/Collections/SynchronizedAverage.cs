using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cave.Collections
{
    /// <summary>Gets a synchronzation wrapper for <see cref="IAverage{T}" /> implementations.</summary>
    /// <seealso cref="IAverage{T}" />
    [SuppressMessage("Naming", "CA1710")]
    public class SynchronizedAverage<T> : IAverage<T>
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="SynchronizedAverage{T}" /> class.</summary>
        /// <param name="base">The base.</param>
        public SynchronizedAverage(IAverage<T> @base) => Base = @base;

        #endregion

        #region Properties

        /// <summary>Gets the base.</summary>
        /// <value>The base.</value>
        public IAverage<T> Base { get; }

        #endregion

        #region IAverage<T> Members

        /// <summary>Adds the specified item.</summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            lock (Base)
            {
                Base.Add(item);
            }
        }

        /// <summary>Gets the average for the current items.</summary>
        /// <value>The average.</value>
        public T Average
        {
            get
            {
                lock (Base)
                {
                    return Base.Average;
                }
            }
        }

        /// <summary>Clears this instance.</summary>
        public void Clear()
        {
            lock (Base)
            {
                Base.Clear();
            }
        }

        /// <summary>Gets the current item count.</summary>
        /// <value>The item count.</value>
        public int Count
        {
            get
            {
                lock (Base)
                {
                    return Base.Count;
                }
            }
        }

        /// <summary>Gets or sets the maximum item count.</summary>
        /// <value>The maximum count.</value>
        /// <remarks>
        /// Setting this to zero or negative values disables the maximum item count. An update is done after next call to
        /// <see cref="Add(T)" />.
        /// </remarks>
        public int MaximumCount
        {
            get
            {
                lock (Base)
                {
                    return Base.MaximumCount;
                }
            }
            set
            {
                lock (Base)
                {
                    Base.MaximumCount = value;
                }
            }
        }

        /// <summary>Copies the collection to a static list and returns an enumerator that iterates through the list.</summary>
        /// <returns>An enumerator that can be used to iterate through the copy of the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (Base)
            {
                return Base.ToList().GetEnumerator();
            }
        }

        /// <summary>Copies the collection to a static list and returns an enumerator that iterates through the list.</summary>
        /// <returns>An enumerator that can be used to iterate through the copy of the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            lock (Base)
            {
                return Base.ToList().GetEnumerator();
            }
        }

        #endregion
    }
}
