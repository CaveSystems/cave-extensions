using System;

namespace Cave;

/// <summary>Gets information about the current state of a long running progress.</summary>
public class ProgressEventArgs : EventArgs
{
    #region Private Fields

    bool doBreak;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="ProgressEventArgs"/> class.</summary>
    /// <param name="userItem">The user item.</param>
    /// <param name="position">The position.</param>
    /// <param name="part">The part done between last callback and current.</param>
    /// <param name="count">The overall count.</param>
    /// <param name="canBreak">A value indicating whether breaking the current operation is possible or not.</param>
    public ProgressEventArgs(object? userItem, long position, int part, long count, bool canBreak)
    {
        CanBreak = canBreak;
        UserItem = userItem;
        Position = position;
        Count = count;
        Part = part;
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets or sets a value indicating whether the current process should break.</summary>
    public bool Break
    {
        get => doBreak;
        set
        {
            if (value && !CanBreak)
            {
                throw new InvalidOperationException("Break operation is not allowed!");
            }

            if (!value && doBreak)
            {
                throw new InvalidOperationException("Break flag has already been set!");
            }

            doBreak |= value;
        }
    }

    /// <summary>Gets a value indicating whether breaking the current operation is possible or not.</summary>
    public bool CanBreak { get; }

    /// <summary>Gets overall count.</summary>
    public long Count { get; }

    /// <summary>Gets part done between last callback and current.</summary>
    public int Part { get; }

    /// <summary>Gets current Position.</summary>
    public long Position { get; }

    /// <summary>Gets user item used at parent function.</summary>
    public object? UserItem { get; }

    #endregion Public Properties
}
