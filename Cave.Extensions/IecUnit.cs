using System.Diagnostics.CodeAnalysis;

namespace Cave;

/// <summary>Gets common IEC units for binary values (byte).</summary>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter")]
public enum IecUnit
{
    /// <summary>Byte</summary>
    B = 0,

    /// <summary>kilo Byte</summary>
    kiB,

    /// <summary>Mega Byte</summary>
    MiB,

    /// <summary>Giga Byte</summary>
    GiB,

    /// <summary>Tera Byte</summary>
    TiB,

    /// <summary>Peta Byte</summary>
    PiB,

    /// <summary>Exa Byte</summary>
    EiB,

    /// <summary>Zetta Byte</summary>
    ZiB,

    /// <summary>Yotta Byte</summary>
    YiB
}
