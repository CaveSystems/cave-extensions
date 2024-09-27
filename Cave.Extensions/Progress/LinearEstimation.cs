

using System;

namespace Cave.Progress;

/// <summary>Provides a simple linear estimation.</summary>
public class LinearEstimation : Estimation
{
    #region Public Properties

    /// <inheritdoc/>
    public override DateTime EstimatedCompletionTime => Progress < 0.01f ? MonotonicTime.UtcNow.AddDays(1) : Started.AddTicks((long)((MonotonicTime.UtcNow - Started).Ticks / Progress));

    /// <inheritdoc/>
    public override TimeSpan EstimatedTimeLeft
    {
        get
        {
            if (Progress <= 0.01f) return TimeSpan.FromDays(1);
            var now = MonotonicTime.UtcNow;
            return Started.AddTicks((long)((now - Started).Ticks / Progress)) - now;
        }
    }

    #endregion Public Properties
}
