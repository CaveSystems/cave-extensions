

using System;

namespace Cave.Progress;

/// <summary>Provides an event for <see cref="IEstimation"/> classes to notify about actualizations.</summary>
public sealed class EstimationUpdatedEventArgs : EventArgs
{
    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="EstimationUpdatedEventArgs"/> class.</summary>
    /// <param name="estimatedCompletionTime">Estimated completion time (absolute).</param>
    /// <param name="currentProgress">Current progress value.</param>
    public EstimationUpdatedEventArgs(DateTime estimatedCompletionTime, float currentProgress)
    {
        EstimatedCompletionTime = estimatedCompletionTime;
        CurrentProgress = currentProgress;
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the current progress.</summary>
    public float CurrentProgress { get; }

    /// <summary>Gets the estimated completion time.</summary>
    public DateTime EstimatedCompletionTime { get; }

    #endregion Public Properties
}
