using System;

namespace Cave
{
    /// <summary>Gets an interface for simple time measurement.</summary>
    public interface IStopWatch
    {
        /// <summary>Gets the elapsed time.</summary>
        TimeSpan Elapsed { get; }

        /// <summary>Gets the elapsed time in milli seconds.</summary>
        long ElapsedMilliSeconds { get; }

        /// <summary>Gets the elapsed time in seconds.</summary>
        double ElapsedSeconds { get; }

        /// <summary>Gets a value indicating whether the StopWatch is running or not.</summary>
        bool IsRunning { get; }

        /// <summary>Gets the resolution of the <see cref="IStopWatch" />.</summary>
        TimeSpan Resolution { get; }

        /// <summary>Gets the frequency of the <see cref="IStopWatch" /> in HZ.</summary>
        long Frequency { get; }

        /// <summary>Gets the <see cref="DateTime" /> (utc) value at start of the <see cref="IStopWatch" />.</summary>
        DateTime StartDateTime { get; }

        /// <summary>Starts the StopWatch (to restart a StopWatch use <see cref="Reset" /> first!).</summary>
        void Start();

        /// <summary>Resets the StopWatch (can be used even if the StopWatch is running).</summary>
        void Reset();

        /// <summary>Stops the StopWatch.</summary>
        void Stop();

        /// <summary>Waits until the specified <see cref="Elapsed" /> time is reached.</summary>
        /// <param name="elapsed">The elapsed timespan.</param>
        void Wait(TimeSpan elapsed);
    }
}
