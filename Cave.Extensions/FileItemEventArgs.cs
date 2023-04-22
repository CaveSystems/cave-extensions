using System;

namespace Cave;

/// <summary>Gets file item event arguments.</summary>
/// <seealso cref="System.EventArgs" />
public class FileItemEventArgs : EventArgs
{
    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="FileItemEventArgs" /> class.</summary>
    /// <param name="file">The file.</param>
    public FileItemEventArgs(FileItem file) => File = file;

    #endregion

    #region Properties

    /// <summary>Gets the file.</summary>
    /// <value>The file.</value>
    public FileItem File { get; }

    /// <summary>Gets or sets a value indicating whether the file was handled or not.</summary>
    public bool Handled { get; set; }

    #endregion
}
