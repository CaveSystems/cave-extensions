

using System;

namespace Cave.Progress;

/// <summary>Provides an interface for progress tracking.</summary>
public interface IProgress
{
    #region Public Events

    /// <summary>Provides an event to be called on progress changes</summary>
    event EventHandler<ProgressEventArgs>? Updated;

    #endregion Public Events

    #region Public Properties

    /// <summary>Gets a value indicating whether the progress is completed or not.</summary>
    bool Completed { get; }

    /// <summary>Gets the date and time the progress was created.</summary>
    DateTime Created { get; }

    /// <summary>Gets the identifier (creation number) of this progress instance.</summary>
    int Identifier { get; }

    /// <summary>Gets the source object of the progress.</summary>
    object Source { get; }

    /// <summary>Gets the current text.</summary>
    string Text { get; }

    /// <summary>Gets the current value (0..1).</summary>
    float Value { get; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Completes the progress.</summary>
    /// <remarks>
    /// This can only be called once and sets the <see cref="Value"/> property to 1.0f and <see cref="Completed"/> property to true. After the properties are
    /// set <see cref="ProgressManager.Updated"/> will be invoked for the falst time with this progress instance. It is then removed from the <see
    /// cref="ProgressManager.Items"/> enumeration.
    /// </remarks>
    void Complete();

    /// <summary>Updates the progress to a higher value (0..1) and optionally sets a new text.</summary>
    /// <param name="value">The new value (higher or equal to the current <see cref="Value"/>).</param>
    /// <param name="message">The progress text (optional). If unset the text will not be changed.</param>
    void Update(float value, string? message = null);

    #endregion Public Methods
}
