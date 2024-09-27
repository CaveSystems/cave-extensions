using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cave;

/// <summary>Enumerator for properties of an object.</summary>
public class PropertyEnumerator : IEnumerator<PropertyData>, IEnumerable<PropertyData>
{
    #region Private Fields

    readonly Stack<PropertyData> stack = new();
    PropertyData? current;

    #endregion Private Fields

    #region Private Methods

    void AddProperties(PropertyData? parent, Type type, object? instance)
    {
        foreach (var property in type.GetProperties(BindingFlags))
        {
            // skip nested
            if (PropertyData.IsNested(parent, property, SkipNamespaces, SkipTypes))
            {
                continue;
            }

            var data = new PropertyData(parent, property, instance);
            if ((Filter != null) && Filter(data))
            {
                continue;
            }

            stack.Push(data);
        }
    }

    #endregion Private Methods

    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="PropertyEnumerator"/> class.</summary>
    /// <param name="type">Type to iterate.</param>
    /// <param name="obj">The source object of <paramref name="type"/>.</param>
    /// <param name="bindingFlags">Property binding flags.</param>
    /// <param name="recursive">Recursive property search.</param>
    /// <param name="filter">Allows to filter properties.</param>
    public PropertyEnumerator(Type type, object obj, BindingFlags bindingFlags, bool recursive = false, PropertyDataFilter? filter = null)
    {
        Filter = filter;
        RootObject = obj;
        RootType = type;
        BindingFlags = bindingFlags;
        Recursive = recursive;
        Reset();
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the used <see cref="BindingFlags"/>.</summary>
    public BindingFlags BindingFlags { get; }

    /// <inheritdoc/>
    public PropertyData Current => current ?? throw new InvalidOperationException("Reset required!");

    /// <summary>Gets the filter used.</summary>
    public PropertyDataFilter? Filter { get; }

    /// <summary>Gets a value indicating whether this instance was disposed or not.</summary>
    public bool IsDisposed { get; private set; }

    /// <summary>Gets a value indicating whether only the <see cref="RootType"/> objects properties are returned or even properties of properties.</summary>
    public bool Recursive { get; }

    /// <summary>Gets the root type.</summary>
    public object RootObject { get; }

    /// <summary>Gets the root type.</summary>
    public Type RootType { get; }

    /// <summary>Gets or sets the namespaces we will not recurse into.</summary>
    public string[] SkipNamespaces { get; set; } = PropertyData.DefaultSkipNamespaces;

    /// <summary>Gets or sets the types we will not recurse into.</summary>
    public Type[] SkipTypes { get; set; } = PropertyData.DefaultSkipTypes;

    /// <inheritdoc/>
    object IEnumerator.Current => Current;

    #endregion Public Properties

    #region Public Methods

    /// <inheritdoc/>
    public void Dispose()
    {
        IsDisposed = true;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        if (IsDisposed) throw new ObjectDisposedException(nameof(PropertyEnumerator));
        if (stack.Count == 0)
        {
            return false;
        }

        current = stack.Pop() ?? throw new InvalidOperationException();
        if (Recursive)
        {
            AddProperties(Current, Current.PropertyInfo.PropertyType, Current.CanGetValue ? Current.TryGetValue() : null);
        }

        return true;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        current = null;
        stack.Clear();
        AddProperties(null, RootType, RootObject);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => new PropertyEnumerator(RootType, RootObject, BindingFlags, Recursive, Filter);

    /// <inheritdoc/>
    IEnumerator<PropertyData> IEnumerable<PropertyData>.GetEnumerator() => new PropertyEnumerator(RootType, RootObject, BindingFlags, Recursive, Filter);

    #endregion Public Methods
}
