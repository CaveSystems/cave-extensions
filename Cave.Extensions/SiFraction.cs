using System.Diagnostics.CodeAnalysis;

#pragma warning disable SA1300 // Element must begin with upper-case letter

namespace Cave
{
    /// <summary>si unit fractions.</summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Does not make sense here.")]
    public enum SiFraction
    {
        /// <summary>Milli</summary>
        m = 1,

        /// <summary>Micro</summary>
        µ,

        /// <summary>Nano</summary>
        n,

        /// <summary>Pico</summary>
        p,

        /// <summary>Femto</summary>
        f,

        /// <summary>Atto</summary>
        a,

        /// <summary>Zepto</summary>
        z,

        /// <summary>Yocto</summary>
        y
    }
}

#pragma warning restore SA1300 // Element must begin with upper-case letter
