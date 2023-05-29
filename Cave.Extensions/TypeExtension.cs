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
#if NET45_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP1_0_OR_GREATER
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

#if NETCOREAPP1_0 || NETCOREAPP1_1 || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
        var typeInfo = toType.GetTypeInfo();
        if (typeInfo.IsPrimitive)
        {
            return ConvertPrimitive(toType, value, formatProvider);
        }
        if (typeInfo.IsEnum)
        {
            return Enum.Parse(toType, value.ToString(), true);
        }
#else
        if (toType.IsPrimitive)
        {
            return ConvertPrimitive(toType, value, formatProvider);
        }
        if (toType.IsEnum)
        {
            return Enum.Parse(toType, value.ToString(), true);
        }
#endif

        if (toType.IsAssignableFrom(value.GetType()))
        {
            return ConvertPrimitive(toType, value, formatProvider);
        }

        // convert to string
        string str;
        {
            if (value is string s)
            {
                str = s;
            }
            else if (value is IFormattable formattable)
            {
                Trace.TraceInformation("Using IFormattable to convert to string!");
                str = formattable.ToString(null, formatProvider);
            }
            else if (value is IConvertible convertible)
            {
                Trace.TraceInformation("Using IConvertible to convert to string!");
                str = convertible.ToString(formatProvider);
            }
            else
            {
                Trace.TraceInformation("Using object.ToString() convert to string!");
                str = value.ToString();
            }
        }
        if (toType == typeof(string))
        {
            return str;
        }

        if (toType.IsArray)
        {
            var elementType = toType.GetElementType();
#if NETCOREAPP1_0 || NETCOREAPP1_1 || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
            if (elementType?.GetTypeInfo().IsPrimitive != true)
            {
                throw new NotSupportedException($"Not primitive array type {toType} not supported!");
            }
#else
            if (elementType?.IsPrimitive != true)
            {
                throw new NotSupportedException($"Not primitive array type {toType} not supported!");
            }
#endif

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
                    return new TimeSpan((long)Math.Round(double.Parse(str.SubstringEnd(-2), formatProvider) * (TimeSpan.TicksPerMillisecond / 1000d / 1000d)));
                }

                if (str.EndsWith("µs", StringComparison.Ordinal))
                {
                    return new TimeSpan((long)Math.Round(double.Parse(str.SubstringEnd(-2), formatProvider) * (TimeSpan.TicksPerMillisecond / 1000d)));
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
            var method = toType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), typeof(IFormatProvider) }, null);
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
            var cctor = toType.GetConstructor(new[] { typeof(string), typeof(IFormatProvider) });
            if (cctor != null)
            {
                try
                {
                    return cctor.Invoke(new object[] { str, formatProvider });
                }
                catch (TargetInvocationException ex)
                {
                    errors.Add(ex.InnerException);
                }
            }
            cctor = toType.GetConstructor(new[] { typeof(string) });
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
            method = toType.GetMethod("op_Explicit", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
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
            if (errors.Count > 0)
            {
                throw new AggregateException(errors.ToArray());
            }

            throw new MissingMethodException($"Type {toType} has no public static Parse(string, IFormatProvider), Parse(string), cctor(string, IFormatProvider), cctor(string) or operator conversion method!");
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

#if NETCOREAPP1_0 || NETCOREAPP1_1
    /// <summary>
    /// Backport for netstandard 1 and netcore 1: TODO obey inherit!
    /// </summary>
    /// <param name="type"></param>
    /// <param name="inherit"></param>
    /// <returns></returns>
    public static object[] GetCustomAttributes(this Type type, bool inherit)
        => type.GetTypeInfo().CustomAttributes.Select(c => c.Constructor.Invoke(c.ConstructorArguments.Select(a => a.Value).ToArray())).ToArray();
#endif

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
    public static string GetCompanyName(this Type type)
#if NETCOREAPP1_0 || NETCOREAPP1_1 || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
        => type?.GetTypeInfo().Assembly.GetCompanyName();
#else
        => type?.Assembly.GetCompanyName();
#endif

#if NETCOREAPP1_0 || NETCOREAPP1_1 || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
    /// <summary>Backport</summary>
    public static MethodInfo GetMethod(this Type type, string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
#if (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD1_6_OR_GREATER)
        //todo obey parametermodifiers
        => type.GetTypeInfo().DeclaredMethods.SingleOrDefault(m => m.Name == name && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(types));
#else
        => type.GetTypeInfo().GetMethod(name, types, modifiers);
#endif
#endif

    /// <summary>Get the assembly product name using the <see cref="AssemblyProductAttribute" />.</summary>
    /// <param name="type">Type to search for the product attribute.</param>
    /// <returns>The product name.</returns>
    public static string GetProductName(this Type type)
#if NETCOREAPP1_0 || NETCOREAPP1_1 || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
        => type?.GetTypeInfo().Assembly.GetProductName();
#else
        => type?.Assembly.GetProductName();
#endif

#if NETCOREAPP1_0 || NETCOREAPP1_1 || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
    /// <summary>Backport</summary>
    public static PropertyInfo[] GetProperties(this Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        => type.GetTypeInfo().GetProperties(bindingFlags);
#endif

#if (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD1_6_OR_GREATER)
    /// <summary>Backport</summary>
    //TODO obey binding flags
    public static PropertyInfo[] GetProperties(this TypeInfo typeInfo, BindingFlags bindingFlags) => typeInfo.DeclaredProperties.ToArray();
#endif

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

#if (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
#if !NETSTANDARD1_6_OR_GREATER
    /// <summary>Backport</summary>
    public static bool IsInstanceOfType(this TypeInfo typeInfo, object instance)
        => typeInfo.IsAssignableFrom(instance.GetType().GetTypeInfo());
#endif
    /// <summary>Backport</summary>
    public static IEnumerable<Attribute> GetCustomAttributes(this Type type, bool inherit = false)
        => type.GetTypeInfo().GetCustomAttributes(inherit);

    /// <summary>Backport</summary>
    public static bool IsInstanceOfType(this Type type, object instance)
        => type.GetTypeInfo().IsInstanceOfType(instance);

    /// <summary>Backport</summary>
    public static bool IsAssignableFrom(this Type type, Type typeToTest)
#if NETSTANDARD1_6_OR_GREATER
        => type.GetTypeInfo().IsAssignableFrom(typeToTest);
#else
        => type.GetTypeInfo().IsAssignableFrom(typeToTest.GetTypeInfo());
#endif

    /// <summary>Backport</summary>
    public static PropertyInfo GetProperty(this Type type, string name)
#if NETSTANDARD1_6_OR_GREATER
        => type.GetTypeInfo().GetProperty(name);
#else
        => type.GetTypeInfo().DeclaredProperties.SingleOrDefault(p => p.Name == name);
#endif

    /// <summary>Backport</summary>
    public static PropertyInfo GetProperty(this Type type, string name, BindingFlags bindingFlags)
#if NETSTANDARD1_6_OR_GREATER
        => type.GetTypeInfo().GetProperty(name, bindingFlags);
#else
        => type.GetTypeInfo().DeclaredProperties.SingleOrDefault(p => p.Name == name);
#endif

    /// <summary>Backport</summary>
    public static IEnumerable<FieldInfo> GetFields(this Type type)
#if NETSTANDARD1_6_OR_GREATER
        => type.GetTypeInfo().GetFields();
#else
        => type.GetTypeInfo().DeclaredFields.ToArray();
#endif

    /// <summary>Backport</summary>
    public static IEnumerable<FieldInfo> GetFields(this Type type, BindingFlags bindingFlags)
#if NETSTANDARD1_6_OR_GREATER
        => type.GetTypeInfo().GetFields(bindingFlags);
#else
        => type.GetTypeInfo().DeclaredFields.ToArray();
#endif

    /// <summary>Backport</summary>
    public static ConstructorInfo GetConstructor(this Type type, Type[] types)
#if NETSTANDARD1_6_OR_GREATER
        => type.GetTypeInfo().GetConstructor(types);
#else
        => type.GetTypeInfo().DeclaredConstructors.SingleOrDefault(c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(types));
#endif
#endif

    /// <summary>
    /// Determines whether a type is a user defined structure. This is true for: type.IsValueType &amp;&amp; !type.IsPrimitive &amp;&amp;
    /// !type.IsEnum.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>Returns true if the type is a user defined structure, false otherwise.</returns>
    public static bool IsStruct(this Type type)
#if NETCOREAPP1_0 || NETCOREAPP1_1 || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
        => (type?.GetTypeInfo().IsValueType == true) && !type.GetTypeInfo().IsPrimitive && !type.GetTypeInfo().IsEnum;
#else
        => (type?.IsValueType == true) && !type.IsPrimitive && !type.IsEnum;
#endif
    #endregion
}
