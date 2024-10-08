﻿#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#pragma warning disable CS0618 // Allow obsolete functions/fields

namespace Cave;

/// <summary>Gets <see cref="AppDomain"/> specific extensions.</summary>
public static class AppDom
{
    #region Public Enums

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

    #endregion Public Enums

    #region Public Methods

    /// <summary>Finds the loaded assembly with the specified name.</summary>
    /// <param name="name">The name of the assembly (this can be a full qualified or a short name).</param>
    /// <param name="throwException">if set to <c>true</c> [throw exception if no assembly can be found].</param>
    /// <returns>Returns the first matching type.</returns>
    /// <exception cref="ArgumentException">Cannot find assembly {0}.</exception>
    public static Assembly? FindAssembly(string name, bool throwException)
    {
        if (name.Contains(','))
        {
            var asmName = new AssemblyName(name);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (AssemblyName.ReferenceMatchesDefinition(assembly.GetName(), asmName))
                {
                    return assembly;
                }
            }
        }
        else
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == name)
                {
                    return assembly;
                }
            }
        }

        if (throwException)
        {
            throw new ArgumentException($"Cannot find assembly {name}");
        }

        return null;
    }

    /// <summary>Finds the type with the specified name.</summary>
    /// <param name="name">The name of the type (this can be a full type name or a full qualified name with assembly information).</param>
    /// <param name="mode">The loader mode.</param>
    /// <returns>Returns the first matching type.</returns>
    /// <exception cref="System.TypeLoadException">when type cannot be loaded.</exception>
    public static Type? FindType(string name, LoadFlags mode = 0)
    {
        if (name.Contains(','))
        {
            var typeName = name.BeforeFirst(',').Trim();
            var assemblyName = name.AfterFirst(',').Trim();
            var assembly =
                mode.HasFlag(LoadFlags.LoadAssemblies) ?
                    Assembly.Load(assemblyName) ?? throw new TypeLoadException($"Could not load assembly {assemblyName}") :
                    FindAssembly(assemblyName, true);
#if NET20 || NET35
            var type = assembly?.GetType(typeName, true);
#else
            var type = assembly?.GetType(typeName, true, false);
#endif
            if (type is not null) return type;
        }
        return FindType(name, null, mode);
    }

    /// <summary>Finds the type with the specified name.</summary>
    /// <param name="typeName">The full qualified assemblyname of the type (e.g. System.Uri).</param>
    /// <param name="assemblyName">The assemblyname to search in (e.g. System).</param>
    /// <param name="mode">The loader mode.</param>
    /// <returns>Returns the first matching type.</returns>
    /// <exception cref="System.TypeLoadException">when type cannot be loaded.</exception>
    public static Type? FindType(string typeName, string? assemblyName, LoadFlags mode = 0)
    {
        if (typeName == null)
        {
            throw new ArgumentNullException(nameof(typeName));
        }

        if (typeName.IndexOf(',') > -1)
        {
            throw new ArgumentOutOfRangeException(nameof(typeName), "Type name has to be the full typename without assembly arguments.");
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
                Trace.TraceWarning($"Could not find assembly {assemblyName}");
            }
        }

        foreach (var assembly in assemblies)
        {
            var type = assembly.GetType(typeName, false);
            if (type != null)
            {
                return type;
            }
        }

        // load assembly
        if (((mode & LoadFlags.LoadAssemblies) != 0) && (assemblyName != null))
        {
            var assembly = Assembly.LoadWithPartialName(assemblyName);
            if (assembly != null)
            {
                Trace.TraceWarning("Insecure Assembly.LoadWithPartialName {0}", assembly);
                var type = assembly.GetType(typeName, false);
                if (type != null)
                {
                    return type;
                }
            }
        }

        Trace.TraceError("Could not find type {0}", typeName);
        if ((mode & LoadFlags.NoException) == 0)
        {
            throw new TypeLoadException($"Cannot load type {typeName}");
        }

        return null;
    }

    /// <summary>Gets all loaded types assignable to the specified one.</summary>
    /// <typeparam name="T">Type or interface all types need to be assignable to.</typeparam>
    /// <returns>Returns a list of types.</returns>
    public static IEnumerable<Type> FindTypes<T>() => FindTypes(typeof(T));

    /// <summary>Gets all loaded types assignable to the specified one.</summary>
    /// <param name="predicate">Filter function.</param>
    /// <returns>Returns a list of types.</returns>
    public static IEnumerable<Type> FindTypes(Func<Type, bool>? predicate = null)
    {
        var selector = AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes());
        if (predicate != null)
        {
            selector = selector.Where(predicate);
        }
        return selector;
    }

    /// <summary>Gets all loaded types assignable to the specified one.</summary>
    /// <param name="ofType">Type or interface all types need to be assignable to.</param>
    /// <returns>Returns a list of types.</returns>
    public static IEnumerable<Type> FindTypes(Type ofType)
        => FindTypes(type => !type.IsAbstract && ofType.IsAssignableFrom(type));

    /// <summary>Gets the installation unique identifier.</summary>
    /// <returns>unique id as GUID.</returns>
    public static Guid GetInstallationGuid() => InstallationGuid.SystemGuid;

    /// <summary>Searches all loaded types assignable to the specified one and containing a default constructor.</summary>
    /// <typeparam name="T">Type or interface all types need to be assignable to.</typeparam>
    /// <returns>Returns a list of new instances.</returns>
    public static List<T> GetInstances<T>(bool ignoreExceptions)
    {
        var types = new List<T>();
        foreach (var type in FindTypes(typeof(T)))
        {
            try
            {
                var obj = Activator.CreateInstance(type);
                if (obj is T api)
                {
                    types.Add(api);
                }
                else if (!ignoreExceptions)
                {
                    throw new Exception($"Cannot convert {type} {obj} to {typeof(T)}!");
                }
            }
            catch
            {
                Trace.TraceError($"Could not create instance of type {type}!");
                if (!ignoreExceptions)
                {
                    throw;
                }
            }
        }

        return types;
    }

    #endregion Public Methods
}

#pragma warning restore CS0618

#endif
