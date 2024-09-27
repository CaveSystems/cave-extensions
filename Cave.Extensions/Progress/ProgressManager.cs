

using System;
using System.Collections.Generic;

namespace Cave.Progress;

/// <summary>Provides progress management using callback events on progress change and completion.</summary>
public static class ProgressManager
{
    #region Private Fields

    static IProgressManager instance;

    #endregion Private Fields

    #region Private Methods

    static void OnUpdated(object? sender, ProgressEventArgs e)
    {
        lock (SyncRoot)
        {
            Updated?.Invoke(sender, e);
        }
    }

    /// <summary>Allows to change the globally used instance.</summary>
    /// <remarks>All events of <see cref="IProgress"/> objects will be routed to the new global instance.</remarks>
    /// <param name="newGlobalInstance">New global instance to use.</param>
    static void SetGlobalInstance(IProgressManager newGlobalInstance)
    {
        lock (SyncRoot)
        {
            if (newGlobalInstance == null)
            {
                throw new ArgumentNullException(nameof(newGlobalInstance));
            }

            if (instance == newGlobalInstance)
            {
                return;
            }

            if (instance != null)
            {
                instance.Updated -= OnUpdated;
            }

            newGlobalInstance.Updated += OnUpdated;
            instance = newGlobalInstance;
        }
    }

    #endregion Private Methods

    #region Public Constructors

    static ProgressManager()
    {
        instance = new DefaultProgressManager();
        instance.Updated += OnUpdated;
    }

    #endregion Public Constructors

    #region Public Events

    /// <summary>Provides an event for each progress update / completion</summary>
    public static event EventHandler<ProgressEventArgs>? Updated;

    #endregion Public Events

    #region Public Properties

    /// <summary>Gets the global static used instance.</summary>
    public static IProgressManager Instance { get => instance; set => SetGlobalInstance(value); }

    /// <summary>Gets the current progress items.</summary>
    public static IEnumerable<IProgress> Items => Instance.Items;

    /// <summary>Gets the global sync root.</summary>
    public static object SyncRoot { get; } = new();

    #endregion Public Properties

    #region Public Methods

    /// <summary>Creates a new progress object implementing the <see cref="IProgress"/> interface.</summary>
    /// <remarks>
    /// This function does not call the <see cref="Updated"/> event for the newly created <see cref="IProgress"/> instance. The <see cref="Updated"/> event will
    /// be fired upon the first <see cref="IProgress.Update(float, string)"/> call.
    /// </remarks>
    /// <param name="source">Source object for the progress.</param>
    /// <returns>Returns a new instance implementing the <see cref="IProgress"/> interface.</returns>
    public static IProgress CreateProgress(object source) => Instance.CreateProgress(source);

    #endregion Public Methods
}
