using System;
using System.Linq;
using System.Reflection;

namespace Cave;

/// <summary>Gets extensions for the <see cref="MemberInfo" /> class.</summary>
public static class MemberInfoExtension
{
    #region Static

    /// <summary>Gets a specific <see cref="Attribute" /> present at the member. If the attribute type cannot be found null is returned.</summary>
    /// <typeparam name="T">The attribute type to check for.</typeparam>
    /// <param name="member">The member to check.</param>
    /// <param name="inherit">Inherit attributes from parents.</param>
    /// <returns>Returns the attribute found or null.</returns>
    public static T GetAttribute<T>(this MemberInfo member, bool inherit = false)
        where T : Attribute =>
        (T)GetAttribute(member, typeof(T), inherit);

    /// <summary>Gets a specific <see cref="Attribute" /> present at the member. If the attribute type cannot be found null is returned.</summary>
    /// <param name="member">The member to check.</param>
    /// <param name="attributeType">The attribute type to check for.</param>
    /// <param name="inherit">Inherit attributes from parents.</param>
    /// <returns>Returns the attribute found or null.</returns>
    public static object GetAttribute(this MemberInfo member, Type attributeType, bool inherit = false) => member?.GetCustomAttributes(inherit).Where(a => attributeType.IsAssignableFrom(a.GetType())).FirstOrDefault();

    /// <summary>Checks a member for presence of a specific <see cref="Attribute" /> instance.</summary>
    /// <typeparam name="T">The attribute type to check for.</typeparam>
    /// <param name="member">The member to check.</param>
    /// <param name="inherit">Inherit attributes from parents.</param>
    /// <returns>Returns true if at least one attribute of the desired type could be found, false otherwise.</returns>
    public static bool HasAttribute<T>(this MemberInfo member, bool inherit = false)
        where T : Attribute =>
        HasAttribute(member, typeof(T), inherit);

    /// <summary>Checks a member for presence of a specific <see cref="Attribute" /> instance.</summary>
    /// <param name="member">The member to check.</param>
    /// <param name="attributeType">The attribute type to check for.</param>
    /// <param name="inherit">Inherit attributes from parents.</param>
    /// <returns>Returns true if at least one attribute of the desired type could be found, false otherwise.</returns>
    public static bool HasAttribute(this MemberInfo member, Type attributeType, bool inherit = false)
    {
        if (member == null)
        {
            throw new ArgumentNullException(nameof(member));
        }

        if (attributeType == null)
        {
            throw new ArgumentNullException(nameof(attributeType));
        }

        return member.GetCustomAttributes(inherit).Select(t => t.GetType()).Any(a => attributeType.IsAssignableFrom(a));
    }

    #endregion
}
