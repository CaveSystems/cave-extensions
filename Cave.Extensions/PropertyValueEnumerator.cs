using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cave
{
    /// <summary>Enumerator for valid properties of an object.</summary>
    public sealed class PropertyValueEnumerator : IEnumerator<PropertyData>, IEnumerable<PropertyData>
    {
        Stack<PropertyData> stack;

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="PropertyValueEnumerator" /> class.</summary>
        /// <param name="instance">Instance to iterate.</param>
        /// <param name="bindingFlags">Property binding flags.</param>
        /// <param name="recursive">Recursive property search.</param>
        public PropertyValueEnumerator(object instance, BindingFlags bindingFlags, bool recursive = false)
        {
            Root = instance;
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
        public object Root { get; }

        #endregion

        #region IEnumerable<PropertyData> Members

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => new PropertyValueEnumerator(Root, BindingFlags, Recursive);

        /// <inheritdoc />
        IEnumerator<PropertyData> IEnumerable<PropertyData>.GetEnumerator() => new PropertyValueEnumerator(Root, BindingFlags, Recursive);

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
                AddProperties(current.FullPath, current.Value);
            }

            Current = current;
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

        void AddProperties(string rootPath, object instance)
        {
            if (instance == null)
            {
                return;
            }

            foreach (var property in instance.GetType().GetProperties(BindingFlags))
            {
                stack.Push(new PropertyData(rootPath, property, instance));
            }
        }

        #endregion
    }
}
