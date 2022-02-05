using System;

namespace Cave.Progress
{
    /// <summary>Provides an event for <see cref="IEstimation" /> classes to notify about actualizations.</summary>
    public sealed class EstimationUpdatedEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="EstimationUpdatedEventArgs" /> class.</summary>
        /// <param name="estimatedCompletionTime">Estimated completion time (absolute).</param>
        public EstimationUpdatedEventArgs(DateTime estimatedCompletionTime) => EstimatedCompletionTime = estimatedCompletionTime;

        #endregion

        #region Properties

        /// <summary>Gets the estimated completion time.</summary>
        public DateTime EstimatedCompletionTime { get; }

        #endregion
    }
}
