using System;
using System.IO;

namespace Cave;

#nullable enable

/// <summary>Provides extensions to the <see cref="RootLocation"/> enumeration.</summary>
public static class RootLocationExtension
{
    #region Public Methods

    /// <summary>Gets the full file location of the specified <paramref name="location"/> and relative components.</summary>
    /// <param name="location">Location to lookup</param>
    /// <param name="relativePathWithFileName">Relative sub folders and file name including extension.</param>
    /// <returns>Returns the full location</returns>
    public static FileLocation GetFileName(this RootLocation location, string? relativePathWithFileName) => relativePathWithFileName is null ? throw new ArgumentNullException(nameof(relativePathWithFileName)) :
        new FileLocation(location, subFolders: Path.GetDirectoryName(relativePathWithFileName), fileName: Path.GetFileNameWithoutExtension(relativePathWithFileName), extension: Path.GetExtension(relativePathWithFileName));

    /// <summary>Gets the full file location of the specified <paramref name="location"/> and relative components.</summary>
    /// <param name="location">Location to lookup</param>
    /// <param name="subFolders">Addisional sub folders (optional)</param>
    /// <param name="fileName">File name (optional)</param>
    /// <param name="extension">File extension (optional)</param>
    /// <returns>Returns the full location</returns>
    public static FileLocation GetFileName(this RootLocation location, string? subFolders = null, string? fileName = null, string? extension = null) => new(location, subFolders: subFolders, fileName: fileName, extension: extension);

    /// <summary>Gets the full location of the specified <paramref name="location"/>.</summary>
    /// <param name="location">Location to lookup</param>
    /// <returns>Returns the full location</returns>
    public static FileLocation GetFolder(this RootLocation location) => new(location, null, null, null);

    /// <summary>Gets the full location of the specified <paramref name="location"/>.</summary>
    /// <param name="location">Location to lookup</param>
    /// <param name="subFolders">Addisional sub folders (optional)</param>
    /// <returns>Returns the full location</returns>
    public static FileLocation GetFolder(this RootLocation location, params string[] subFolders) => new(location, subFolders.Join('/'), null, null);

    #endregion Public Methods
}
