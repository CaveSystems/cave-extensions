using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        /// <summary>
        /// Returns all the public properties of the specified Type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An array of PropertyInfo objects representing all public properties of the current Type.</returns>
        public static PropertyInfo[] GetProperties(this Type type) => type.GetTypeInfo().DeclaredProperties.ToArray();
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

        /// <summary>
        /// Get the assembly product name using the <see cref="AssemblyProductAttribute"/>.
        /// </summary>
        /// <param name="type">Type to search for the product attribute.</param>
        /// <returns>The product name.</returns>
        public static string GetProductName(this Type type)
        {
#if NETSTANDARD13
            return type.GetTypeInfo().Assembly.GetProductName();
#else
            return type.Assembly.GetProductName();
#endif
        }

        /// <summary>
        /// Get the assembly company name using the <see cref="AssemblyCompanyAttribute"/>.
        /// </summary>
        /// <param name="type">Type to search for the product attribute.</param>
        /// <returns>The company name.</returns>
        public static string GetCompanyName(this Type type)
        {
#if NETSTANDARD13
            return type.GetTypeInfo().Assembly.GetCompanyName();
#else
            return type.Assembly.GetCompanyName();
#endif
        }

        /// <summary>
        /// Determines whether a type is a user defined structure.
        /// This is true for: type.IsValueType &amp;&amp; !type.IsPrimitive &amp;&amp; !type.IsEnum.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Returns true if the type is a user defined structure, false otherwise.</returns>
#if NETSTANDARD13
        public static bool IsStruct(this Type type) => type.GetTypeInfo().IsValueType && !type.GetTypeInfo().IsPrimitive && !type.GetTypeInfo().IsEnum;
#else
        public static bool IsStruct(this Type type) => type.IsValueType && !type.IsPrimitive && !type.IsEnum;
#endif

        /// <summary>
        /// Converts a (primitive) value to the desired type.
        /// </summary>
        /// <param name="toType">Type to convert to.</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="cultureInfo">The culture to use during formatting.</param>
        /// <returns>Returns a new instance of the specified type.</returns>
        public static object ConvertPrimitive(this Type toType, object value, IFormatProvider cultureInfo)
        {
            try
            {
                return Convert.ChangeType(value, toType, cultureInfo);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(string.Format("The value '{0}' cannot be converted to target type '{1}'!", value, toType), ex);
            }
        }

        /// <summary>
        /// Converts a value to the desired field value.
        /// </summary>
        /// <param name="toType">Type to convert to.</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="cultureInfo">The culture to use during formatting.</param>
        /// <returns>Returns a new instance of the specified type.</returns>
        public static object ConvertValue(this Type toType, object value, CultureInfo cultureInfo)
        {
            if (toType == null)
            {
                throw new ArgumentNullException(nameof(toType));
            }

            if (cultureInfo == null)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            if (value == null)
            {
                return null;
            }

            // if (toType.Name.StartsWith("Nullable"))
            if (Nullable.GetUnderlyingType(toType) != null)
            {
#if NET45 || NET46 || NET47 || NETSTANDARD13 || NETSTANDARD20
                toType = toType.GenericTypeArguments[0];
#elif NET20 || NET35 || NET40
                toType = toType.GetGenericArguments()[0];
#else
#error No code defined for the current framework or NETXX version define missing!
#endif
            }
            if (toType == typeof(bool))
            {
                switch (value.ToString().ToLower())
                {
                    case "true":
                    case "on":
                    case "yes":
                    case "1":
                        return true;
                    case "false":
                    case "off":
                    case "no":
                    case "0":
                        return false;
                }
            }
#if NETSTANDARD13
            if (toType.GetTypeInfo().IsPrimitive)
#else
            if (toType.IsPrimitive)
#endif
            {
                return ConvertPrimitive(toType, value, cultureInfo);
            }

            if (toType.IsAssignableFrom(value.GetType()))
            {
                return ConvertPrimitive(toType, value, cultureInfo);
            }

#if NETSTANDARD13
            if (toType.GetTypeInfo().IsEnum)
#else
            if (toType.IsEnum)
#endif
            {
                return Enum.Parse(toType, value.ToString(), true);
            }

            // convert to string
            string str;
            {
                if (value is string)
                {
                    str = (string)value;
                }
                else
                {
                    // try to find public ToString(IFormatProvider) method in class
#if NETSTANDARD13
                    var methods = value.GetType().GetTypeInfo().GetDeclaredMethods("ToString");
                    var method = methods.FirstOrDefault(m => m.GetParameters().SingleOrDefault()?.ParameterType == typeof(IFormatProvider) && m.ReturnType == typeof(string));
#else
                    var method = value.GetType().GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(IFormatProvider) }, null);
#endif
                    if (method != null)
                    {
                        try
                        {
                            str = (string)method.Invoke(value, new object[] { cultureInfo });
                        }
                        catch (TargetInvocationException ex)
                        {
                            throw ex.InnerException;
                        }
                    }
                    else
                    {
                        str = value.ToString();
                    }
                }
            }
            if (toType == typeof(string))
            {
                return str;
            }

            if (toType == typeof(DateTime))
            {
                if (long.TryParse(str, out var ticks))
                {
                    return new DateTime(ticks, DateTimeKind.Unspecified);
                }

                if (DateTime.TryParse(str, cultureInfo, DateTimeStyles.AssumeLocal, out DateTime dt) ||
                    DateTimeParser.TryParseDateTime(str, out dt))
                {
                    return dt;
                }
            }
            if (toType == typeof(TimeSpan))
            {
                try
                {
                    if (str.Contains(":"))
                    {
                        return TimeSpan.Parse(str);
                    }
                    if (str.EndsWith("ms"))
                    {
                        return new TimeSpan((long)Math.Round(double.Parse(str.SubstringEnd(1)) * TimeSpan.TicksPerMillisecond));
                    }
                    return str.EndsWith("s")
                        ? new TimeSpan((long)Math.Round(double.Parse(str.SubstringEnd(1)) * TimeSpan.TicksPerSecond))
                        : (object)new TimeSpan(long.Parse(str));
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException(string.Format("Value '{0}' is not a valid TimeSpan!", str), ex);
                }
            }

            // parse from string
            {
                // try to find public static Parse(string, IFormatProvider) method in class
                var errors = new List<Exception>();
#if NETSTANDARD13
                var method = toType.GetTypeInfo().GetDeclaredMethod("Parse");
                var methodParams = method.GetParameters();
                if (method.ReturnType == toType && methodParams.Length == 2 && methodParams[0].ParameterType == typeof(string) && methodParams[1].ParameterType == typeof(IFormatProvider))
#else
                var method = toType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(IFormatProvider) }, null);
                if (method != null)
#endif
                {
                    try
                    {
                        return method.Invoke(null, new object[] { str, cultureInfo });
                    }
                    catch (TargetInvocationException ex)
                    {
                        errors.Add(ex.InnerException);
                    }
                }
#if NETSTANDARD13
                method = toType.GetTypeInfo().GetDeclaredMethod("Parse");
                if (method.ReturnType == toType && method.GetParameters().SingleOrDefault()?.ParameterType == typeof(string))
#else
                method = toType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
                if (method != null)
#endif
                {
                    try
                    {
                        return method.Invoke(null, new object[] { str });
                    }
                    catch (TargetInvocationException ex)
                    {
                        errors.Add(ex.InnerException);
                    }
                }
                if (errors.Count > 0)
                {
                    throw new AggregateException(errors.ToArray());
                }

                throw new MissingMethodException(string.Format("Type {0} has no public static Parse(string, IFormatProvider) or Parse(string) method!", toType));
            }
        }
    }
}
