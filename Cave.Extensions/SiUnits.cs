using System.Diagnostics.CodeAnalysis;

#pragma warning disable SA1300 // Element must begin with upper-case letter

namespace Cave
{
    /// <summary>
    /// Provides the international system of units default units.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Does not make sense here.")]
    public enum SiUnits : int
    {
        /// <summary>
        /// kilo
        /// </summary>
        k = 1,

        /// <summary>
        /// Mega
        /// </summary>
        M,

        /// <summary>
        /// Giga
        /// </summary>
        G,

        /// <summary>
        /// Tera
        /// </summary>
        T,

        /// <summary>
        /// Peta
        /// </summary>
        P,

        /// <summary>
        /// Exa
        /// </summary>
        E,

        /// <summary>
        /// Zetta
        /// </summary>
        Z,

        /// <summary>
        /// Yota
        /// </summary>
        Y,
    }
}

#pragma warning restore SA1300 // Element must begin with upper-case letter
