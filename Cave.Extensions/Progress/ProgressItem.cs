#nullable enable

using System;

namespace Cave.Progress;

sealed class ProgressItem : IProgress
{
    #region Fields

    readonly object syncRoot = new();

    #endregion

    #region Constructors

    public ProgressItem(object source, int identifier)
    {
        Source = source;
        Identifier = identifier;
    }

    #endregion

    #region Properties

    public IEstimation? Estimation { get; set; }

    #endregion

    #region IProgress Members

    public void Complete()
    {
        lock (syncRoot)
        {
            Value = 1;
            Completed = true;
        }
        OnUpdated();
    }

    public bool Completed { get; private set; }
    public DateTime Created { get; } = MonotonicTime.UtcNow;
    public int Identifier { get; }
    public object Source { get; }
    public string Text { get; private set; } = string.Empty;

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

    public event EventHandler<ProgressEventArgs>? Updated;

    public float Value { get; private set; }

    #endregion

    #region Overrides

    public override string ToString() => $"{Value:P} : {Text}";

    #endregion

    #region Members

    void OnUpdated() => Updated?.Invoke(this, new(this));

    #endregion
}
