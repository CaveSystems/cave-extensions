#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cave.Progress;

/// <summary>Provides a simple class for calculating the estimated completion time of a process based on its progress.</summary>
public abstract class Estimation : IEstimation
{
    #region Fields

    /// <summary>Obtains the <see cref="EstimationItem" />s of this estimation.</summary>
    readonly List<EstimationItem> items = [];

    #endregion

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="Estimation" /> class.</summary>
    protected Estimation() => Reset();

    #endregion

    #region Properties

    /// <summary>Gets a value indicating whether this istance is synchronized or not.</summary>
    public virtual bool IsSynchronized => true;

    /// <summary>Gets the <see cref="EstimationItem" /> at the specified index.</summary>
    /// <value>The <see cref="EstimationItem" />.</value>
    /// <param name="index">The index.</param>
    /// <returns>Returns the item at the specified index.</returns>
    public EstimationItem this[int index]
    {
        get
        {
            lock (SyncRoot)
            {
                return items[index];
            }
        }
    }

    /// <summary>Gets the item count.</summary>
    /// <value>The item count.</value>
    public int ItemCount
    {
        get
        {
            lock (SyncRoot)
            {
                return items.Count;
            }
        }
    }

    /// <summary>Gets the items.</summary>
    /// <value>The items.</value>
    public IList<EstimationItem> Items
    {
        get
        {
            lock (SyncRoot)
            {
                return items.ToList().AsReadOnly();
            }
        }
    }

    /// <summary>Gets the synchronization root.</summary>
    public object SyncRoot { get; } = new();

    /// <summary>Gets or sets the maximum items to keep at the list.</summary>
    public int MaximumItems { get; set; }

    #endregion

    #region IEstimation Members

    /// <summary>Gets the elapsed time.</summary>
    public TimeSpan Elapsed => MonotonicTime.UtcNow - Started;

    /// <summary>Gets the estimated completion time of the process.</summary>
    public abstract DateTime EstimatedCompletionTime { get; }

    /// <summary>Gets the estimated time left based on the current time.</summary>
    public abstract TimeSpan EstimatedTimeLeft { get; }

    /// <summary>Gets the current progress of the process.</summary>
    public float Progress
    {
        get
        {
            lock (SyncRoot)
            {
                return items[^1].Progress;
            }
        }
    }

    /// <summary>Gets/sets the current progress of the process in percent.</summary>
    public float ProgressPercent => Progress * 100.0f;

    /// <summary>Resets the progress, sets the <see cref="Started" /> to <see cref="DateTime.UtcNow" /> and begins a new estimation.</summary>
    public void Reset()
    {
        lock (SyncRoot)
        {
            items.Clear();
            items.Add(new(0.0f));
        }
    }

    /// <summary>Gets the date and time the progress was started.</summary>
    public DateTime Started
    {
        get
        {
            lock (SyncRoot)
            {
                return items[0].DateTime;
            }
        }
    }

    /// <summary>Actualize the <see cref="EstimatedCompletionTime" /> by setting the progress.</summary>
    /// <param name="progress">The progress in range [0.0 .. 1.0].</param>
    public void Update(float progress)
    {
        // never add 0.0 to progress list
        if (progress <= 0.0f)
        {
            return;
        }
        AddItem(new(progress));
    }

    /// <summary>Updates the <see cref="EstimatedCompletionTime" /> by setting the progress.</summary>
    /// <param name="currentValue">The current value of the progress.</param>
    /// <param name="maximum">The maximum value of the progress.</param>
    public void Update(float currentValue, float maximum) => Update(currentValue / maximum);

    /// <summary>Updates the <see cref="EstimatedCompletionTime" /> by setting the progress.</summary>
    /// <param name="offset">The start value of the progress (0..1).</param>
    /// <param name="currentValue">The current value of the progress (0..max).</param>
    /// <param name="maximum">The maximum value of the progress.</param>
    public void Update(float offset, float currentValue, float maximum) => Update(offset + (currentValue / maximum));

    #endregion

    #region Overrides

    /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString() => $"{ProgressPercent:N2}% - {Elapsed.FormatTime()} elapsed - {EstimatedTimeLeft.FormatTime()} remaining...";

    #endregion

    #region Members

    /// <summary>Provides the estimation updated event.</summary>
    public event EventHandler<EstimationUpdatedEventArgs>? Updated;

    /// <summary>Updates the <see cref="EstimatedCompletionTime" /> by setting the progress.</summary>
    public void Update(IProgress progress)
    {
        var percent = progress.Value;
        // never add 0.0 to progress list
        if (percent < 0.0f)
        {
            return;
        }

        AddItem(new(percent) { DateTime = progress.Created });
    }

    /// <summary>This function is called internally whenever the list of <see cref="EstimationItem" />s (<see cref="Items" />) is updated.</summary>
    /// <param name="estimatedCompletionTime">Estimated completion time (absolute).</param>
    /// <param name="currentProgress">Current progress value.</param>
    /// <remarks>Overloaded versions of this function have to call the base function in order to generate the ActualizedEvent.</remarks>
    protected virtual void OnUpdated(DateTime estimatedCompletionTime, float currentProgress) => Updated?.Invoke(this, new(estimatedCompletionTime, currentProgress));

    void AddItem(EstimationItem item)
    {
        lock (SyncRoot)
        {
            if (item.Progress <= items[^1].Progress)
            {
                return;
            }
            items.Add(item);
            while (MaximumItems > 0 && items.Count > MaximumItems)
            {
                RemoveOneProgressItem();
            }
        }
        OnUpdated(EstimatedCompletionTime, item.Progress);
    }

    void RemoveOneProgressItem()
    {
        var bestDistance = 100f;
        var bestIndex = -1;
        for (var i = 0; i < (items.Count - 1); i++)
        {
            var distance = items[i + 1].Progress - items[i].Progress;
            if (distance < bestDistance)
            {
                bestIndex = i;
                bestDistance = distance;
            }
        }
        items.RemoveAt(bestIndex);
    }

    #endregion
}
