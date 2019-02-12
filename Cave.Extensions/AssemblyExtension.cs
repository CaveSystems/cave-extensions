using System;
using System.IO;
using System.Reflection;

namespace Cave
{
    /// <summary>
    /// Provides extensions for <see cref="Assembly"/> instances
    /// </summary>
    public static class AssemblyExtension
    {
        /// <summary>
        /// Obtains a local file path for the specified assembly.
        /// </summary>
        /// <param name="assembly">Assembly instance</param>
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
