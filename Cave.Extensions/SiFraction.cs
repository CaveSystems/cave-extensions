using System.Diagnostics.CodeAnalysis;

namespace Cave;

/// <summary>si unit fractions.</summary>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter")]
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
