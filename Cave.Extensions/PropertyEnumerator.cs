using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cave
{
    /// <summary>
    /// Enumerator for properties of an object.
    /// </summary>
    public class PropertyEnumerator : IEnumerator<PropertyData>, IEnumerable<PropertyData>
    {
        Stack<PropertyData> stack;

        /// <summary>
        /// Gets the used <see cref="BindingFlags"/>.
        /// </summary>
        public BindingFlags BindingFlags { get; }

        /// <summary>
        /// Gets the root type.
        /// </summary>
        public Type Root { get; }

        /// <summary>
        /// Gets a value indicating whether only the <see cref="Root"/> objects properties are returned or even properties of properties.
        /// </summary>
        public bool Recursive { get; }

        public PropertyEnumerator(Type type, BindingFlags bindingFlags, bool recursive = false)
        {
            Root = type;
            BindingFlags = bindingFlags;
            Recursive = recursive;
            Reset();
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if (stack.Count == 0) return false;
            Current = stack.Pop();
            if (Recursive)
            {
                AddProperties(Current.FullPath, Current.PropertyInfo.PropertyType);
            }
            return true;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            stack = new Stack<PropertyData>();
            AddProperties("", Root);
        }

        void AddProperties(string rootPath, Type type)
        {
            foreach (var property in type.GetProperties(BindingFlags))
            {
                stack.Push(new PropertyData(rootPath, property, null));
            }
        }

        /// <inheritdoc/>
        public PropertyData Current { get; private set; }

        /// <inheritdoc/>
        object IEnumerator.Current => Current;

        /// <inheritdoc/>
        public void Dispose() { GC.SuppressFinalize(this); }

        /// <inheritdoc/>
        IEnumerator<PropertyData> IEnumerable<PropertyData>.GetEnumerator() => new PropertyEnumerator(Root, BindingFlags, Recursive);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => new PropertyEnumerator(Root, BindingFlags, Recursive);
    }
}
