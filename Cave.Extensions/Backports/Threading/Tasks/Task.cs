#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20 || NET35

using System.ComponentModel;

namespace System.Threading.Tasks;

public class Task<TResult> : Task
{
    Func<TResult> func;

    public TResult Result { get; private set; }

    internal override void OnRunAction() => Result = func();

    public Task(Func<TResult> func, TaskCreationOptions options = TaskCreationOptions.None) : base(options, null, null) => this.func = func;
}

public class Task : IDisposable
{
    #region Nested type: Factory

    #region Task.Factory class

    public static class Factory
    {
        #region Static

        public static Task<TResult> StartNew<TResult>(Func<TResult> func, TaskCreationOptions options = TaskCreationOptions.None)
        {
            var task = new Task<TResult>(func, options);
            ThreadPool.QueueUserWorkItem(task.Worker, null);
            return task;
        }

        public static Task<TResult> StartNew<TResult>(Func<object, TResult> func, object state, TaskCreationOptions options = TaskCreationOptions.None)
        {
            var task = new Task<TResult>(() => func(state), options);
            ThreadPool.QueueUserWorkItem(task.Worker, null);
            return task;
        }

        public static Task StartNew(Action action, TaskCreationOptions options = TaskCreationOptions.None)
        {
            var task = new Task(options, action, null);
            ThreadPool.QueueUserWorkItem(task.Worker, null);
            return task;
        }

        public static Task StartNew(Action<object> action, object state, TaskCreationOptions options = TaskCreationOptions.None)
        {
            var task = new Task(options, action, state);
            ThreadPool.QueueUserWorkItem(task.Worker, null);
            return task;
        }

        public static Task StartNew<T>(Action<T> action, object state, TaskCreationOptions options = TaskCreationOptions.None)
            => StartNew(o => action((T)o), state, options);

        static Factory()
        {
            ThreadPool.SetMaxThreads(1000, 1000);
            ThreadPool.SetMinThreads(100, 100);
        }

        #endregion Static
    }

    #endregion Task.Factory class

    #endregion Nested type: Factory

    #region Static

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
            if ((wait < TimeSpan.Zero) && !task.IsCompleted)
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

    #endregion Static

    #region Fields

    readonly object state;
    readonly object action;
    readonly TaskCreationOptions creationOptions;
    readonly ManualResetEvent completedEvent = new(false);

    #endregion Fields

    #region Constructors

    internal Task(TaskCreationOptions creationOptions, object action, object state)
    {
        this.creationOptions = creationOptions;
        this.action = action;
        this.state = state;
    }

    #endregion Constructors

    #region Properties

    public bool IsFaulted => Exception != null;

    public Exception Exception { get; private set; }

    public bool IsCompleted { get; private set; }

    #endregion Properties

    #region Members

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

    public bool Wait(int mssTimeout)
    {
        if (IsCompleted)
        {
            return !IsFaulted ? true : throw Exception;
        }

        var result = completedEvent.WaitOne(mssTimeout);
        return !IsFaulted ? result : throw Exception;
    }

    internal virtual void OnRunAction()
    {
        if (action is Action actionTyp1)
        {
            actionTyp1();
        }
        else if (action is Action<object> actionTyp2)
        {
            actionTyp2(state);
        }
        else
        {
            throw new ExecutionEngineException($"Fatal exception in Task.Worker. Invalid action type {action}!");
        }
    }

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
            // spawn a new separate thread for long running threads
            if ((creationOptions == TaskCreationOptions.LongRunning) && Thread.CurrentThread.IsThreadPoolThread)
            {
                var thread = new Thread(Worker)
                {
                    IsBackground = true
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
            OnRunAction();
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

    #endregion Members

    #region IDisposable Member

    protected virtual void Dispose(bool disposing) { }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion IDisposable Member
}

#endif
