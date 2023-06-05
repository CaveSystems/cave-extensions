#nullable enable

using System;

namespace Cave.Progress;

/// <summary>
/// Provides a simple linear estimation.
/// </summary>
public class LinearEstimation : Estimation
{
    /// <inheritdoc/>
    public override DateTime EstimatedCompletionTime => MonotonicTime.UtcNow + EstimatedTimeLeft;

    /// <inheritdoc/>
    public override TimeSpan EstimatedTimeLeft => new TimeSpan((long)((MonotonicTime.UtcNow - Started).Ticks * (1 - Progress)));
}
