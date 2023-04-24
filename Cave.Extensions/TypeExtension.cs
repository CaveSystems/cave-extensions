using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cave;

/// <summary>Gets extensions for the <see cref="Type" /> class.</summary>
public static class TypeExtension
{
    #region Static

    /// <summary>Converts a (primitive) value to the desired type.</summary>
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
            throw new NotSupportedException($"The value '{value}' cannot be converted to target type '{toType}'!", ex);
        }
    }

    /// <summary>Converts a value to the desired field value.</summary>
    /// <param name="toType">Type to convert to.</param>
    /// <param name="value">Value to convert.</param>
    /// <param name="culture">The culture to use during formatting.</param>
    /// <returns>Returns a new instance of the specified type.</returns>
    public static object ConvertValue(this Type toType, object value, CultureInfo culture) => ConvertValue(toType, value, (IFormatProvider)culture);

    /// <summary>Converts a value to the desired field value.</summary>
    /// <param name="toType">Type to convert to.</param>
    /// <param name="value">Value to convert.</param>
    /// <param name="formatProvider">The format provider to use during formatting.</param>
    /// <returns>Returns a new instance of the specified type.</returns>
    public static object ConvertValue(this Type toType, object value, IFormatProvider formatProvider = null)
    {
        if (toType == null)
        {
            throw new ArgumentNullException(nameof(toType));
        }

        formatProvider ??= CultureInfo.InvariantCulture;

        if (value == null)
        {
            return null;
        }

        if (Nullable.GetUnderlyingType(toType) != null)
        {
            if (Equals(value, "<null>"))
            {
                return null;
            }
#if NET45_OR_GREATER || NETSTANDARD1_3_OR_GREATER || NET5_0_OR_GREATER
            toType = toType.GenericTypeArguments[0];
#elif NET20_OR_GREATER
            toType = toType.GetGenericArguments()[0];
#else
#error No code defined for the current framework or NETXX version define missing!
#endif
        }

        if (toType == typeof(bool))
        {
            switch (value.ToString().ToUpperInvariant())
            {
                case "TRUE":
                case "ON":
                case "YES":
                case "1":
                    return true;
                case "FALSE":
                case "OFF":
                case "NO":
                case "0":
                    return false;
            }
        }
        if (toType.IsPrimitive)
        {
            return ConvertPrimitive(toType, value, formatProvider);
        }

        if (toType.IsAssignableFrom(value.GetType()))
        {
            return ConvertPrimitive(toType, value, formatProvider);
        }

        if (toType.IsEnum)
        {
            return Enum.Parse(toType, value.ToString(), true);
        }

        // convert to string
        string str;
        {
            if (value is string s)
            {
                str = s;
            }
            else
            {
                Trace.TraceWarning("Try to find public ToString(IFormatProvider) method in class");
                var method = value.GetType().GetMethod("ToString",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    new[] { typeof(IFormatProvider) },
                    null);
                if (method != null)
                {
                    try
                    {
                        str = (string)method.Invoke(value, new object[] { formatProvider });
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

        if (toType.IsArray)
        {
            var elementType = toType.GetElementType();
            if (elementType?.IsPrimitive != true)
            {
                throw new NotSupportedException($"Not primitive array type {toType} not supported!");
            }

            var parts = str.AfterFirst('{').BeforeLast('}').Split(',');
            var array = Array.CreateInstance(elementType, parts.Length);
            for (var i = 0; i < parts.Length; i++)
            {
                array.SetValue(ConvertValue(elementType, parts[i], formatProvider), i);
            }

            return array;
        }

        if (toType == typeof(DateTime))
        {
            if (long.TryParse(str, out var ticks))
            {
                return new DateTime(ticks, DateTimeKind.Unspecified);
            }

            return str.ParseDateTime(formatProvider);
        }

        if (toType == typeof(TimeSpan))
        {
            try
            {
                if (str.Contains(':'))
                {
#if NET20 || NET35
                    return TimeSpan.Parse(str);
#else
                    return TimeSpan.Parse(str, formatProvider);
#endif
                }

                if (str.EndsWith("ns", StringComparison.Ordinal))
                {
                    return new TimeSpan((long)Math.Round(double.Parse(str.SubstringEnd(-2), formatProvider) * (TimeSpan.TicksPerMillisecond / 1000)));
                }

                if (str.EndsWith("ms", StringComparison.Ordinal))
                {
                    return new TimeSpan((long)Math.Round(double.Parse(str.SubstringEnd(-2), formatProvider) * TimeSpan.TicksPerMillisecond));
                }

                if (str.EndsWith("s", StringComparison.Ordinal))
                {
                    return new TimeSpan((long)Math.Round(double.Parse(str.SubstringEnd(-1), formatProvider) * TimeSpan.TicksPerSecond));
                }

                if (str.EndsWith("min", StringComparison.Ordinal))
                {
                    return new TimeSpan((long)Math.Round(double.Parse(str.SubstringEnd(-3), formatProvider) * TimeSpan.TicksPerMinute));
                }

                if (str.EndsWith("h", StringComparison.Ordinal))
                {
                    return new TimeSpan((long)Math.Round(double.Parse(str.SubstringEnd(-1), formatProvider) * TimeSpan.TicksPerHour));
                }

                if (str.EndsWith("d", StringComparison.Ordinal))
                {
                    return new TimeSpan((long)Math.Round(double.Parse(str.SubstringEnd(-1), formatProvider) * TimeSpan.TicksPerDay));
                }

                return str.EndsWith("a", StringComparison.Ordinal)
                    ? TimeSpan.FromDays(double.Parse(str.SubstringEnd(-1), formatProvider) * 365.25)
                    : (object)new TimeSpan(long.Parse(str, formatProvider));
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"Value '{str}' is not a valid TimeSpan!", ex);
            }
        }

        // parse from string
        {
            // try to find public static Parse(string, IFormatProvider) method in class
            var errors = new List<Exception>();
            var method = toType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[]
                {
                    typeof(string),
                    typeof(IFormatProvider)
                },
                null);
            if (method != null)
            {
                try
                {
                    return method.Invoke(null, new object[]
                    {
                        str,
                        formatProvider
                    });
                }
                catch (TargetInvocationException ex)
                {
                    errors.Add(ex.InnerException);
                }
            }
            method = toType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
            if (method != null)
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
            method = toType.GetMethod("op_Implicit", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
            if (method != null)
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
            var cctor = toType.GetConstructor(new[] { typeof(string) });
            if (cctor != null)
            {
                try
                {
                    return cctor.Invoke(new object[] { str });
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

            throw new MissingMethodException($"Type {toType} has no public static Parse(string, IFormatProvider), Parse(string) or cctor(string) method!");
        }
    }

    /// <summary>Gets a specific <see cref="Attribute" /> present at the type. If the attribute type cannot be found null is returned.</summary>
    /// <typeparam name="T">The attribute type to check for.</typeparam>
    /// <param name="type">The type to check.</param>
    /// <param name="inherit">Inherit attributes from parents.</param>
    /// <returns>Returns the attribute found or null.</returns>
    public static T GetAttribute<T>(this Type type, bool inherit = false)
        where T : Attribute =>
        (T)GetAttribute(type, typeof(T), inherit);

    /// <summary>Gets a specific <see cref="Attribute" /> present at the type. If the attribute type cannot be found null is returned.</summary>
    /// <param name="type">The type to check.</param>
    /// <param name="attributeType">The attribute type to check for.</param>
    /// <param name="inherit">Inherit attributes from parents.</param>
    /// <returns>Returns the attribute found or null.</returns>
    public static object GetAttribute(this Type type, Type attributeType, bool inherit = false) =>
        type?.GetCustomAttributes(inherit).Where(a => attributeType.IsAssignableFrom(a.GetType())).FirstOrDefault()
     ?? throw new ArgumentNullException(nameof(type));

    /// <summary>
    /// Gets all attributes of a specific <see cref="Attribute" /> type present at the type. If the attribute type cannot be found an
    /// empty list is returned.
    /// </summary>
    /// <typeparam name="T">The attribute type to check for.</typeparam>
    /// <param name="type">The type to check.</param>
    /// <param name="inherit">Inherit attributes from parents.</param>
    /// <returns>Returns the attributes found.</returns>
    public static IEnumerable<T> GetAttributes<T>(this Type type, bool inherit = false)
        where T : Attribute =>
        GetAttributes(type, typeof(T), inherit).Cast<T>();

    /// <summary>
    /// Gets all attributes of a specific <see cref="Attribute" /> type present at the type. If the attribute type cannot be found an
    /// empty list is returned.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="attributeType">The attribute type to check for.</param>
    /// <param name="inherit">Inherit attributes from parents.</param>
    /// <returns>Returns the attributes found.</returns>
    public static IEnumerable GetAttributes(this Type type, Type attributeType, bool inherit = false) =>
        type?.GetCustomAttributes(inherit).Where(a => attributeType.IsAssignableFrom(a.GetType()))
     ?? throw new ArgumentNullException(nameof(type));

    /// <summary>Get the assembly company name using the <see cref="AssemblyCompanyAttribute" />.</summary>
    /// <param name="type">Type to search for the product attribute.</param>
    /// <returns>The company name.</returns>
    public static string GetCompanyName(this Type type) => type?.Assembly.GetCompanyName();

    /// <summary>Get the assembly product name using the <see cref="AssemblyProductAttribute" />.</summary>
    /// <param name="type">Type to search for the product attribute.</param>
    /// <returns>The product name.</returns>
    public static string GetProductName(this Type type) => type?.Assembly.GetProductName();

    /// <summary>Checks a type for presence of a specific <see cref="Attribute" /> instance.</summary>
    /// <typeparam name="T">The attribute type to check for.</typeparam>
    /// <param name="type">The type to check.</param>
    /// <param name="inherit">Inherit attributes from parents.</param>
    /// <returns>Returns true if at least one attribute of the desired type could be found, false otherwise.</returns>
    public static bool HasAttribute<T>(this Type type, bool inherit = false)
        where T : Attribute =>
        HasAttribute(type, typeof(T), inherit);

    /// <summary>Checks a type for presence of a specific <see cref="Attribute" /> instance.</summary>
    /// <param name="type">The type to check.</param>
    /// <param name="attributeType">The attribute type to check for.</param>
    /// <param name="inherit">Inherit attributes from parents.</param>
    /// <returns>Returns true if at least one attribute of the desired type could be found, false otherwise.</returns>
    public static bool HasAttribute(this Type type, Type attributeType, bool inherit = false) =>
        type?.GetCustomAttributes(inherit).Select(t => t.GetType()).Any(a => attributeType.IsAssignableFrom(a))
     ?? throw new ArgumentNullException(nameof(type));

    /// <summary>
    /// Determines whether a type is a user defined structure. This is true for: type.IsValueType &amp;&amp; !type.IsPrimitive &amp;&amp;
    /// !type.IsEnum.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>Returns true if the type is a user defined structure, false otherwise.</returns>
    public static bool IsStruct(this Type type) => (type?.IsValueType == true) && !type.IsPrimitive && !type.IsEnum;

    #endregion
}
