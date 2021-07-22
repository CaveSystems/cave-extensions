using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Cave
{
    /// <summary>Gets <see cref="AppDomain" /> specific extensions.</summary>
    public static class AppDom
    {
        #region LoadFlags enum

        /// <summary>Gets loader modes.</summary>
        [Flags]
        public enum LoadFlags
        {
            /// <summary>Throw exceptions on loader error and do not load additional assemblies.</summary>
            None = 0,

            /// <summary>Do not throw exceptions</summary>
            NoException = 1,

            /// <summary>Try to load assemblies if type cannot be found (insecure!)</summary>
            /// <remarks>Using this function may result in a security risk if someone can put assemblies to the program folder!</remarks>
            LoadAssemblies = 2
        }

        #endregion

        #region Static

        /// <summary>Finds the loaded assembly with the specified name.</summary>
        /// <param name="name">The name of the assembly.</param>
        /// <param name="throwException">if set to <c>true</c> [throw exception if no assembly can be found].</param>
        /// <returns>Returns the first matching type.</returns>
        /// <exception cref="ArgumentException">Cannot find assembly {0}.</exception>
        public static Assembly FindAssembly(string name, bool throwException)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == name)
                {
                    return assembly;
                }
            }

            if (throwException)
            {
                throw new ArgumentException($"Cannot find assembly {name}");
            }

            return null;
        }

        /// <summary>Finds the type with the specified name.</summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="mode">The loader mode.</param>
        /// <returns>Returns the first matching type.</returns>
        /// <exception cref="System.TypeLoadException">when type cannot be loaded.</exception>
        [Obsolete("Use FindType(string typeName, string assemblyName = null, LoadMode mode = 0)")]
        public static Type FindType(string name, LoadFlags mode = 0) => FindType(name.BeforeFirst(','), null, mode);

        /// <summary>Finds the type with the specified name.</summary>
        /// <param name="typeName">The full qualified assemblyname of the type (e.g. System.Uri).</param>
        /// <param name="assemblyName">The assemblyname to search in (e.g. System).</param>
        /// <param name="mode">The loader mode.</param>
        /// <returns>Returns the first matching type.</returns>
        /// <exception cref="System.TypeLoadException">when type cannot be loaded.</exception>
#pragma warning disable CS0618 // Allow obsolete functions/fields
        public static Type FindType(string typeName, string assemblyName = null, LoadFlags mode = 0)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            if (typeName.IndexOf(',') > -1)
            {
                throw new ArgumentOutOfRangeException(nameof(typeName), "Type name has to be the full qualified typename without additional arguments.");
            }

            // try direct load with assembly name first.
            if (assemblyName != null)
            {
                var type = Type.GetType($"{typeName}, {assemblyName}", false);
                if (type != null)
                {
                    return type;
                }
            }
            else
            {
                // try direct load.
                var type = Type.GetType(typeName, false);
                if (type != null)
                {
                    return type;
                }
            }

            // iterate all loaded assemblies
            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (assemblyName != null)
            {
                assemblies = assemblies.Where(a => a.GetName().Name == assemblyName);
                if (!assemblies.Any())
                {
                    Trace.TraceWarning("Could not find assembly <red>{0}", assemblyName);
                }
            }

            foreach (var assembly in assemblies)
            {
                Trace.TraceInformation("Searching for type <cyan>{0}<default> in assembly <cyan>{1}", typeName, assembly);
                var type = assembly.GetType(typeName, false);
                if (type != null)
                {
                    Trace.TraceInformation("Using type <green>{0}", type.FullName);
                    return type;
                }
            }

            // load assembly
            if (((mode & LoadFlags.LoadAssemblies) != 0) && (assemblyName != null))
            {
                var assembly = Assembly.LoadWithPartialName(assemblyName);
                if (assembly != null)
                {
                    Trace.TraceInformation("<red>(Insecure)<default> loaded assembly <yellow>{0}", assembly);
                    var type = assembly.GetType(typeName, false);
                    if (type != null)
                    {
                        Trace.TraceInformation("Using type <yellow>{0}", type.FullName);
                        return type;
                    }
                }
            }

            Trace.TraceError("Could not find type <red>{0}", typeName);
            if ((mode & LoadFlags.NoException) == 0)
            {
                throw new TypeLoadException($"Cannot load type {typeName}");
            }

            return null;
        }
#pragma warning restore CS0618

        /// <summary>Gets the installation unique identifier.</summary>
        /// <returns>unique id as GUID.</returns>
        public static Guid GetInstallationGuid() => InstallationGuid.SystemGuid;

        /// <summary>Gets all loaded types assignable to the specified one and containing a default constructor.</summary>
        /// <typeparam name="T">Type or interface all types need to be assignable to.</typeparam>
        /// <returns>Returns a list of types.</returns>
        public static List<T> GetTypes<T>()
        {
            var interfaceType = typeof(T);
            var types = new List<T>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.IsAbstract)
                    {
                        continue;
                    }

                    if (interfaceType.IsAssignableFrom(type))
                    {
                        try
                        {
                            var api = (T)Activator.CreateInstance(type);
                            types.Add(api);
                        }
                        catch
                        {
                            Trace.TraceError($"Could not create instance of type {type}!");
                        }
                    }
                }
            }

            return types;
        }

        /// <summary>Gets the installation identifier.</summary>
        /// <value>The installation identifier.</value>
        /// <exception cref="NotSupportedException">if <see cref="Guid.GetHashCode()" /> failes.</exception>
        [Obsolete("Use InstallationGuid.ProgramGuid")]
        public static uint ProgramID => (uint)InstallationGuid.ProgramGuid.ToString().GetHashCode();

        #endregion
    }
}
