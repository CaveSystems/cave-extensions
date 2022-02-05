using System;

namespace Cave.Progress
{
    /// <summary>Provides an event for <see cref="IEstimation" /> classes to notify about actualization.</summary>
    public sealed class EstimationActualizedEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="EstimationActualizedEventArgs" /> class.</summary>
        /// <param name="estimatedEndTime">Estimated completion time.</param>
        public EstimationActualizedEventArgs(DateTime estimatedEndTime) => EstimatedEndTime = estimatedEndTime;

        #endregion

        #region Properties

        /// <summary>Gets the estimated end time.</summary>
        public DateTime EstimatedEndTime { get; }

        #endregion
    }
}
