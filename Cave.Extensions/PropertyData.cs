using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Cave;

/// <summary>
/// Provides property information, full path and value access for the <see cref="PropertyEnumerator"/> and <see cref="PropertyValueEnumerator"/> classes.
/// </summary>
[DebuggerDisplay("{FullPath}")]
public class PropertyData
{
    #region Private Fields

    bool? canGet;

    bool? canSet;

    #endregion Private Fields

    #region Private Methods

    static bool SkipNamespace(IList<string> skipNamespaces, string @namespace)
    {
        bool Test(string skip) => (@namespace == skip) || @namespace.StartsWith(skip + '.', StringComparison.Ordinal);
        return skipNamespaces.Any(Test);
    }

    #endregion Private Methods

    #region Public Constructors

    /// <summary>Creates a new instance of the <see cref="PropertyData"/> class.</summary>
    /// <param name="parent">Parent property data. This may be set to null for the root item.</param>
    /// <param name="propertyInfo">The property info. This may not be null.</param>
    /// <param name="source">The source object of the property.</param>
    /// <param name="arrayIndex">Item is part of an array.</param>
    public PropertyData(PropertyData parent, PropertyInfo propertyInfo, object source, int arrayIndex = -1)
    {
        RootPath = parent?.FullPath ?? string.Empty;
        if (arrayIndex > -1) RootPath += $"[{arrayIndex}]";
        Parent = parent;
        PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
        Source = source;
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the default namespaces property enumeration should not recurse into.</summary>
    public static string[] DefaultSkipNamespaces => new[] { "System", "Microsoft" };

    /// <summary>Gets the default types property enumeration should not recurse into.</summary>
    public static Type[] DefaultSkipTypes => Type.EmptyTypes;

    /// <summary>Gets a value indicating whether the <see cref="Value"/> property can be used or not.</summary>
    public bool CanGetValue => canGet ??= (Source != null) && PropertyInfo.CanRead && (PropertyInfo.GetGetMethod()?.GetParameters().Length == 0);

    /// <summary>Gets a value indicating whether the <see cref="Value"/> property can be used or not.</summary>
    public bool CanSetValue => canSet ??= (Source != null) && PropertyInfo.CanWrite && (PropertyInfo.GetSetMethod()?.GetParameters().Length == 1);

    /// <summary>Gets the full path of the property.</summary>
    public string FullPath => $"{RootPath}/{PropertyInfo.Name}";

    /// <summary>Gets the parent property. This is null at the root property.</summary>
    public PropertyData Parent { get; }

    /// <summary>Gets the property information.</summary>
    public PropertyInfo PropertyInfo { get; }

    /// <summary>Gets the root path of the property.</summary>
    public string RootPath { get; }

    /// <summary>
    /// Gets the source object of this property. This is null at <see cref="PropertyEnumerator"/> and may be null at <see cref="PropertyValueEnumerator"/> if
    /// the property value or root property value is null.
    /// </summary>
    public object Source { get; }

    /// <summary>Gets the current value of the property. This will result in exceptions if <see cref="CanGetValue"/> == false.</summary>
    public object Value => PropertyInfo.GetValue(Source, null);

    #endregion Public Properties

    #region Public Methods

    /// <summary>Checks a <see cref="PropertyData"/> instance for a nested property. Example of a nested Property: Assembly.ManifestModule.Assembly.</summary>
    /// <param name="start">PropertyData to check for parents.</param>
    /// <param name="property">PropertyInfo to check.</param>
    /// <param name="skipNamespaces">Namespaces to be skipped during recursion.</param>
    /// <param name="skipTypes">Types to be skipped during recursion.</param>
    /// <returns>Returns true if the PropertyInfo is found (nested), false otherwise.</returns>
    public static bool IsNested(PropertyData start, PropertyInfo property, IList<string> skipNamespaces, IList<Type> skipTypes)
    {
        if ((start != null) && (property.DeclaringType != null))
        {
            if (skipTypes.Contains(property.DeclaringType))
            {
                return true;
            }
            if (SkipNamespace(skipNamespaces, property.DeclaringType.Namespace))
            {
                return true;
            }
        }
        var parent = start;
        while (parent != null)
        {
            if ((parent.PropertyInfo.Name == property.Name)
             && (parent.PropertyInfo.PropertyType == property.PropertyType)
             && (parent.PropertyInfo.DeclaringType == property.DeclaringType))
            {
                return true;
            }
            parent = parent.Parent;
        }
        return false;
    }

    /// <summary>Gets the current value of the property. This will result in exceptions if <see cref="CanGetValue"/> == false.</summary>
    public object GetValue() => PropertyInfo.GetValue(Source, null);

    /// <summary>Gets the current property value of the specified object. The object has to match the PropertyInfo.DeclaringType.</summary>
    /// <param name="source">Source object to read from.</param>
    /// <returns>Returns the property value.</returns>
    public object GetValueOf(object source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (source.GetType() != PropertyInfo.DeclaringType)
        {
            throw new ArgumentOutOfRangeException(nameof(source));
        }

        return PropertyInfo.GetValue(source, null);
    }

    /// <summary>Sets the current value of the property. This will result in exceptions if <see cref="CanSetValue"/> == false.</summary>
    public void SetValue(object value) => PropertyInfo.SetValue(Source, value, null);

    /// <summary>Tries to get the value and catches all exceptions the properties getter throws.</summary>
    /// <returns>Returns the value of the property, null if value cannot be retrieved.</returns>
    public object TryGetValue()
    {
        try
        {
            return PropertyInfo.GetValue(Source, null);
        }
        catch
        {
            return null;
        }
    }

    #endregion Public Methods
}
