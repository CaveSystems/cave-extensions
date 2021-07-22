#pragma warning disable IDE0055 // we will not document back ports
#if NET20 || NET35 || NETSTANDARD10
using System.Collections.Generic;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Gets backports of the Parallel class in net &gt; 4.
    /// </summary>
    public static class Parallel
    {
        #region static class

        /// <summary>
        /// Executes a for loop in which iterations may run in parallel.
        /// </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="action">The delegate that is invoked once per iteration.</param>
        public static void For(int fromInclusive, int toExclusive, Action<int> action)
        {
            using var instance = new Runner<int>();
            instance.Action = action ?? throw new ArgumentNullException(nameof(action));
            for (var i = fromInclusive; i < toExclusive; i++)
            {
                instance.Start(i);
                if (instance.LoopState.StopByAnySource) return;
            }
            instance.Wait();
        }

        /// <summary>
        /// Executes a for loop in which iterations may run in parallel.
        /// </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="action">The delegate that is invoked once per iteration.</param>
        public static void For(int fromInclusive, int toExclusive, Action<int, ParallelLoopState> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            using var instance = new Runner<int>();
            instance.Action = (item) => action(item, instance.LoopState);
            for (var i = fromInclusive; i < toExclusive; i++)
            {
                instance.Start(i);
                if (instance.LoopState.StopByAnySource) return;
            }
            instance.Wait();
        }

        /// <summary>Executes a foreach operation.</summary>
        /// <typeparam name="T">The type of elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="concurrentTasks">The concurrent tasks.</param>
        /// <param name="items">The items.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(int concurrentTasks, IEnumerable<T> items, Action<T> action)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            using var instance = new Runner<T>();
            instance.ConcurrentTasks = concurrentTasks;
            instance.Action = action ?? throw new ArgumentNullException(nameof(action));
            foreach (var item in items)
            {
                instance.Start(item);
                if (instance.LoopState.StopByAnySource) return;
            }
            instance.Wait();
        }

        /// <summary>Executes a foreach operation in which up to <see cref="Environment.ProcessorCount"/> * 4 iterations may run in parallel.</summary>
        /// <typeparam name="T">The type of elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(IEnumerable<T> items, Action<T> action)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            using var instance = new Runner<T>();
            instance.Action = action ?? throw new ArgumentNullException(nameof(action));
            foreach (var item in items)
            {
                instance.Start(item);
                if (instance.LoopState.StopByAnySource) return;
            }
            instance.Wait();
        }

        /// <summary>
        /// Executes a foreach operation on an IEnumerable in which iterations may run in parallel, loop options can be configured, and the state of the loop can be monitored and manipulated.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the source.</typeparam>
        /// <param name="items">An enumerable data source.</param>
        /// <param name="action">The delegate that is invoked once per iteration.</param>
        public static void ForEach<TSource>(IEnumerable<TSource> items, Action<TSource, ParallelLoopState> action)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (action == null) throw new ArgumentNullException(nameof(action));
            using var instance = new Runner<TSource>();
            instance.Action = (item) => action(item, instance.LoopState);
            foreach (var item in items)
            {
                instance.Start(item);
                if (instance.LoopState.StopByAnySource) return;
            }
            instance.Wait();
        }

        #endregion

        class Runner<T> : IDisposable
        {
            public int ConcurrentTasks { get; set; } = Environment.ProcessorCount << 2;

            public Action<T> Action { get; set; }

            readonly List<Exception> exceptions = new();

            readonly AutoResetEvent completed = new(false);
            int currentTasks;

            void Run(object item)
            {
                try
                {
                    Action((T)item);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    LoopState.SetException();
                    throw;
                }
                finally
                {
                    Interlocked.Decrement(ref currentTasks);
                    completed.Set();
                }
            }

            internal void Start(T item)
            {
                Interlocked.Increment(ref currentTasks);
                while (currentTasks >= ConcurrentTasks)
                {
                    if (LoopState.StopByAnySource) return;
                    completed.WaitOne(1);
                }

                if (exceptions.Count > 0)
                {
                    throw new AggregateException(exceptions.ToArray());
                }
#if NETSTANDARD10
                Task.Factory.StartNew(Run, item, TaskCreationOptions.None);
#else
                ThreadPool.QueueUserWorkItem(Run, item);
#endif
            }

            public void Wait()
            {
                while (currentTasks > 0)
                {
                    completed.WaitOne();
                }
            }

            public void Dispose() => (completed as IDisposable)?.Dispose();

            public ParallelLoopState LoopState { get; } = new ParallelLoopState();
        }
    }
}

#endif
#pragma warning restore IDE0055
