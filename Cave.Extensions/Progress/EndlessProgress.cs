namespace Cave.Progress
{
    /// <summary>Provides an endless progress calculator (increasing slices all the time but never reaching 100%).</summary>
    public class EndlessProgress
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="EndlessProgress" /> class.</summary>
        /// <param name="estimatedMaximumCount">The estimated maximum count.</param>
        public EndlessProgress(long estimatedMaximumCount) => EstimatedMaxCount = estimatedMaximumCount;

        #endregion

        #region Properties

        /// <summary>Gets the count.</summary>
        /// <value>The count.</value>
        public long Count { get; private set; }

        /// <summary>Gets the estimated maximum count.</summary>
        /// <value>The estimated maximum count.</value>
        public long EstimatedMaxCount { get; private set; }

        /// <summary>Gets the value in range [0..1).</summary>
        /// <value>The value in range [0..1).</value>
        public float Value { get; private set; }

        #endregion

        #region Members

        /// <summary>Increments this instance.</summary>
        public void Increment()
        {
            var c = ++Count;
            if (c > EstimatedMaxCount)
            {
                EstimatedMaxCount = c + 1;
            }

            Value = c / (float)EstimatedMaxCount;
        }

        #endregion
    }
}
