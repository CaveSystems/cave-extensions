using System;
using System.Collections.Generic;

namespace Cave.Progress;

/// <summary>Provides an interface for the <see cref="ProgressManager" /> and <see cref="ProgressManagerBase" /> implementations.</summary>
public interface IProgressManager
{
    #region Properties

    /// <summary>Gets the current progress items.</summary>
    IEnumerable<IProgress> Items { get; }

    #endregion

    #region Members

    /// <summary>Provides an event for each progress update / completion</summary>
    event EventHandler<ProgressEventArgs> Updated;

    /// <summary>Creates a new progress object implementing the <see cref="IProgress" /> interface.</summary>
    /// <remarks>
    /// This function does not call the <see cref="Updated" /> event for the newly created <see cref="IProgress" /> instance. The
    /// <see cref="Updated" /> event will be fired upon the first <see cref="IProgress.Update(float, string)" /> call.
    /// </remarks>
    /// <param name="source">Source object for the progress.</param>
    /// <returns>Retruns a new instance implementing the <see cref="IProgress" /> interface.</returns>
    IProgress CreateProgress(object source);

    #endregion
}
