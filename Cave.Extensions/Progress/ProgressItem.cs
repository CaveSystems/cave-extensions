

using System;

namespace Cave.Progress;

sealed class ProgressItem : IProgress
{
    #region Private Fields

    readonly object syncRoot = new();

    #endregion Private Fields

    #region Private Methods

    void OnUpdated() => Updated?.Invoke(this, new(this));

    #endregion Private Methods

    #region Public Constructors

    public ProgressItem(object source, int identifier)
    {
        Source = source;
        Identifier = identifier;
    }

    #endregion Public Constructors

    #region Public Events

    public event EventHandler<ProgressEventArgs>? Updated;

    #endregion Public Events

    #region Public Properties

    public bool Completed { get; private set; }
    public DateTime Created { get; } = MonotonicTime.UtcNow;
    public IEstimation? Estimation { get; set; }

    public int Identifier { get; }

    public object Source { get; }

    public string Text { get; private set; } = string.Empty;

    public float Value { get; private set; }

    #endregion Public Properties

    #region Public Methods

    public void Complete()
    {
        lock (syncRoot)
        {
            Value = 1;
            Completed = true;
        }
        OnUpdated();
    }

    public override string ToString() => $"{Value:P} : {Text}";

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
