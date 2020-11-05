using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cave
{
    public class PropertyValueEnumerator : IEnumerator<PropertyData>, IEnumerable<PropertyData>
    {
        Stack<PropertyData> stack;

        public BindingFlags BindingFlags { get; }

        public object Root { get; }

        public bool Recursive { get; }

        public PropertyValueEnumerator(object instance, BindingFlags bindingFlags, bool recursive = false)
        {
            Root = instance;
            BindingFlags = bindingFlags;
            Recursive = recursive;
            Reset();
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if (stack.Count == 0) return false;
            var current = stack.Pop();
            if (Recursive)
            {
                AddProperties(current.FullPath, current.Value);
            }

            Current = current;
            return true;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            stack = new Stack<PropertyData>();
            AddProperties("", Root);
        }

        void AddProperties(string rootPath, object instance)
        {
            if (instance == null) return;
            foreach (var property in instance.GetType().GetProperties(BindingFlags))
            {
                stack.Push(new PropertyData(rootPath, property, instance));
            }
        }

        /// <inheritdoc/>
        public PropertyData Current { get; private set; }

        /// <inheritdoc/>
        object IEnumerator.Current => Current;

        /// <inheritdoc/>
        public void Dispose() { stack = null; }

        /// <inheritdoc/>
        IEnumerator<PropertyData> IEnumerable<PropertyData>.GetEnumerator() => new PropertyValueEnumerator(Root, BindingFlags, Recursive);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => new PropertyValueEnumerator(Root, BindingFlags, Recursive);
    }
}
