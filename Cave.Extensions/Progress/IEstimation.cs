using System;

namespace Cave.Progress
{
    /// <summary>Provides an interface for calculating the estimated completion time of a process or task based on its progress.</summary>
    public interface IEstimation
    {
        #region Properties

        /// <summary>Gets the elapsed time.</summary>
        TimeSpan Elapsed { get; }

        /// <summary>Gets the estimated completion time of the process.</summary>
        DateTime EstimatedCompletionTime { get; }

        /// <summary>Gets the estimated time left based on the current time.</summary>
        TimeSpan EstimatedTimeLeft { get; }

        /// <summary>Gets the current progress of the process.</summary>
        float Progress { get; }

        /// <summary>Gets/sets the current progress of the process in percent.</summary>
        float ProgressPercent { get; }

        /// <summary>Gets the time the process was started.</summary>
        DateTime StartTime { get; }

        #endregion

        #region Members

        /// <summary>Resets the progress, sets <see cref="StartTime" /> to <see cref="DateTime.UtcNow" /> and begins a new estimation.</summary>
        void Reset();

        /// <summary>Actualizes <see cref="EstimatedCompletionTime" /> by setting the progress.</summary>
        /// <param name="progress">The progress in range [0.0 .. 1.1].</param>
        void Update(float progress);

        /// <summary>Actualizes <see cref="EstimatedCompletionTime" /> by setting the progress.</summary>
        /// <param name="currentValue">The current value of the progress.</param>
        /// <param name="maximum">The maximum value of the progress.</param>
        void Update(float currentValue, float maximum);

        /// <summary>Actualize the <see cref="EstimatedCompletionTime" /> by setting the progress.</summary>
        /// <param name="offset">The start value of the progress (0..1).</param>
        /// <param name="currentValue">The current value of the progress (0..max).</param>
        /// <param name="maximum">The maximum value of the progress.</param>
        void Update(float offset, float currentValue, float maximum);

        #endregion
    }
}
