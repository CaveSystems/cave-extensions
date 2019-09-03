using System;

namespace Cave
{
    /// <summary>
    /// Provides access to a selected IStopWatch implementation for the current platform.
    /// </summary>
    public static class StopWatch
    {
        /// <summary>Gets the type of the selected stop watch.</summary>
        /// <value>The type of the selected.</value>
        public static Type SelectedType { get; private set; } = typeof(DateTimeStopWatch);

        /// <summary>Sets the type.</summary>
        /// <param name="type">The type.</param>
        public static void SetType(Type type)
        {
            var watch = (IStopWatch)Activator.CreateInstance(type);
            watch.Start();
            SelectedType = type;
        }

        /// <summary>
        /// Gets a new started IStopWatch object.
        /// </summary>
        /// <returns>The started stopwatch.</returns>
        public static IStopWatch StartNew()
        {
            var watch = (IStopWatch)Activator.CreateInstance(SelectedType);
            watch.Start();
            return watch;
        }

        /// <summary>Checks the resolution.</summary>
        /// <param name="watch">The IStopWatch to check.</param>
        /// <param name="samples">The samples.</param>
        /// <returns>The resolution.</returns>
        /// <exception cref="ArgumentNullException">watch.</exception>
        public static TimeSpan CheckResolution(IStopWatch watch, int samples = 50)
        {
            if (watch == null)
            {
                throw new ArgumentNullException("watch");
            }

            if (!watch.IsRunning)
            {
                watch.Start();
            }

            TimeSpan best = TimeSpan.MaxValue;
            for (var i = 0; i < samples; i++)
            {
                TimeSpan start = watch.Elapsed;
                TimeSpan next = watch.Elapsed;
                while (next == start)
                {
                    next = watch.Elapsed;
                }

                TimeSpan res = next - start;
                if (res < best)
                {
                    best = res;
                }
            }
            return best;
        }
    }
}