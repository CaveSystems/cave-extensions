using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Cave
{
    /// <summary>Provides extensions to object instances.</summary>
    public static class ObjectExtension
    {
        #region Static

        /// <summary>Get a list of available properties.</summary>
        /// <param name="instance">Object instance to read.</param>
        /// <param name="bindingFlags">A bitwise combination of the enumeration values that specify how the search is conducted.</param>
        /// <param name="withValue">List only sub-properties with values.</param>
        /// <param name="noRecursion">Disable recursion.</param>
        /// <returns>Returns an <see cref="IEnumerable{T}" /> with all properties of the specified instance.</returns>
        [Obsolete("Use GetProperties(instance, flags, bindingFlags, filter) instead!")]
        public static IEnumerable<PropertyData> GetProperties(this object instance, BindingFlags bindingFlags, bool withValue, bool noRecursion)
            => GetProperties(instance, bindingFlags: bindingFlags, withValue: withValue, noRecursion: noRecursion);

        /// <summary>Get a list of available properties.</summary>
        /// <param name="instance">Object instance to read.</param>
        /// <param name="bindingFlags">A bitwise combination of the enumeration values that specify how the search is conducted.</param>
        /// <param name="withValue">List only sub-properties with values.</param>
        /// <param name="noRecursion">Disable recursion.</param>
        /// <param name="filter">Allows to filter properties.</param>
        /// <returns>Returns an <see cref="IEnumerable{T}" /> with all properties of the specified instance.</returns>
        [Obsolete("Use GetProperties(instance, flags, bindingFlags, filter) instead!")]
        public static IEnumerable<PropertyData> GetProperties(this object instance, BindingFlags bindingFlags = 0, bool withValue = false, bool noRecursion = false, PropertyDataFilter filter = null)
            => GetProperties(instance, (withValue ? PropertyFlags.FilterUnset : 0) | (noRecursion ? 0 : PropertyFlags.Recursive), bindingFlags, filter);

        /// <summary>Get a list of available properties.</summary>
        /// <param name="instance">Object instance to read.</param>
        /// <param name="flags">Filter flags for property search.</param>
        /// <param name="bindingFlags">A bitwise combination of the enumeration values that specify how the search is conducted.</param>
        /// <param name="filter">Allows to filter properties.</param>
        /// <returns>Returns an <see cref="IEnumerable{T}" /> with all properties of the specified instance.</returns>
        public static IEnumerable<PropertyData> GetProperties(this object instance, PropertyFlags flags, BindingFlags bindingFlags = 0, PropertyDataFilter filter = null)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (bindingFlags == 0)
            {
                bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            }

            var recursive = flags.HasFlag(PropertyFlags.Recursive);
            return flags.HasFlag(PropertyFlags.FilterUnset)
                ? new PropertyValueEnumerator(instance, bindingFlags, recursive, filter)
                : new PropertyEnumerator(instance.GetType(), instance, bindingFlags, recursive, filter);
        }

        /// <summary>Gets the specified property value.</summary>
        /// <remarks>
        /// See available full path items using <see cref="PropertyEnumerator" /> and <see cref="PropertyValueEnumerator" /> or use
        /// <see cref="GetProperties(object, BindingFlags, bool, bool)" />.
        /// </remarks>
        /// <param name="instance">Instance to read from.</param>
        /// <param name="fullPath">Full property path.</param>
        /// <param name="noException">Ignore null value properties and missing fields.</param>
        /// <returns>Returns the value of the specified property or default.</returns>
        [Obsolete("Use TryGetPropertyValue instead!")]
        public static object GetPropertyValue(this object instance, string fullPath, bool noException)
        {
            if (noException)
            {
                TryGetPropertyValue(instance, fullPath, out var value);
                return value;
            }

            return GetPropertyValue(instance, fullPath);
        }

        /// <summary>Gets the specified property value and checks the return value type.</summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="instance">Instance to read from.</param>
        /// <param name="fullPath">Full property path.</param>
        /// <param name="noException">Ignore null value properties and missing fields.</param>
        /// <returns>Returns the value of the specified property or default.</returns>
        [Obsolete("Use TryGetPropertyValue instead!")]
        public static TValue GetPropertyValue<TValue>(this object instance, string fullPath, bool noException)
        {
            if (noException)
            {
                TryGetPropertyValue<TValue>(instance, fullPath, out var value);
                return value;
            }

            return GetPropertyValue<TValue>(instance, fullPath);
        }

        /// <summary>Gets the specified property value.</summary>
        /// <remarks>
        /// See available full path items using <see cref="PropertyEnumerator" /> and <see cref="PropertyValueEnumerator" /> or use
        /// <see cref="GetProperties(object, BindingFlags, bool, bool)" />.
        /// </remarks>
        /// <param name="instance">Instance to read from.</param>
        /// <param name="fullPath">Full property path.</param>
        /// <param name="bindingFlags">BindingFlags for the property. (Default = Public | Instance).</param>
        /// <returns>Returns the value of the specified property or default.</returns>
        public static object GetPropertyValue(this object instance, string fullPath, BindingFlags bindingFlags = 0)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            var result = TryGetPropertyValue(instance, fullPath, out var value, bindingFlags, true);
            return (result == GetPropertyValueError.None) ? value : throw new($"Unhandled error {result}!");
        }

        /// <summary>Gets the specified property value and checks the return value type.</summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="instance">Instance to read from.</param>
        /// <param name="fullPath">Full property path.</param>
        /// <returns>Returns the value of the specified property or default.</returns>
        public static TValue GetPropertyValue<TValue>(this object instance, string fullPath)
        {
            var current = GetPropertyValue(instance, fullPath);
            if (current is TValue value)
            {
                return value;
            }

            if (current is null)
            {
                return default;
            }

            var targetType = typeof(TValue);
            if (targetType.IsInstanceOfType(current) || current is IConvertible)
            {
                return (TValue)Convert.ChangeType(current, targetType);
            }

            throw new ArgumentOutOfRangeException(nameof(fullPath), $"Property path {fullPath} is not of type {typeof(TValue)}!");
        }

        /// <summary>Checks whether all properties equal.</summary>
        /// <typeparam name="TObject">The object type to get property definitions from.</typeparam>
        /// <param name="instance">Source instance to read properties from.</param>
        /// <param name="other">Target instance to check properties against.</param>
        /// <param name="ignoredByAttribute">If set all properties containing this attribute will be ignored.</param>
        /// <param name="ignoredByName">If set all properties with a name matching any entry will be ignored.</param>
        /// <returns>True if all properties equal, false otherwise.</returns>
        public static bool PropertiesEqual<TObject>(this TObject instance, TObject other, Type ignoredByAttribute = null, params string[] ignoredByName) => PropertiesEqual(instance, other, new[] { ignoredByAttribute }, ignoredByName);

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
                if ((ignoredByAttribute != null) && property.GetCustomAttributes(true).Any(a => ignoredByAttribute.Contains(a.GetType())))
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

        /// <summary>Gets the specified property value.</summary>
        /// <remarks>
        /// See available full path items using <see cref="PropertyEnumerator" /> and <see cref="PropertyValueEnumerator" /> or use
        /// <see cref="GetProperties(object, BindingFlags, bool, bool)" />.
        /// </remarks>
        /// <param name="instance">Instance to read from.</param>
        /// <param name="fullPath">Full property path.</param>
        /// <param name="result">Returns the result value.</param>
        /// <param name="bindingFlags">BindingFlags for the property. (Default = Public | Instance).</param>
        /// <returns>Returns <see cref="GetPropertyValueError.None" /> on success or the error encountered.</returns>
        public static GetPropertyValueError TryGetPropertyValue(this object instance, string fullPath, out object result, BindingFlags bindingFlags = 0)
            => TryGetPropertyValue(instance, fullPath, out result, bindingFlags, false);

        /// <summary>Gets the specified property value.</summary>
        /// <remarks>
        /// See available full path items using <see cref="PropertyEnumerator" /> and <see cref="PropertyValueEnumerator" /> or use
        /// <see cref="GetProperties(object, BindingFlags, bool, bool)" />.
        /// </remarks>
        /// <param name="instance">Instance to read from.</param>
        /// <param name="fullPath">Full property path.</param>
        /// <param name="result">Returns the result value.</param>
        /// <param name="bindingFlags">BindingFlags for the property. (Default = Public | Instance).</param>
        /// <param name="throwException">Throw exceptions instead of returning an error code.</param>
        /// <returns>Returns <see cref="GetPropertyValueError.None" /> on success or the error encountered.</returns>
        static GetPropertyValueError TryGetPropertyValue(this object instance, string fullPath, out object result, BindingFlags bindingFlags, bool throwException)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (bindingFlags == 0)
            {
                bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            }

            IList<string> path = fullPath?.Split(new[] { '.', '/' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            var current = instance;
            for (var i = 0; i < path.Count; i++)
            {
                var part = path[i];
                if (current == null)
                {
                    if (throwException)
                    {
                        throw new NullReferenceException($"Property path {path.Take(i).Join("/")} is null!");
                    }
                    result = null;
                    return GetPropertyValueError.NullReference;
                }

                var property = current.GetType().GetProperty(part.BeforeFirst('['), bindingFlags);
                if (property == null)
                {
                    if (throwException)
                    {
                        throw new ArgumentOutOfRangeException(nameof(fullPath), $"Property path {path.Take(i + 1).Join("/")} could not be found at specified instance!");
                    }
                    result = null;
                    return GetPropertyValueError.InvalidPath;
                }

                try
                {
                    current = property.GetValue(current, null);
                }
                catch
                {
                    if (throwException)
                    {
                        throw new ArgumentOutOfRangeException(nameof(fullPath), $"Property path {path.Take(i + 1).Join("/")} could not be found at specified instance!");
                    }
                    result = null;
                    return GetPropertyValueError.InvalidPath;
                }

                var indexer = part.AfterFirst('[');
                try
                {
                    if (indexer.Length > 0)
                    {
                        var index = int.Parse(indexer.BeforeFirst(']'));
                        if (current is IEnumerable enumerable)
                        {
                            current = enumerable.Cast<object>().Skip(index).FirstOrDefault();
                        }
                        else if (current is null)
                        {
                            if (throwException)
                            {
                                throw new NullReferenceException($"Property path {path.Take(i).Join("/")} is null!");
                            }
                            result = null;
                            return GetPropertyValueError.NullReference;
                        }
                        else
                        {
                            if (throwException)
                            {
                                throw new NullReferenceException($"Property path {path.Take(i).Join("/")} is of invalid type {current.GetType()}!");
                            }
                            result = null;
                            return GetPropertyValueError.InvalidType;
                        }
                    }
                }
                catch (TargetParameterCountException)
                {
                    if (throwException)
                    {
                        throw new ArgumentOutOfRangeException(nameof(fullPath), $"Property {fullPath} requires multiple target parameters!");
                    }
                    result = null;
                    return GetPropertyValueError.ParameterRequired;
                }
                catch
                {
                    if (throwException)
                    {
                        throw new ArgumentOutOfRangeException(nameof(fullPath), $"Property index [{indexer}] of {fullPath} is out of valid range!");
                    }
                    result = null;
                    return GetPropertyValueError.InvalidPath;
                }
            }

            result = current;
            return GetPropertyValueError.None;
        }

        /// <summary>Gets the specified property value.</summary>
        /// <remarks>
        /// See available full path items using <see cref="PropertyEnumerator" /> and <see cref="PropertyValueEnumerator" /> or use
        /// <see cref="GetProperties(object, BindingFlags, bool, bool)" />.
        /// </remarks>
        /// <param name="instance">Instance to read from.</param>
        /// <param name="fullPath">Full property path.</param>
        /// <param name="result">Returns the result value.</param>
        /// <param name="bindingFlags">BindingFlags for the property. (Default = Public | Instance).</param>
        /// <returns>Returns <see cref="GetPropertyValueError.None" /> on success or the error encountered.</returns>
        public static GetPropertyValueError TryGetPropertyValue<TValue>(this object instance, string fullPath, out TValue result, BindingFlags bindingFlags = default)
        {
            var error = TryGetPropertyValue(instance, fullPath, out var obj, bindingFlags);
            if (error != GetPropertyValueError.None)
            {
                // error during get
                result = default;
                return error;
            }

            if (obj is TValue v)
            {
                // no error, value type matches
                result = v;
                return GetPropertyValueError.None;
            }

            {
                // (error = none) but type does not match
                result = default;
                return GetPropertyValueError.InvalidType;
            }
        }

        #endregion
    }
}
