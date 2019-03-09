using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cave
{
    /// <summary>
    /// Provides a task list for organizing waits.
    /// </summary>
    public class TaskList
    {
        /// <summary>
        /// The maximum concurrent threads.
        /// </summary>
        public int MaximumConcurrentThreads = Environment.ProcessorCount * 2;

        Dictionary<Task, object> tasks = new Dictionary<Task, object>();

        void Cleanup()
        {
            foreach (Task task in tasks.Keys.ToArray())
            {
                if (task.IsCompleted)
                {
                    lock (tasks)
                    {
                        tasks.Remove(task);
                    }
                    if (task is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        /// <summary>Adds the specified task.</summary>
        /// <param name="task">The task.</param>
        public void Add(Task task)
        {
            tasks.TryAdd(task, null);
        }

        /// <summary>Waits for all tasks to complete.</summary>
        public void WaitAll()
        {
            WaitAll(null, 1000);
        }

        /// <summary>Waits for all tasks to complete.</summary>
        public void WaitAll(Action action)
        {
            WaitAll(action, 1000);
        }

        /// <summary>Waits for all tasks to complete.</summary>
        public void WaitAll(Action action, int sleepTime)
        {
            while (true)
            {
                if (Task.WaitAll(ToArray(), sleepTime))
                {
                    Cleanup();
                    return;
                }
                action?.Invoke();
            }
        }

        /// <summary>Waits for any task to complete.</summary>
        public void WaitAny()
        {
            WaitAny(null, 1000);
        }

        /// <summary>Waits for any task to complete.</summary>
        public void WaitAny(Action action)
        {
            WaitAny(action, 1000);
        }

        /// <summary>Waits for any task to complete.</summary>
        public void WaitAny(Action action, int sleepTime)
        {
            while (true)
            {
                if (Task.WaitAny(ToArray(), sleepTime) > -1)
                {
                    Cleanup();
                    return;
                }
                action?.Invoke();
            }
        }

        /// <summary>Waits until the number of tasks falls below Environment.ProcessorCount.</summary>
        public void Wait()
        {
            Wait(null, 1000);
        }

        /// <summary>Waits until the number of tasks falls below Environment.ProcessorCount.</summary>
        public void Wait(Action action)
        {
            Wait(action, 1000);
        }

        /// <summary>Waits until the number of tasks falls below Environment.ProcessorCount.</summary>
        public void Wait(Action action, int sleepTime)
        {
            while (true)
            {
                Task[] tasks = ToArray();
                if (tasks.Length < MaximumConcurrentThreads)
                {
                    return;
                }

                if (Task.WaitAny(tasks, sleepTime) == -1)
                {
                    action?.Invoke();
                }

                Cleanup();
            }
        }

        /// <summary>Retrieves all tasks as array.</summary>
        /// <returns>Returns an array of Tasks.</returns>
        public Task[] ToArray()
        {
            Cleanup();
            return tasks.Keys.ToArray();
        }

        /// <summary>Gets the task count.</summary>
        /// <value>The task count.</value>
        public int Count
        {
            get
            {
                return tasks.Count;
            }
        }
    }
}
