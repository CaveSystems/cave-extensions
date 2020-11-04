using System;
using System.Diagnostics;
using System.Reflection;

namespace Cave
{
    /// <summary>
    /// Provides property information, full path and value access for the <see cref="PropertyEnumerator"/> and <see cref="PropertyValueEnumerator"/> classes.
    /// </summary>
    [DebuggerDisplay("{FullPath}")]
    public class PropertyData
    {
        /// <summary>
        /// Gets the property information.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets the root path of the property.
        /// </summary>
        public string RootPath { get; }

        /// <summary>
        /// Gets the full path of the property.
        /// </summary>
        public string FullPath => $"{RootPath}/{PropertyInfo.Name}";

        /// <summary>
        /// Gets the source object of this property.
        /// This is null at <see cref="PropertyEnumerator"/> and may be null at <see cref="PropertyValueEnumerator"/> if the property value or root property value is null.
        /// </summary>
        public object Source { get; }

        /// <summary>
        ///Gets the current value of the property.
        /// This is null at <see cref="PropertyEnumerator"/>.
        /// </summary>
        public object Value => Source == null ? null : PropertyInfo.GetValue(Source, null);

        /// <summary>
        /// Creates a new instance of the <see cref="PropertyData"/> class.
        /// </summary>
        /// <param name="rootPath">The root path. This may not be null.</param>
        /// <param name="propertyInfo">The property info. This may not be null.</param>
        /// <param name="source">The source object of the property.</param>
        public PropertyData(string rootPath, PropertyInfo propertyInfo, object source = null)
        {
            RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            Source = source;
        }
    }
}
