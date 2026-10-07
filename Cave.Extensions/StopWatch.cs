using System;

namespace Cave;

/// <summary>Gets access to a selected IStopWatch implementation for the current platform.</summary>
public static class StopWatch
{
    #region Public Properties

    /// <summary>Gets the type of the selected stop watch.</summary>
    /// <value>The type of the selected.</value>
    public static Type SelectedType => selectedWatch.GetType();

    static IStopWatch selectedWatch = new DateTimeStopWatch();

    #endregion Public Properties

    #region Public Methods

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
        if (Activator.CreateInstance(type) is not IStopWatch watch) throw new InvalidCastException($"Could not create {nameof(IStopWatch)} with {type}!");
        selectedWatch = watch;
    }

    /// <summary>Sets the type.</summary>
    /// <typeparam name="T">The type.</typeparam>
    public static void SetType<T>() where T : IStopWatch, new()
    {
        selectedWatch = new T();
    }

    /// <summary>Gets a new started IStopWatch object.</summary>
    /// <returns>The started stopwatch.</returns>
    public static IStopWatch StartNew()
    {
        var watch = selectedWatch.CreateNew();
        watch.Start();
        return watch;
    }

    #endregion Public Methods
}
