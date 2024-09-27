using System;

namespace Cave.Progress;

/// <summary>Provides an item containing the creation <see cref="DateTime"/>.</summary>
public sealed class EstimationItem
{
    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="EstimationItem"/> class.</summary>
    /// <param name="progress">Current progress value (0..1).</param>
    public EstimationItem(float progress) => Progress = progress is >= 0 and <= 1 ? progress : throw new ArgumentOutOfRangeException(nameof(progress));

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the <see cref="DateTime"/> (utc) value this item was created.</summary>
    public DateTime DateTime { get; init; } = MonotonicTime.UtcNow;

    /// <summary>Gets the progress at the <see cref="DateTime"/> this item was created.</summary>
    public float Progress { get; init; }

    #endregion Public Properties
}
