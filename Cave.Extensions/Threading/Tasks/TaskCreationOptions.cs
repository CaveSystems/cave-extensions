#if NETSTANDARD10
#elif NET35 || NET20
#pragma warning disable CS1591, IDE0055, IDE0079, IDE0130

namespace System.Threading.Tasks
{
    /// <summary>
    /// Specifies flags that control optional behavior for the creation and execution of tasks.
    /// (Backport from net 4.0).
    /// </summary>
    [Flags]
    public enum TaskCreationOptions
    {
        /// <summary>
        /// Specifies that the default behavior should be used.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// A hint to a TaskScheduler to schedule a task in as fair a manner as possible, meaning that tasks scheduled sooner will be more likely to be run sooner,
        /// and tasks scheduled later will be more likely to be run later.
        /// </summary>
        PreferFairness = 0x1,

        /// <summary>
        /// Specifies that a task will be a long-running, coarse-grained operation involving fewer, larger components than fine-grained systems.
        /// It provides a hint to the TaskScheduler that oversubscription may be warranted.
        /// Oversubscription lets you create more threads than the available number of hardware threads.
        /// </summary>
        LongRunning = 0x2,
    }
}

#pragma warning restore CS1591, IDE0055, IDE0079, IDE0130
#endif
