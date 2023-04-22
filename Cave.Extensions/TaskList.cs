using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cave;

/// <summary>Gets a task list for organizing waits.</summary>
public class TaskList
{
    #region Fields

    readonly Dictionary<Task, object> tasks = new();

    /// <summary>The maximum concurrent threads.</summary>
    public int MaximumConcurrentThreads = Environment.ProcessorCount * 2;

    #endregion

    #region Properties

    /// <summary>Gets the task count.</summary>
    /// <value>The task count.</value>
    public int Count => tasks.Count;

    #endregion

    #region Members

    /// <summary>Adds the specified task.</summary>
    /// <param name="task">The task.</param>
    public void Add(Task task) => tasks.TryAdd(task, null);

    /// <summary>Retrieves all tasks as array.</summary>
    /// <returns>Returns an array of Tasks.</returns>
    public Task[] ToArray()
    {
        Cleanup();
        return tasks.Keys.ToArray();
    }

    /// <summary>Waits until the number of tasks falls below Environment.ProcessorCount.</summary>
    public void Wait() => Wait(null, 1000);

    /// <summary>Waits until the number of tasks falls below Environment.ProcessorCount.</summary>
    /// <param name="action">The action to wait for.</param>
    public void Wait(Action action) => Wait(action, 1000);

    /// <summary>Waits until the number of tasks falls below Environment.ProcessorCount.</summary>
    /// <param name="action">The action to wait for.</param>
    /// <param name="sleepTime">The time in milliseconds to sleep while waiting.</param>
    public void Wait(Action action, int sleepTime)
    {
        while (true)
        {
            var tasks = ToArray();
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

    /// <summary>Waits for all tasks to complete.</summary>
    public void WaitAll() => WaitAll(null, 1000);

    /// <summary>Waits for all tasks to complete.</summary>
    /// <param name="action">The action to wait for.</param>
    public void WaitAll(Action action) => WaitAll(action, 1000);

    /// <summary>Waits for all tasks to complete.</summary>
    /// <param name="action">The action to wait for.</param>
    /// <param name="sleepTime">The time in milliseconds to sleep while waiting.</param>
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
    public void WaitAny() => WaitAny(null, 1000);

    /// <summary>Waits for any task to complete.</summary>
    /// <param name="action">The action to wait for.</param>
    public void WaitAny(Action action) => WaitAny(action, 1000);

    /// <summary>Waits for any task to complete.</summary>
    /// <param name="action">The action to wait for.</param>
    /// <param name="sleepTime">The time in milliseconds to sleep while waiting.</param>
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

    void Cleanup()
    {
        foreach (var task in tasks.Keys.ToArray())
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

    #endregion
}
