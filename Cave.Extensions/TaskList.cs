using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cave.Collections.Generic;

namespace Cave;

/// <summary>Gets a task list for organizing tasks based on completion speed.</summary>
public class TaskList
{
    #region Private Fields

    readonly Set<Task> tasks = new();

    /// <summary>used at <see cref="WaitAny()"/> and overloads to see task completion when not checking.</summary>
    int waitAny = 0;

    #endregion Private Fields

    #region Public Fields

    /// <summary>The maximum concurrent threads.</summary>
    public int MaximumConcurrentThreads = Environment.ProcessorCount * 2;

    #endregion Public Fields

    #region Public Properties

    /// <summary>Gets the task count.</summary>
    /// <value>The task count.</value>
    public int Count => tasks.Count;

    #endregion Public Properties

    #region Public Methods

    /// <summary>Adds the specified task. The task has be be already scheduled using Task.Run or Task.Factory.StartNew</summary>
    /// <param name="task">The task.</param>
    public void Add(Task task)
    {
        if (task.Status == TaskStatus.Created)
        {
            throw new InvalidOperationException($"Only add started tasks to the {nameof(TaskList)}!");
        }
        lock (this)
        {
            tasks.Add(task);
        }
    }

    /// <summary>Checks all tasks using the specified <paramref name="filter"/> and returns true if a matching one is found.</summary>
    /// <param name="filter">Filter to apply</param>
    /// <returns>Returns true if the filter matches at least one task, false otherwise.</returns>
    public bool Any(Func<Task, bool> filter)
    {
        lock (this)
        {
            return tasks.Any(filter);
        }
    }

    /// <summary>Checks all tasks whether any is still running and returns true if a matching one is found.</summary>
    /// <returns>Returns true if at least one task is still running, false otherwise.</returns>
    public bool AnyIsRunning() => Any(task => !task.IsCompleted);

    /// <summary>Checks all tasks whether any is successfully completed and returns true if a matching one is found.</summary>
    /// <returns>Returns true if at least one task is successfully completed, false otherwise.</returns>
    public bool AnySucceeded() => Any(task => task.Status == TaskStatus.RanToCompletion);

    /// <summary>Gets all exceptions of all tasks</summary>
    /// <returns>Returns a new list containing all exceptions thrown inside any task.</returns>
    public Exception[] GetErrors()
    {
        lock (this)
        {
            return [.. tasks.Where(task => task.IsFaulted).Select(task => task.Exception)];
        }
    }

    /// <summary>Gets the first available result. This requires a successful call to <see cref="AnySucceeded"/> first!</summary>
    /// <typeparam name="TResult"></typeparam>
    /// <returns>Returns the first available result.</returns>
    /// <exception cref="InvalidOperationException">No task completed! Test with <see cref="AnySucceeded"/> first!</exception>
    /// <exception cref="InvalidOperationException">Task {task} is not of type Task{TResult}!</exception>
    public TResult GetFirstResult<TResult>()
    {
        lock (this)
        {
            var task = tasks.FirstOrDefault(task => task.IsCompleted);
            if (task is null) throw new InvalidOperationException($"No task completed! Test with {nameof(AnySucceeded)} first!");
            if (task is not Task<TResult> typed) throw new InvalidOperationException($"Task {task} is not of type Task<{typeof(TResult).Name}>!");
            return typed.Result;
        }
    }

    /// <summary>Gets the list of running tasks</summary>
    /// <returns>Returns a new list of running tasks</returns>
    public Task[] GetRunningTasks() => GetTasks(task => !task.IsCompleted);

    /// <summary>Retrieves all tasks as array.</summary>
    /// <returns>Returns an array of Tasks.</returns>
    public Task[] GetTasks() => GetTasks(null);

    /// <summary>Gets all tasks matching the specified filter</summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public Task[] GetTasks(Func<Task, bool>? filter)
    {
        lock (this)
        {
            IEnumerable<Task> t = tasks;
            if (filter is not null) t = t.Where(filter);
            return [.. t];
        }
    }

    /// <summary>Removes all completed tasks (failed, cancelled and ran to completion).</summary>
    /// <returns>Returns the list of tasks removed</returns>
    public Task[] RemoveCompleted()
    {
        lock (tasks)
        {
            Task[] completed = [.. tasks.Where(task => task.IsCompleted)];
            tasks.RemoveRange(completed);
            waitAny = 0;
            return completed;
        }
    }

    /// <summary>Starts and adds a new task for the specified <paramref name="function"/>.</summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="function">Function to call.</param>
    public void StartNew<TResult>(Func<TResult> function) => Add(Task.Factory.StartNew(function));

    /// <summary>Starts and adds a new task for the specified <paramref name="action"/>.</summary>
    /// <param name="action">Action to call</param>
    public void StartNew(Action action) => Add(Task.Factory.StartNew(action));

    /// <summary>Retrieves all tasks as array.</summary>
    /// <returns>Returns an array of Tasks.</returns>
    [Obsolete("Use GetRunningTasks() instead!")]
    public Task[] ToArray() => GetRunningTasks();

    /// <summary>Waits until the number of tasks falls below Environment.ProcessorCount.</summary>
    [Obsolete("Use WaitFreeThreads() instead!")]
    public void Wait() => WaitFreeThreads();

    /// <summary>Waits until the number of tasks falls below Environment.ProcessorCount.</summary>
    /// <param name="action">The action to run while waiting (will be called 1/s).</param>
    [Obsolete("Use WaitFreeThreads() instead!")]
    public void Wait(Action? action) => WaitFreeThreads(action);

    /// <summary>Waits until the number of tasks falls below Environment.ProcessorCount.</summary>
    /// <param name="action">The action to run while waiting.</param>
    /// <param name="sleepTime">The time in milliseconds to sleep while waiting.</param>
    [Obsolete("Use WaitFreeThreads() instead!")]
    public void Wait(Action? action, int sleepTime) => WaitFreeThreads(action, sleepTime);

    /// <summary>Waits for all tasks to complete.</summary>
    public void WaitAll() => WaitAll(null, -1);

    /// <summary>Waits for all tasks to complete.</summary>
    /// <param name="action">The action to call each 1000ms.</param>
    public void WaitAll(Action? action) => WaitAll(action, 1000);

    /// <summary>Waits for all tasks to complete.</summary>
    /// <param name="action">The action to call while waiting.</param>
    /// <param name="sleepTime">The time in milliseconds to sleep while waiting.</param>
    public void WaitAll(Action? action, int sleepTime)
    {
        while (true)
        {
            if (Task.WaitAll(GetRunningTasks(), sleepTime))
            {
                return;
            }

            action?.Invoke();
        }
    }

    /// <summary>Waits for any task to complete.</summary>
    public void WaitAny() => WaitAny(null, 1000);

    /// <summary>Waits for any task to complete.</summary>
    /// <param name="action">The action to call each 1000ms.</param>
    public void WaitAny(Action? action) => WaitAny(action, 1000);

    /// <summary>Waits for any task to complete.</summary>
    /// <param name="action">The action to call while waiting.</param>
    /// <param name="sleepTime">The time in milliseconds to sleep while waiting.</param>
    public void WaitAny(Action? action, int sleepTime)
    {
        lock (this)
        {
            while (true)
            {
                Task[] running = [.. tasks.Where(t => !t.IsCompleted)];
                if (waitAny != running.Length || Task.WaitAny(running, sleepTime) > -1)
                {
                    break;
                }
                action?.Invoke();
            }
            waitAny = tasks.Count(t => !t.IsCompleted);
        }
    }

    /// <summary>Waits until the number of tasks falls below Environment.ProcessorCount.</summary>
    public void WaitFreeThreads() => WaitFreeThreads(null, -1);

    /// <summary>Waits until the number of tasks falls below Environment.ProcessorCount.</summary>
    /// <param name="action">The action to run while waiting (will be called 1/s).</param>
    public void WaitFreeThreads(Action? action) => WaitFreeThreads(action, 1000);

    /// <summary>Waits until the number of tasks falls below Environment.ProcessorCount.</summary>
    /// <param name="action">The action to run while waiting.</param>
    /// <param name="sleepTime">The time in milliseconds to sleep while waiting.</param>
    public void WaitFreeThreads(Action? action, int sleepTime)
    {
        while (true)
        {
            var tasks = GetRunningTasks();
            if (tasks.Length < MaximumConcurrentThreads)
            {
                return;
            }

            if (Task.WaitAny(tasks, sleepTime) == -1)
            {
                action?.Invoke();
            }
        }
    }

    #endregion Public Methods
}
