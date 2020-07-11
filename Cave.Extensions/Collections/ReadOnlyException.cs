#if NETSTANDARD13
namespace System.Data
{
    /// <summary>
    /// Represents the exception that is thrown when you try to change the value of a read-only column.
    /// </summary>
    public class ReadOnlyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyException"/> class.
        /// </summary>
        public ReadOnlyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public ReadOnlyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ReadOnlyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
#endif
