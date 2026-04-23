using System.Diagnostics.CodeAnalysis;

namespace Cave;

/// <summary>Gets the international system of units default units.</summary>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter")]
public enum SiUnit
{
    /// <summary>kilo</summary>
    k = 1,

    /// <summary>Mega</summary>
    M,

    /// <summary>Giga</summary>
    G,

    /// <summary>Tera</summary>
    T,

    /// <summary>Peta</summary>
    P,

    /// <summary>Exa</summary>
    E,

    /// <summary>Zetta</summary>
    Z,

    /// <summary>Yota</summary>
    Y
}
