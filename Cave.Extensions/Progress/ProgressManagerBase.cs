using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cave.Collections.Generic;

namespace Cave.Progress;

/// <summary>Provides progress management using callback events on progress change and completion.</summary>
public abstract class ProgressManagerBase : IProgressManager
{
    #region Private Fields

    readonly Set<IProgress> items = new();
    int nextIdentifier;

    #endregion Private Fields

    #region Private Methods

    void ItemUpdated(object? sender, ProgressEventArgs e)
    {
        Updated?.Invoke(this, e);
        if (e.Progress.Completed)
        {
            e.Progress.Updated -= ItemUpdated;
            lock (items)
            {
                _ = items.TryRemove(e.Progress);
            }
        }
    }

    #endregion Private Methods

    #region Public Events

    /// <summary>Provides an event for each progress update / completion</summary>
    public event EventHandler<ProgressEventArgs>? Updated;

    #endregion Public Events

    #region Public Properties

    /// <summary>Gets the current progress items.</summary>
    public IEnumerable<IProgress> Items
    {
        get
        {
            lock (items)
            {
                return items.ToList();
            }
        }
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Creates a new progress object implementing the <see cref="IProgress"/> interface.</summary>
    /// <remarks>
    /// This function does not call the <see cref="Updated"/> event for the newly created <see cref="IProgress"/> instance. The <see cref="Updated"/> event will
    /// be fired upon the first <see cref="IProgress.Update(float, string)"/> call.
    /// </remarks>
    /// <returns>Retruns a new instance implementing the <see cref="IProgress"/> interface.</returns>
    public IProgress CreateProgress(object source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        var result = new ProgressItem(source, Interlocked.Increment(ref nextIdentifier));
        result.Updated += ItemUpdated;
        lock (items)
        {
            items.Add(result);
        }

        return result;
    }

    #endregion Public Methods
}
