#pragma warning disable SA1300 // Element must begin with upper-case letter

namespace Cave
{
    /// <summary>
    /// Provides common IEC units for binary values (byte)
    /// </summary>
    public enum IecUnits : int
    {
        /// <summary>
        /// Byte
        /// </summary>
        B = 0,

        /// <summary>
        /// kilo Byte
        /// </summary>
        kiB,

        /// <summary>
        /// Mega Byte
        /// </summary>
        MiB,

        /// <summary>
        /// Giga Byte
        /// </summary>
        GiB,

        /// <summary>
        /// Tera Byte
        /// </summary>
        TiB,

        /// <summary>
        /// Peta Byte
        /// </summary>
        PiB,

        /// <summary>
        /// Exa Byte
        /// </summary>
        EiB,

        /// <summary>
        /// Zetta Byte
        /// </summary>
        ZiB,

        /// <summary>
        /// Yotta Byte
        /// </summary>
        YiB,
    }
}

#pragma warning restore SA1300 // Element must begin with upper-case letter
