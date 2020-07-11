using System;

namespace Cave
{
    /// <summary>
    /// Provides flags for Base64 encoding/decoding
    /// </summary>
    [Flags]
    public enum Base64Flags : int
    {
        /// <summary>
        /// No padding, default charset
        /// </summary>
        None = 0,

        /// <summary>
        /// En-/Decode with padding
        /// </summary>
        Padding = 1,

        /// <summary>
        /// Use url safe character set instead if the default character set
        /// </summary>
        UrlChars = 2,
    }
}
