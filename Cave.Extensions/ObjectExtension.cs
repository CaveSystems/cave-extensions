using System;
using System.Diagnostics;
using System.Linq;

namespace Cave
{
    /// <summary>Gets extensions on object instances.</summary>
    public static class ObjectExtension
    {
        /// <summary>Checks whether all properties equal.</summary>
        /// <typeparam name="TObject">The object type to get property definitions from.</typeparam>
        /// <param name="instance">Source instance to read properties from.</param>
        /// <param name="other">Target instance to check properties against.</param>
        /// <param name="ignoredByAttribute">If set all properties containing this attribute will be ignored.</param>
        /// <param name="ignoredByName">If set all properties with a name matching any entry will be ignored.</param>
        /// <returns>True if all properties equal, false otherwise.</returns>
        public static bool PropertiesEqual<TObject>(this TObject instance, TObject other, Type ignoredByAttribute = null, params string[] ignoredByName)
            => PropertiesEqual(instance, other, new[] { ignoredByAttribute }, ignoredByName);

        /// <summary>Checks whether all properties equal.</summary>
        /// <typeparam name="TObject">The object type to get property definitions from.</typeparam>
        /// <param name="instance">Source instance to read properties from.</param>
        /// <param name="other">Target instance to check properties against.</param>
        /// <param name="ignoredByAttribute">If set all properties containing any of the specified attributes will be ignored.</param>
        /// <param name="ignoredByName">If set all properties with a name matching any entry will be ignored.</param>
        /// <returns>True if all properties equal, false otherwise.</returns>
        public static bool PropertiesEqual<TObject>(this TObject instance, TObject other, Type[] ignoredByAttribute = null, string[] ignoredByName = null)
        {
            var type = typeof(TObject);
            var i = 0;
            foreach (var property in type.GetProperties())
            {
                if (!property.CanRead)
                {
                    continue;
                }

                if ((ignoredByName != null) && ignoredByName.Contains(property.Name))
                {
                    continue;
                }

#if NET20 || NET35 || NET40
                if (ignoredByAttribute != null && property.GetCustomAttributes(true).Any(a => ignoredByAttribute.Contains(a.GetType())))
                {
                    continue;
                }
                var instanceValue = property.GetGetMethod().Invoke(instance, null);
                var otherValue = property.GetGetMethod().Invoke(other, null);
#else
                if ((ignoredByAttribute != null) && ignoredByAttribute.Intersect(property.CustomAttributes.Select(a => a.AttributeType)).Any())
                {
                    continue;
                }

                var instanceValue = property.GetMethod.Invoke(instance, null);
                var otherValue = property.GetMethod.Invoke(other, null);
#endif
                if (!Equals(instanceValue, otherValue))
                {
                    Trace.TraceInformation($"{type.Name}.{property.Name} not equal ({instanceValue} != {otherValue})");
                    return false;
                }

                i++;
            }

            if (i == 0)
            {
                throw new ArgumentException($"Type {type} has no readable properties.");
            }

            return true;
        }
    }
}
