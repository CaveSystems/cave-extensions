using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cave
{
    /// <summary>
    /// Provides extensions for <see cref="Assembly"/> instances.
    /// </summary>
    public static class AssemblyExtension
    {
#if NET20 || NET35 || NET40
        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified assembly.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute to search for.</typeparam>
        /// <param name="assembly">The assembly to inspect.</param>
        /// <returns>A collection of the custom attributes that are applied to element and that match T, or an empty collection if no such attributes exist.</returns>
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this Assembly assembly)
            where TAttribute : Attribute
        {
            List<TAttribute> result = new List<TAttribute>();
            foreach (var attribute in assembly.GetCustomAttributes(typeof(TAttribute), true))
            {
                result.Add((TAttribute)attribute);
            }
            return result;
        }
#endif

        /// <summary>
        /// Get the assembly product name using the <see cref="AssemblyProductAttribute"/>.
        /// </summary>
        /// <param name="assembly">The assembly to inspect.</param>
        /// <returns>The product name.</returns>
        public static string GetProductName(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var attributes = assembly.GetCustomAttributes<AssemblyProductAttribute>();
            return attributes.FirstOrDefault()?.Product ?? throw new ArgumentException("Product attribute unset!");
        }

        /// <summary>
        /// Get the assembly company name using the <see cref="AssemblyCompanyAttribute"/>.
        /// </summary>
        /// <param name="assembly">The assembly to inspect.</param>
        /// <returns>The company name.</returns>
        public static string GetCompanyName(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var attributes = assembly.GetCustomAttributes<AssemblyCompanyAttribute>();
            return attributes.FirstOrDefault()?.Company ?? throw new ArgumentException("Company attribute unset!");
        }

        /// <summary>
        /// Gets a local file path for the specified assembly.
        /// </summary>
        /// <param name="assembly">Assembly instance.</param>
        /// <returns>Returns the file path.</returns>
        public static string GetAssemblyFilePath(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

#if !NETSTANDARD13
            {
                string path = assembly.Location;

                // strip connection string
                if (ConnectionString.TryParse(path, out ConnectionString connectionString))
                {
                    path = connectionString.Location;
                }
                if (File.Exists(path))
                {
                    return path;
                }
            }
#endif
            {
                var path = assembly.ManifestModule.FullyQualifiedName;
                if (ConnectionString.TryParse(path, out ConnectionString connectionString))
                {
                    path = connectionString.Location;
                }
                if (File.Exists(path))
                {
                    return path;
                }
            }

            throw new FileNotFoundException("Could not resolve Assembly path.", assembly.ToString());
        }
    }
}
