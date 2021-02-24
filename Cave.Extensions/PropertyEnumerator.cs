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
        /// <param name="bindingFlags">Property binding flags.</param>
        /// <param name="recursive">Recursive property search.</param>
        public PropertyEnumerator(Type type, BindingFlags bindingFlags, bool recursive = false)
        {
            Root = type;
            BindingFlags = bindingFlags;
            Recursive = recursive;
            Reset();
        }

        #endregion

        #region Properties

        /// <summary>Gets the used <see cref="BindingFlags" />.</summary>
        public BindingFlags BindingFlags { get; }

        /// <summary>Gets a value indicating whether only the <see cref="Root" /> objects properties are returned or even properties of properties.</summary>
        public bool Recursive { get; }

        /// <summary>Gets the root type.</summary>
        public Type Root { get; }

        #endregion

        #region IEnumerable<PropertyData> Members

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => new PropertyEnumerator(Root, BindingFlags, Recursive);

        /// <inheritdoc />
        IEnumerator<PropertyData> IEnumerable<PropertyData>.GetEnumerator() => new PropertyEnumerator(Root, BindingFlags, Recursive);

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
                AddProperties(Current.FullPath, Current.PropertyInfo.PropertyType);
            }

            return true;
        }

        /// <inheritdoc />
        public void Reset()
        {
            stack = new Stack<PropertyData>();
            AddProperties(string.Empty, Root);
        }

        /// <inheritdoc />
        public PropertyData Current { get; private set; }

        #endregion

        #region Members

        void AddProperties(string rootPath, Type type)
        {
            foreach (var property in type.GetProperties(BindingFlags))
            {
                // skip nested
                if (property.PropertyType == type)
                {
                    continue;
                }

                stack.Push(new PropertyData(rootPath, property));
            }
        }

        #endregion
    }
}
