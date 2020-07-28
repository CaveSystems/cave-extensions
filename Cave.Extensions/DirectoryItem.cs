using System;
using System.Diagnostics;
using System.IO;

namespace Cave
{
    /// <summary>Gets a directory item. Directories are always rooted!.</summary>
    [DebuggerDisplay("{FullPath}")]
    public sealed class DirectoryItem
    {
        /// <summary>Initializes a new instance of the <see cref="DirectoryItem" /> class.</summary>
        /// <param name="baseDirectory">The base directory.</param>
        /// <param name="subDirectory">The subdirectory.</param>
        public DirectoryItem(string baseDirectory, string subDirectory)
        {
            BaseDirectory = baseDirectory;
            Relative = subDirectory;
            FullPath = FileSystem.GetFullPath(FileSystem.Combine(BaseDirectory, Relative));
        }

        /// <summary>Gets the base directory (used when searching for this file).</summary>
        public string BaseDirectory { get; }

        /// <summary>Gets the relative path.</summary>
        public string Relative { get; }

        /// <summary>Gets the full path of the directory.</summary>
        public string FullPath { get; }

        /// <summary>Gets a relative path.</summary>
        /// <param name="fullPath">The full path of the file / directory.</param>
        /// <param name="basePath">The base path of the file / directory.</param>
        /// <returns>Returns the relative path.</returns>
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

            var chars = new[] { '\\', '/' };
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
                string.Join(Path.DirectorySeparatorChar.ToString(), relative, baseCheck.Length, relative.Length - baseCheck.Length);
        }

        /// <summary>
        ///     Creates a new directory instance from a specified base path and the full path to the directory. (The
        ///     subdirectories will be extracted.)
        /// </summary>
        /// <param name="baseDirectory">The base directory.</param>
        /// <param name="fullDirectory">The full path of the directory.</param>
        /// <returns>Returns a new <see cref="DirectoryItem" />.</returns>
        public static DirectoryItem FromFullPath(string baseDirectory, string fullDirectory) =>
            new DirectoryItem(baseDirectory, GetRelative(fullDirectory, baseDirectory));

        /// <inheritdoc />
        public override string ToString() => FullPath;

        /// <inheritdoc />
        public override int GetHashCode() => FullPath.GetHashCode();
    }
}
