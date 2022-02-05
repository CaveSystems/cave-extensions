using System;
using System.Collections.Generic;
using System.Linq;

namespace Cave.Progress
{
    /// <summary>Provides a simple class for calculating the estimated completion time of a process based on its progress.</summary>
    public abstract class Estimation : IEstimation
    {
        /// <summary>Obtains the <see cref="EstimationItem" />s of this estimation.</summary>
        readonly List<EstimationItem> items = new List<EstimationItem>();

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Estimation" /> class.</summary>
        protected Estimation() { Reset(); }

        #endregion

        #region Properties

        /// <summary>Gets the <see cref="EstimationItem" /> at the specified index.</summary>
        /// <value>The <see cref="EstimationItem" />.</value>
        /// <param name="index">The index.</param>
        /// <returns>Returns the item at the specified index.</returns>
        public EstimationItem this[int index] => items[index];

        /// <summary>Gets the item count.</summary>
        /// <value>The item count.</value>
        public int ItemCount => items.Count;

        /// <summary>Gets the items.</summary>
        /// <value>The items.</value>
        public IList<EstimationItem> Items => items.ToList().AsReadOnly();

        #endregion

        #region Overrides

        /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString() =>
            $"{ProgressPercent:N2}% - {Elapsed.FormatTime()} elapsed - {EstimatedTimeLeft.FormatTime()} remaining...";

        #endregion

        #region Members

        #region events

        /// <summary>provides packet incoming events</summary>
        public event EventHandler<EstimationUpdatedEventArgs> Actualized;

        #endregion

        /// <summary>This function is called internally whenever the list of <see cref="EstimationItem" />s (<see cref="Items" />) is updated.</summary>
        /// <param name="estimatedCompletionTime">Estimated completion time (absolute).</param>
        /// <remarks>Overloaded versions of this function have to call the base function in order to generate the ActualizedEvent.</remarks>
        protected virtual void OnUpdated(DateTime estimatedCompletionTime)
        {
            Actualized?.Invoke(this, new EstimationUpdatedEventArgs(estimatedCompletionTime));
        }

        #endregion

        #region ITimeSpanEstimation Member

        /// <summary>Gets the time the process was started.</summary>
        public DateTime StartTime => items[0].DateTime;

        /// <summary>Resets the progress, sets the <see cref="StartTime" /> to <see cref="DateTime.UtcNow" /> and begins a new estimation.</summary>
        public void Reset()
        {
            items.Clear();
            items.Add(new EstimationItem(0.0f));
        }

        /// <summary>Gets the current progress of the process.</summary>
        public float Progress => items[items.Count - 1].Progress;

        /// <summary>Gets/sets the current progress of the process in percent.</summary>
        public float ProgressPercent => Progress * 100.0f;

        /// <summary>Actualize the <see cref="EstimatedCompletionTime" /> by setting the progress.</summary>
        /// <param name="progress">The progress in range [0.0 .. 1.0].</param>
        public void Update(float progress)
        {
            if ((progress < 0.0) || (progress > 1.0))
            {
                throw new ArgumentOutOfRangeException(nameof(progress));
            }

            // never add 0.0 to progress list
            if (progress == 0.0)
            {
                return;
            }

            items.Add(new EstimationItem(progress));
            if (items.Count > 1000)
            {
                items.RemoveRange(1, 100);
            }

            OnUpdated(EstimatedCompletionTime);
        }

        /// <summary>Actualize the <see cref="EstimatedCompletionTime" /> by setting the progress.</summary>
        /// <param name="currentValue">The current value of the progress.</param>
        /// <param name="maximum">The maximum value of the progress.</param>
        public void Update(float currentValue, float maximum) => Update(currentValue / maximum);

        /// <summary>Actualize the <see cref="EstimatedCompletionTime" /> by setting the progress.</summary>
        /// <param name="offset">The start value of the progress (0..1).</param>
        /// <param name="currentValue">The current value of the progress (0..max).</param>
        /// <param name="maximum">The maximum value of the progress.</param>
        public void Update(float offset, float currentValue, float maximum) => Update(offset + (currentValue / maximum));

        /// <summary>Gets the estimated completion time of the process.</summary>
        public abstract DateTime EstimatedCompletionTime { get; }

        /// <summary>Gets the estimated time left based on the current time.</summary>
        public abstract TimeSpan EstimatedTimeLeft { get; }

        /// <summary>Gets the elapsed time.</summary>
        public TimeSpan Elapsed => DateTime.UtcNow - items[0].DateTime;

        #endregion
    }
}
