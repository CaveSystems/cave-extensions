namespace Cave
{
    /// <summary>
    /// Interface to compute checksums on bytes/byte arrays.
    /// </summary>
    /// <typeparam name="T">Type of byte array.</typeparam>
    public interface IChecksum<T>
    {
        #region Public Properties

        /// <summary>
        /// Gets the checksum computed so far.
        /// </summary>
        T Value { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Resets the checksum to initialization state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Adds one byte to the checksum.
        /// </summary>
        /// <param name="value">the byte to add. Only the lowest 8 bits will be used.</param>
        void Update(int value);

        /// <summary>
        /// Updates the checksum with the specified byte array.
        /// </summary>
        /// <param name="buffer">buffer an array of bytes.</param>
        void Update(byte[] buffer);

        /// <summary>
        /// Updates the checksum with the specified byte array.
        /// </summary>
        /// <param name="buffer">The buffer containing the data.</param>
        /// <param name="offset">The offset in the buffer where the data starts.</param>
        /// <param name="count">the number of data bytes to add.</param>
        void Update(byte[] buffer, int offset, int count);

        #endregion Public Methods
    }
}
