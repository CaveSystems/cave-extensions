using System;

namespace Cave.Progress
{
    /// <summary>Provides the progress instance to use for events.</summary>
    public class ProgressEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="ProgressEventArgs" /> class.</summary>
        /// <param name="progress">Current progress value (0..1).</param>
        public ProgressEventArgs(IProgress progress) => Progress = progress;

        #endregion

        #region Properties

        /// <summary>Gets the progress object implementing the <see cref="IProgress" /> interface.</summary>
        public IProgress Progress { get; }

        #endregion
    }
}
