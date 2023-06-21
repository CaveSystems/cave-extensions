using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cave;

/// <summary>Enumerator for valid properties of an object.</summary>
public sealed class PropertyValueEnumerator : IEnumerator<PropertyData>, IEnumerable<PropertyData>
{
    #region Fields

    Stack<PropertyData> stack;

    #endregion

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="PropertyValueEnumerator" /> class.</summary>
    /// <param name="instance">Instance to iterate.</param>
    /// <param name="bindingFlags">Property binding flags.</param>
    /// <param name="recursive">Recursive property search.</param>
    /// <param name="filter">Allows to filter properties.</param>
    public PropertyValueEnumerator(object instance, BindingFlags bindingFlags, bool recursive = false, PropertyDataFilter filter = null)
    {
        Filter = filter;
        Root = instance;
        BindingFlags = bindingFlags;
        Recursive = recursive;
        Reset();
    }

    #endregion

    #region Properties

    /// <summary>Gets the used <see cref="BindingFlags" />.</summary>
    public BindingFlags BindingFlags { get; }

    /// <summary>Gets the filter used.</summary>
    public PropertyDataFilter Filter { get; }

    /// <summary>Gets a value indicating whether only the <see cref="Root" /> objects properties are returned or even properties of properties.</summary>
    public bool Recursive { get; }

    /// <summary>Gets the root type.</summary>
    public object Root { get; }

    /// <summary>Gets or sets the namespaces we will not recurse into.</summary>
    public string[] SkipNamespaces { get; set; } = PropertyData.DefaultSkipNamespaces;

    /// <summary>Gets or sets the types we will not recurse into.</summary>
    public Type[] SkipTypes { get; set; } = PropertyData.DefaultSkipTypes;

    #endregion

    #region IEnumerable<PropertyData> Members

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => new PropertyValueEnumerator(Root, BindingFlags, Recursive, Filter);

    /// <inheritdoc />
    IEnumerator<PropertyData> IEnumerable<PropertyData>.GetEnumerator() => new PropertyValueEnumerator(Root, BindingFlags, Recursive, Filter);

    #endregion

    #region IEnumerator<PropertyData> Members

    /// <inheritdoc />
    public void Dispose() => stack = null;

    /// <inheritdoc />
    object IEnumerator.Current => Current;

    /// <inheritdoc />
    public bool MoveNext()
    {
        if (stack.Count == 0)
        {
            return false;
        }

        var current = stack.Pop();
        if (Recursive)
        {
            object value = null;
            if (current.CanGetValue)
            {
                try
                {
                    value = current.Value;
                }
                catch
                {
                    /* getter throws error */
                }
            }
            AddProperties(current, value);
        }

        Current = current;
        return true;
    }

    /// <inheritdoc />
    public void Reset()
    {
        stack = new();
        AddProperties(null, Root);
    }

    /// <inheritdoc />
    public PropertyData Current { get; private set; }

    #endregion

    #region Members

    void AddProperties(PropertyData parent, object instance)
    {
        if (instance == null) return;
        if (instance is string) return;

        var instanceType = instance.GetType();
        foreach (var propertyInfo in instanceType.GetProperties(BindingFlags))
        {
            // skip nested
            if (PropertyData.IsNested(parent, propertyInfo, SkipNamespaces, SkipTypes))
            {
                continue;
            }

            if (propertyInfo.IsIndexProperty())
            {
                continue;
            }

            var data = new PropertyData(parent, propertyInfo, instance);
            if ((Filter != null) && Filter(data))
            {
                continue;
            }

            stack.Push(data);
        }

        if (instance is IEnumerable enumerable)
        {
            try
            {
                HandleEnumerable(parent, enumerable);
            }
            catch { }
        }
    }

    void HandleEnumerable(PropertyData parent, IEnumerable enumerable)
    {
        var i = -1;
        foreach (var item in enumerable)
        {
            i++;
            if (item is null)
            {
                continue;
            }
            var type = item.GetType();
            foreach (var property in type.GetProperties(BindingFlags))
            {
                // skip nested
                if (PropertyData.IsNested(parent, property, SkipNamespaces, SkipTypes))
                {
                    continue;
                }

                var data = new PropertyData(parent, property, item, i);
                if ((Filter != null) && Filter(data))
                {
                    continue;
                }

                stack.Push(data);
            }
        }
    }

    #endregion
}
