using System.Reflection;

namespace Cave;

/// <summary>Provides extensions to the <see cref="PropertyInfo" /> class.</summary>
public static class PropertyInfoExtension
{
    #region Static

    /// <summary>Checks whether the specified property is an index property or not.</summary>
    /// <param name="property">Property to check.</param>
    /// <returns>Returns true if the specified property is an index property or false otherwise.</returns>
    public static bool IsIndexProperty(this PropertyInfo property) => property is not null && (property.GetIndexParameters().Length > 0);

    #endregion
}
