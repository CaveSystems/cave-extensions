#pragma warning disable IDE0055 // we will not document back ports
#if NETSTANDARD10
#elif NET35 || NET20

namespace System.Threading.Tasks
{
    /// <summary>
    /// Gets a basic set tasking functions backported from net 4.0 using the <see cref="Task.Factory.StartNew(Action, TaskCreationOptions)"/> function.
    /// </summary>
    public class Task : IDisposable
    {
        /// <summary>
        /// Waits for all of the provided Task objects to complete execution.
        /// </summary>
        /// <param name="tasks">Task instances on which to wait.</param>
        public static void WaitAll(params Task[] tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            foreach (var task in tasks)
            {
                task.Wait();
            }
        }

        /// <summary>
        /// Waits for all of the provided Task objects to complete execution.
        /// </summary>
        /// <param name="tasks">Task instances on which to wait.</param>
        /// <param name="timeoutMillis">milliseconds to wait.</param>
        /// <returns>True if all tasks have completed in the given time.</returns>
        public static bool WaitAll(Task[] tasks, int timeoutMillis)
        {
            if (timeoutMillis < 0)
            {
                WaitAll(tasks);
                return true;
            }

            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            var timeout = DateTime.UtcNow + new TimeSpan(TimeSpan.TicksPerMillisecond * timeoutMillis);
            foreach (var task in tasks)
            {
                var wait = timeout - DateTime.UtcNow;
                if (wait < TimeSpan.Zero && !task.IsCompleted)
                {
                    return false;
                }

                if (!task.Wait((int)(wait.Ticks / TimeSpan.TicksPerMillisecond)))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Waits for any of the provided Task objects to complete execution.
        /// </summary>
        /// <param name="tasks">Task instances on which to wait.</param>
        /// <returns>The index of the completed Task object in the tasks array.</returns>
        public static int WaitAny(Task[] tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            while (true)
            {
                for (var i = 0; i < tasks.Length; i++)
                {
                    if (tasks[i].IsCompleted)
                    {
                        return i;
                    }
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Waits for any of the provided Task objects to complete execution.
        /// </summary>
        /// <param name="tasks">Task instances on which to wait.</param>
        /// <param name="timeoutMillis">milliseconds to wait.</param>
        /// <returns>The index of the completed Task object in the tasks array.</returns>
        public static int WaitAny(Task[] tasks, int timeoutMillis)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            var timeout = DateTime.UtcNow + new TimeSpan(timeoutMillis * TimeSpan.TicksPerMillisecond);
            while (DateTime.UtcNow <= timeout)
            {
                for (var i = 0; i < tasks.Length; i++)
                {
                    if (tasks[i].IsCompleted)
                    {
                        return i;
                    }
                }
                Thread.Sleep(1);
            }
            return -1;
        }

        #region Task.Factory class

        /// <summary>
        /// Gets a simple task starting mechanism backported from net 4.0 using the <see cref="Task.Factory.StartNew(Action, TaskCreationOptions)"/> function.
        /// </summary>
        public static class Factory
        {
            static Factory()
            {
                ThreadPool.SetMaxThreads(1000, 1000);
                ThreadPool.SetMinThreads(100, 100);
            }

            /// <summary>
            /// Creates and starts a task.
            /// </summary>
            /// <param name="action">The action delegate to execute asynchronously.</param>
            /// <param name="options">LongRunning spawns a new seperate Thread.</param>
            /// <returns>Returns a new <see cref="Task"/> instance.</returns>
            public static Task StartNew(Action action, TaskCreationOptions options = TaskCreationOptions.None)
            {
                var task = new Task(options, action, null);
                ThreadPool.QueueUserWorkItem(task.Worker, action);
                return task;
            }

            /// <summary>
            /// Creates and starts a task.
            /// </summary>
            /// <param name="action">The action delegate to execute asynchronously.</param>
            /// <param name="state">An object containing data to be used by the action delegate.</param>
            /// <param name="options">LongRunning spawns a new seperate Thread.</param>
            /// <returns>Returns a new <see cref="Task"/> instance.</returns>
            public static Task StartNew(Action<object> action, object state, TaskCreationOptions options = TaskCreationOptions.None)
            {
                var task = new Task(options, action, state);
                ThreadPool.QueueUserWorkItem(task.Worker, action);
                return task;
            }

            /// <summary>
            /// Creates and starts a task.
            /// </summary>
            /// <typeparam name="T">Type of the action delegate.</typeparam>
            /// <param name="action">The action delegate to execute asynchronously.</param>
            /// <param name="state">An object containing data to be used by the action delegate.</param>
            /// <param name="options">LongRunning spawns a new seperate Thread.</param>
            /// <returns>Returns a new <see cref="Task"/> instance.</returns>
            public static Task StartNew<T>(Action<T> action, T state, TaskCreationOptions options = TaskCreationOptions.None) => StartNew((object o) => action((T)o), state, options);
        }

        #endregion

        #region private functionality
        readonly object state;
        readonly object action;
        readonly TaskCreationOptions creationOptions;
        readonly ManualResetEvent completedEvent = new(false);

        void Worker(object nothing = null)
        {
            void Exit()
            {
                lock (this)
                {
                    IsCompleted = true;
                    completedEvent.Set();
                }
            }

            try
            {
                // spawn a new seperate thread for long running threads
                if ((creationOptions == TaskCreationOptions.LongRunning) && Thread.CurrentThread.IsThreadPoolThread)
                {
                    var thread = new Thread(Worker)
                    {
                        IsBackground = true,
                    };
                    thread.Start(null);
                    return;
                }
            }
            catch (Exception ex)
            {
                Exception = new AggregateException(ex);
                Exit();
                return;
            }

            try
            {
                if (action is Action actionTyp1)
                {
                    actionTyp1();
                    return;
                }
                else if (action is Action<object> actionTyp2)
                {
                    actionTyp2(state);
                    return;
                }
                else
                {
                    throw new ExecutionEngineException($"Fatal exception in Task.Worker. Invalid action type {action}!");
                }
            }
            catch (Exception ex)
            {
                Exception = new AggregateException(ex);
            }
            finally
            {
                Exit();
            }
        }
        #endregion

        #region constructor
        private Task(TaskCreationOptions creationOptions, object action, object state)
        {
            this.creationOptions = creationOptions;
            this.action = action;
            this.state = state;
        }
        #endregion

        #region public functionality

        /// <summary>
        /// Waits for a task completion.
        /// </summary>
        public void Wait()
        {
            while (!IsCompleted)
            {
                completedEvent.WaitOne();
            }
            if (IsFaulted)
            {
                throw Exception;
            }
        }

        /// <summary>
        /// Waits for a task completion.
        /// </summary>
        /// <param name="mssTimeout">Milliseconds to wait.</param>
        /// <returns>Releases the lock on an object and blocks the current thread until it reacquires the lock.</returns>
        public bool Wait(int mssTimeout)
        {
            if (IsCompleted)
            {
                return !IsFaulted ? true : throw Exception;
            }

            var result = completedEvent.WaitOne(mssTimeout);
            return !IsFaulted ? result : throw Exception;
        }
        #endregion

        #region public properties

        /// <summary>
        /// Gets the expection thown by a task.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the task completed or not.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Task completed due to an unhandled exception.
        /// </summary>
        public bool IsFaulted => Exception != null;
        #endregion

        #region IDisposable Member

        /// <summary>Releases the unmanaged resources used by this instance and optionally releases the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Frees all used resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
#endif
#pragma warning restore IDE0055
