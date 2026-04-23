namespace Cave;

/// <summary>Gets a callback to deliver information about the state of a long running progress.</summary>
/// <param name="sender">The source of the progress event.</param>
/// <param name="e">A <see cref="ProgressEventArgs"/> that contains the event data.</param>
public delegate void ProgressCallback(object sender, ProgressEventArgs e);
