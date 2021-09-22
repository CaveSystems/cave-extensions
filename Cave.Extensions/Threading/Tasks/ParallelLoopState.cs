#if NET20 || NET35 || NETSTANDARD10
#pragma warning disable CS1591, IDE0055, IDE0079, IDE0130

using System.Diagnostics;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Backport of the net 4.0 class.
    /// </summary>
    [DebuggerDisplay("ShouldExitCurrentIteration = {ShouldExitCurrentIteration}")]
    public class ParallelLoopState
    {
        /// <summary>
        /// Gets whether the current iteration of the loop should exit based on requests made by this or other iterations.
        /// </summary>
        /// <remarks>
        /// When an iteration of a loop calls <see cref="Break()"/> or <see cref="Stop()"/>, or when one throws an exception, or when the loop is canceled, the
        /// <see cref="Parallel"/> class will proactively attempt to prohibit additional iterations of the loop from starting execution. However, there may be
        /// cases where it is unable to prevent additional iterations from starting. It may also be the case that a long-running iteration has already begun
        /// execution. In such cases, iterations may explicitly check the <see cref="ShouldExitCurrentIteration"/> property and cease execution if the property
        /// returns true.
        /// </remarks>
        public bool ShouldExitCurrentIteration { get; private set; }

        /// <summary>
        /// Gets whether any iteration of the loop has called <see cref="Stop()"/>.
        /// </summary>
        public bool IsStopped { get; private set; }

        /// <summary>
        /// Gets whether any iteration of the loop has thrown an exception that went unhandled by that iteration.
        /// </summary>
        public bool IsExceptional { get; private set; }

        /// <summary>
        /// Communicates that the <see cref="Parallel"/> loop should cease execution at the system's earliest convenience.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Break()"/> method was previously called. <see cref="Break()"/> and <see cref="Stop()"/> may not be used in combination by iterations
        /// of the same loop.
        /// </exception>
        /// <remarks>
        /// <para>
        /// <see cref="Stop()"/> may be used to communicate to the loop that no other iterations need be run. For long-running iterations that may already be
        /// executing, <see cref="Stop()"/> causes <see cref="IsStopped"/> to return true for all other iterations of the loop, such that another iteration may
        /// check <see cref="IsStopped"/> and exit early if it's observed to be true.
        /// </para>
        /// <para><see cref="Stop()"/> is typically employed in search-based algorithms, where once a result is found, no other iterations need be executed.</para>
        /// </remarks>
        public void Stop() => IsStopped = true;

        /// <summary>
        /// Communicates that the <see cref="Parallel"/> loop should cease execution at the system's earliest convenience of iterations beyond the current iteration.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Stop()"/> method was previously called. <see cref="Break()"/> and <see cref="Stop()"/> may not be used in combination by iterations
        /// of the same loop.
        /// </exception>
        /// <remarks>
        /// <para>
        /// <see cref="Break()"/> may be used to communicate to the loop that no other iterations after the current iteration need be run. For example, if <see
        /// cref="Break()"/> is called from the 100th iteration of a for loop iterating in parallel from 0 to 1000, all iterations less than 100 should still be
        /// run, but the iterations from 101 through to 1000 are not necessary.
        /// </para>
        /// <para><see cref="Break()"/> is typically employed in search-based algorithms where an ordering is present in the data source.</para>
        /// </remarks>
        public void Break() => ShouldExitCurrentIteration = true;

        internal void SetException() => IsExceptional = true;
        internal bool StopByAnySource => IsExceptional || IsStopped || ShouldExitCurrentIteration;
    }
}

#pragma warning restore CS1591, IDE0055, IDE0079, IDE0130

#endif
