#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20 || NET35 || NETSTANDARD10

using System.Collections.Generic;

namespace System.Threading.Tasks
{
    public static class Parallel
    {
        #region Nested type: Runner

        sealed class Runner<T> : IDisposable
        {
            #region Fields

            readonly AutoResetEvent completed = new(false);

            readonly List<Exception> exceptions = [];
            int currentTasks;

            #endregion Fields

            #region Properties

            public ParallelLoopState LoopState { get; } = new();

            public Action<T> Action { get; set; }

            public int ConcurrentTasks { get; set; } = -1;

            #endregion Properties

            #region IDisposable Members

            public void Dispose() => (completed as IDisposable)?.Dispose();

            #endregion IDisposable Members

            #region Members

            public void Wait()
            {
                while (currentTasks > 0)
                {
                    completed.WaitOne();
                }
            }

            internal void Start(T item)
            {
                Interlocked.Increment(ref currentTasks);
                while (ConcurrentTasks > 0 && currentTasks >= ConcurrentTasks)
                {
                    if (LoopState.StopByAnySource)
                    {
                        return;
                    }
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

            #endregion Members
        }

        #endregion Nested type: Runner

        #region Static

        public static void For(int fromInclusive, int toExclusive, Action<int> action)
        {
            using var instance = new Runner<int>();
            instance.Action = action ?? throw new ArgumentNullException(nameof(action));
            for (var i = fromInclusive; i < toExclusive; i++)
            {
                instance.Start(i);
                if (instance.LoopState.StopByAnySource)
                {
                    return;
                }
            }
            instance.Wait();
        }

        public static void For(int fromInclusive, int toExclusive, Action<int, ParallelLoopState> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            using var instance = new Runner<int>();
            instance.Action = item => action(item, instance.LoopState);
            for (var i = fromInclusive; i < toExclusive; i++)
            {
                instance.Start(i);
                if (instance.LoopState.StopByAnySource)
                {
                    return;
                }
            }
            instance.Wait();
        }

        public static void ForEach<T>(int concurrentTasks, IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            using var instance = new Runner<T>();
            instance.ConcurrentTasks = concurrentTasks;
            instance.Action = action ?? throw new ArgumentNullException(nameof(action));
            foreach (var item in items)
            {
                instance.Start(item);
                if (instance.LoopState.StopByAnySource)
                {
                    return;
                }
            }
            instance.Wait();
        }

        public static void ForEach<T>(IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            using var instance = new Runner<T>();
            instance.Action = action ?? throw new ArgumentNullException(nameof(action));
            foreach (var item in items)
            {
                instance.Start(item);
                if (instance.LoopState.StopByAnySource)
                {
                    return;
                }
            }
            instance.Wait();
        }

        public static void ForEach<TSource>(IEnumerable<TSource> items, Action<TSource, ParallelLoopState> action)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            using var instance = new Runner<TSource>();
            instance.Action = item => action(item, instance.LoopState);
            foreach (var item in items)
            {
                instance.Start(item);
                if (instance.LoopState.StopByAnySource)
                {
                    return;
                }
            }
            instance.Wait();
        }

        #endregion Static
    }
}

#endif
