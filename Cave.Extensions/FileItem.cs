#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)

using System;
using System.Diagnostics;
using System.IO;

namespace Cave;

/// <summary>Gets a file path string handler.</summary>
[DebuggerDisplay("{FullPath}")]
public sealed class FileItem
{
    #region Static

    /// <summary>Creates a new file instance from a specified base path and the full path to the file. (The subdirectories will be extracted.)</summary>
    /// <param name="baseDirectory">The base file.</param>
    /// <param name="fullFilePath">The full path of the file.</param>
    /// <returns>A new file item.</returns>
    public static FileItem FromFullPath(string baseDirectory, string fullFilePath)
    {
        var relativePath = GetRelative(fullFilePath, baseDirectory);
        return new(baseDirectory, relativePath);
    }

    /// <summary>Gets a relative path.</summary>
    /// <param name="fullPath">The full path of the file / directory.</param>
    /// <param name="basePath">The base path of the file / directory.</param>
    /// <returns>A relative path.</returns>
    public static string GetRelative(string fullPath, string basePath)
    {
        if (fullPath == null)
        {
            throw new ArgumentNullException(nameof(fullPath));
        }

        if (basePath == null)
        {
            throw new ArgumentNullException(nameof(basePath));
        }

        var chars = new[]
        {
            '\\',
            '/'
        };
        var relative = fullPath.Split(chars, StringSplitOptions.RemoveEmptyEntries);
        var baseCheck = basePath.Split(chars, StringSplitOptions.RemoveEmptyEntries);
        var comparison = Platform.IsMicrosoft ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
        for (var i = 0; i < baseCheck.Length; i++)
        {
            if (!string.Equals(baseCheck[i], relative[i], comparison))
            {
                throw new ArgumentException($"BasePath {basePath} is not a valid base for FullPath {fullPath}!");
            }
        }

        return "." + Path.DirectorySeparatorChar +
            string.Join($"{Path.DirectorySeparatorChar}", relative, baseCheck.Length, relative.Length - baseCheck.Length);
    }

    /// <summary>Converts the file item to string containing the full path.</summary>
    /// <param name="file">The file item to convert.</param>
    /// <returns>The full path.</returns>
    public static implicit operator string(FileItem file) => file?.FullPath;

    #endregion

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="FileItem" /> class.</summary>
    /// <param name="baseDirectory">The base directory.</param>
    /// <param name="subDirectoryAndName">The subdirectory and name of the file.</param>
    public FileItem(string baseDirectory, string subDirectoryAndName)
    {
        BaseDirectory = FileSystem.GetFullPath(baseDirectory);
        Relative = subDirectoryAndName;
        FullPath = FileSystem.GetFullPath(FileSystem.Combine(BaseDirectory, Relative));
    }

    #endregion

    #region Properties

    /// <summary>Gets the base directory (used when searching for this file).</summary>
    public string BaseDirectory { get; }

    /// <summary>Gets the directory.</summary>
    /// <value>The directory.</value>
    public string Directory
    {
        get
        {
            var i = FullPath.LastIndexOfAny(new[]
            {
                '\\',
                '/'
            });
            return FullPath.Substring(0, i);
        }
    }

    /// <summary>Gets the extension.</summary>
    /// <value>The extension.</value>
    public string Extension => Path.GetExtension(FullPath);

    /// <summary>Gets the full path of the file.</summary>
    public string FullPath { get; }

    /// <summary>Gets the file name with extension.</summary>
    /// <value>The file name with extension.</value>
    public string Name => Path.GetFileName(FullPath);

    /// <summary>Gets the relative path.</summary>
    public string Relative { get; }

    #endregion
}

#endif
