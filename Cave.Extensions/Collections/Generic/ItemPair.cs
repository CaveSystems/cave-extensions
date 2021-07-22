namespace Cave.Collections.Generic
{
    /// <summary>Gets a struct with two typed objects.</summary>
    /// <typeparam name="T1">The type of the first object.</typeparam>
    /// <typeparam name="T2">The type of the second object.</typeparam>
    public class ItemPair<T1, T2>
    {
        #region Constructors

        /// <summary>Creates a new instance with the specified values.</summary>
        /// <param name="value1">First value.</param>
        /// <param name="value2">Second value.</param>
        public ItemPair(T1 value1, T2 value2)
        {
            A = value1;
            B = value2;
        }

        #endregion

        #region Properties

        /// <summary>Gets the first value.</summary>
        public T1 A { get; }

        /// <summary>Gets the second value.</summary>
        public T2 B { get; }

        #endregion

        #region Overrides

        /// <summary>Checks another ItemPair{T1, T2} for equality.</summary>
        /// <param name="obj">The other instance to check.</param>
        /// <returns>Returns true if the other instance equals this one, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is not ItemPair<T1, T2>)
            {
                return false;
            }

            var other = (ItemPair<T1, T2>)obj;
            return Equals(other.A, A) && Equals(other.B, B);
        }

        /// <summary>Gets the hash code for this instance.</summary>
        /// <returns></returns>
        public override int GetHashCode() => A.GetHashCode() ^ B.GetHashCode();

        /// <summary>Gets a string "A B".</summary>
        /// <returns></returns>
        public override string ToString() => $"{A} {B}";

        #endregion
    }
}
