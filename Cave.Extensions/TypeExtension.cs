using System;
using System.Linq;
using System.Reflection;

namespace Cave
{
    /// <summary>
    /// Provides extensions for the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtension
    {
#if NETSTANDARD13
        /// <summary>
        /// Returns custom attributes applied to this member.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="inherit">true to search this member's inheritance chain to find the attributes; otherwise, false. This parameter is ignored for properties and events.</param>
        /// <returns>Returns an array that contains all the custom attributes applied to this member, or an array with zero elements if no attributes are defined.</returns>
        public static Attribute[] GetCustomAttributes(this Type type, bool inherit = false) => type.GetTypeInfo().GetCustomAttributes(inherit).ToArray();
#endif

        /// <summary>
        /// Checks a type for presence of a specific <see cref="Attribute"/> instance.
        /// </summary>
        /// <typeparam name="T">The attribute type to check for.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <param name="inherit">Inherit attributes from parents.</param>
        /// <returns>Returns true if at least one attribute of the desired type could be found, false otherwise.</returns>
        public static bool HasAttribute<T>(this Type type, bool inherit = false)
            where T : Attribute
            => HasAttribute(type, typeof(T), inherit);

        /// <summary>
        /// Checks a type for presence of a specific <see cref="Attribute"/> instance.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="attributeType">The attribute type to check for.</param>
        /// <param name="inherit">Inherit attributes from parents.</param>
        /// <returns>Returns true if at least one attribute of the desired type could be found, false otherwise.</returns>
        public static bool HasAttribute(this Type type, Type attributeType, bool inherit = false) => type.GetCustomAttributes(inherit).Select(t => t.GetType()).Any(a => attributeType.IsAssignableFrom(a));

        /// <summary>
        /// Gets a specific <see cref="Attribute"/> present at the type. If the attribute type cannot be found null is returned.
        /// </summary>
        /// <typeparam name="T">The attribute type to check for.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <param name="inherit">Inherit attributes from parents.</param>
        /// <returns>Returns the attribute found or null.</returns>
        public static T GetAttribute<T>(this Type type, bool inherit = false)
            where T : Attribute
            => (T)GetAttribute(type, typeof(T), inherit);

        /// <summary>
        /// Gets a specific <see cref="Attribute"/> present at the type. If the attribute type cannot be found null is returned.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="attributeType">The attribute type to check for.</param>
        /// <param name="inherit">Inherit attributes from parents.</param>
        /// <returns>Returns the attribute found or null.</returns>
        public static object GetAttribute(this Type type, Type attributeType, bool inherit = false) => type.GetCustomAttributes(inherit).Where(a => attributeType.IsAssignableFrom(a.GetType())).FirstOrDefault();
    }
}
