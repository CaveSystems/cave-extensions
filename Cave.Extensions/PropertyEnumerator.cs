using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cave
{
    /// <summary>Enumerator for properties of an object.</summary>
    public class PropertyEnumerator : IEnumerator<PropertyData>, IEnumerable<PropertyData>
    {
        Stack<PropertyData> stack;

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="PropertyEnumerator" /> class.</summary>
        /// <param name="type">Type to iterate.</param>
        /// <param name="obj">The source object of <paramref name="type"/>.</param>
        /// <param name="bindingFlags">Property binding flags.</param>
        /// <param name="recursive">Recursive property search.</param>
        /// <param name="filter">Allows to filter properties.</param>
        public PropertyEnumerator(Type type, object obj, BindingFlags bindingFlags, bool recursive = false, PropertyDataFilter filter = null)
        {
            Filter = filter;
            RootObject = obj;
            RootType = type;
            BindingFlags = bindingFlags;
            Recursive = recursive;
            Reset();
        }

        #endregion

        #region Properties

        /// <summary>Gets the filter used.</summary>
        public PropertyDataFilter Filter { get; }

        /// <summary>Gets the used <see cref="BindingFlags" />.</summary>
        public BindingFlags BindingFlags { get; }

        /// <summary>Gets a value indicating whether only the <see cref="RootType" /> objects properties are returned or even properties of properties.</summary>
        public bool Recursive { get; }

        /// <summary>Gets the root type.</summary>
        public Type RootType { get; }

        /// <summary>Gets the root type.</summary>
        public object RootObject { get; }

        /// <summary>
        /// Gets or sets the types we will not recurse into.
        /// </summary>
        public Type[] SkipTypes { get; set; } = PropertyData.DefaultSkipTypes;

        /// <summary>
        /// Gets or sets the namespaces we will not recurse into.
        /// </summary>
        public string[] SkipNamespaces { get; set; } = PropertyData.DefaultSkipNamespaces;

        #endregion

        #region IEnumerable<PropertyData> Members

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => new PropertyEnumerator(RootType, RootObject, BindingFlags, Recursive, Filter);

        /// <inheritdoc />
        IEnumerator<PropertyData> IEnumerable<PropertyData>.GetEnumerator() => new PropertyEnumerator(RootType, RootObject, BindingFlags, Recursive, Filter);

        #endregion

        #region IEnumerator<PropertyData> Members

        /// <inheritdoc />
        public void Dispose() => GC.SuppressFinalize(this);

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (stack.Count == 0)
            {
                return false;
            }

            Current = stack.Pop() ?? throw new InvalidOperationException();
            if (Recursive)
            {
                AddProperties(Current, Current.FullPath, Current.PropertyInfo.PropertyType, Current.CanGetValue ? Current.TryGetValue() : null);
            }

            return true;
        }

        /// <inheritdoc />
        public void Reset()
        {
            stack = new Stack<PropertyData>();
            AddProperties(null, string.Empty, RootType, RootObject);
        }

        /// <inheritdoc />
        public PropertyData Current { get; private set; }

        #endregion

        #region Members

        void AddProperties(PropertyData parent, string rootPath, Type type, object instance)
        {
            foreach (var property in type.GetProperties(BindingFlags))
            {
                // skip nested
                if (PropertyData.IsNested(parent, property, SkipNamespaces, SkipTypes))
                {
                    continue;
                }

                var data = new PropertyData(parent, rootPath, property, instance);
                if (Filter != null && Filter(data))
                {
                    continue;
                }

                stack.Push(data);
            }
        }

        #endregion
    }
}
