using System;

namespace Cave.Progress;

/// <summary>
/// Implements the <see cref="IProgress"/> interface
/// </summary>
sealed class ProgressItem : IProgress
{
    #region Private Fields

    readonly object syncRoot = new();

    #endregion Private Fields

    #region Private Methods

    void OnUpdated() => Updated?.Invoke(this, new(this));

    #endregion Private Methods

    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="ProgressItem"/> class.</summary>
    /// <param name="source">The object that is associated with this progress item. Can be used to track the origin of the progress event.</param>
    /// <param name="identifier">The unique identifier for this progress item.</param>
    public ProgressItem(object source, int identifier)
    {
        Source = source;
        Identifier = identifier;
    }

    #endregion Public Constructors

    #region Public Events

    /// <inheritdoc/>
    public event EventHandler<ProgressEventArgs>? Updated;

    #endregion Public Events

    #region Public Properties

    /// <inheritdoc/>
    public bool Completed { get; private set; }

    /// <inheritdoc/>
    public DateTime Created { get; } = MonotonicTime.UtcNow;

    /// <summary>
    /// Gets or sets the estimation logic or result associated with this instance.
    /// </summary>
    public IEstimation? Estimation { get; set; }

    /// <inheritdoc/>
    public int Identifier { get; }

    /// <inheritdoc/>
    public object Source { get; }

    /// <inheritdoc/>
    public string Text { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public float Value { get; private set; }

    #endregion Public Properties

    #region Public Methods

    /// <inheritdoc/>
    public void Complete()
    {
        lock (syncRoot)
        {
            Value = 1;
            Completed = true;
        }
        OnUpdated();
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Value:P} : {Text}";

    /// <inheritdoc/>
    public void Update(float value, string? text = null)
    {
        lock (syncRoot)
        {
            if (Completed)
            {
                throw new InvalidOperationException();
            }

            if (value < Value)
            {
                return;
            }

            if (value > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (text != null)
            {
                Text = text;
            }

            Value = value;
        }
        OnUpdated();
    }

    #endregion Public Methods
}
