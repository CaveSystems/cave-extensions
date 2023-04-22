using System;

namespace Cave;

/// <summary>Gets access to a selected IStopWatch implementation for the current platform.</summary>
public static class StopWatch
{
    #region Static

    /// <summary>Checks the resolution.</summary>
    /// <param name="watch">The IStopWatch to check.</param>
    /// <param name="samples">The samples.</param>
    /// <returns>The resolution.</returns>
    /// <exception cref="ArgumentNullException">watch.</exception>
    public static TimeSpan CheckResolution(IStopWatch watch, int samples = 50)
    {
        if (watch == null)
        {
            throw new ArgumentNullException(nameof(watch));
        }

        if (!watch.IsRunning)
        {
            watch.Start();
        }

        var best = TimeSpan.MaxValue;
        for (var i = 0; i < samples; i++)
        {
            var start = watch.Elapsed;
            var next = watch.Elapsed;
            while (next == start)
            {
                next = watch.Elapsed;
            }

            var res = next - start;
            if (res < best)
            {
                best = res;
            }
        }

        return best;
    }

    /// <summary>Sets the type.</summary>
    /// <param name="type">The type.</param>
    public static void SetType(Type type)
    {
        var watch = (IStopWatch)Activator.CreateInstance(type);
        watch.Start();
        SelectedType = type;
    }

    /// <summary>Gets a new started IStopWatch object.</summary>
    /// <returns>The started stopwatch.</returns>
    public static IStopWatch StartNew()
    {
        var watch = (IStopWatch)Activator.CreateInstance(SelectedType);
        watch.Start();
        return watch;
    }

    /// <summary>Gets the type of the selected stop watch.</summary>
    /// <value>The type of the selected.</value>
    public static Type SelectedType { get; private set; } = typeof(DateTimeStopWatch);

    #endregion
}
