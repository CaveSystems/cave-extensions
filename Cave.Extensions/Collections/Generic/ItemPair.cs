namespace Cave.Collections.Generic
{
    /// <summary>
    /// Provides a struct with two typed objects.
    /// </summary>
    /// <typeparam name="T1">The type of the first object.</typeparam>
    /// <typeparam name="T2">The type of the second object.</typeparam>
    public class ItemPair<T1, T2>
    {
        /// <summary>
        /// Creates a new instance with the specified values.
        /// </summary>
        /// <param name="value1">First value.</param>
        /// <param name="value2">Second value.</param>
        public ItemPair(T1 value1, T2 value2)
        {
            A = value1;
            B = value2;
        }

        /// <summary>
        /// Obtains the first value.
        /// </summary>
        public T1 A { get; private set; }

        /// <summary>
        /// Obtains the second value.
        /// </summary>
        public T2 B { get; private set; }

        /// <summary>
        /// Obtains a string "A B".
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", A, B);
        }

        /// <summary>
        /// Obtains the hash code for this instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode();
        }

        /// <summary>
        /// Checks another ItemPair{T1, T2} for equality.
        /// </summary>
        /// <param name="obj">The other instance to check.</param>
        /// <returns>Returns true if the other instance equals this one, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ItemPair<T1, T2>))
            {
                return false;
            }

            ItemPair<T1, T2> other = (ItemPair<T1, T2>)obj;
            return Equals(other.A, A) && Equals(other.B, B);
        }
    }
}
