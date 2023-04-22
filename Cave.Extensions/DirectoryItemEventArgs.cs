using System;

namespace Cave;

/// <summary>Gets directory item event arguments.</summary>
/// <seealso cref="System.EventArgs" />
public sealed class DirectoryItemEventArgs : EventArgs
{
    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="DirectoryItemEventArgs" /> class.</summary>
    /// <param name="dir">The dir.</param>
    public DirectoryItemEventArgs(DirectoryItem dir) => Directory = dir;

    #endregion

    #region Properties

    /// <summary>Gets the directory.</summary>
    /// <value>The directory.</value>
    public DirectoryItem Directory { get; }

    /// <summary>Gets or sets a value indicating whether the file was handled or not.</summary>
    public bool Handled { get; set; }

    #endregion
}
