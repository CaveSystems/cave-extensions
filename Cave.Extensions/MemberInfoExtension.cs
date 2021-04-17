using System;
using System.Linq;
using System.Reflection;

namespace Cave
{
    /// <summary>Gets extensions for the <see cref="MemberInfo" /> class.</summary>
    public static class MemberInfoExtension
    {
#if NETSTANDARD13
        /// <summary>
        /// Determines whether an instance of a specified type can be assigned to an instance of the current type.
        /// </summary>
        /// <param name="assignTo">The current type to compare.</param>
        /// <param name="assignFrom">The type to compare with the current type.</param>
        /// <returns>Returns true if the <paramref name="assignFrom"/> can be assigned to a variable of type <paramref name="assignTo"/>.</returns>
        public static bool IsAssignableFrom(this Type assignTo, Type assignFrom)
        {
            return assignTo.GetTypeInfo().IsAssignableFrom(assignFrom.GetTypeInfo());
        }
#endif

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
    }
}
