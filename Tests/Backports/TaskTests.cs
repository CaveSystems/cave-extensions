using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Test.Backports;

[TestFixture]
class TaskTests
{
    static TaskTests()
    {
    }

    static void TestSleep(int number) => Thread.Sleep(1000 - number);

    static void TestWait(Task task)
    {
        try
        {
            task.Wait();
        }
        catch (Exception ex)
        {
            Assert.AreEqual(typeof(AggregateException), ex.GetType());
            Assert.AreEqual("TestException", ex.InnerException.Message);
            return;
        }

        Assert.Fail("No exception during wait!");
    }

    [Test]
    public void TaskException()
    {
        var task = Task.Factory.StartNew(() => throw new("TestException"));
        TestWait(task);
        Assert.IsTrue(task.IsFaulted);
        Assert.IsTrue(task.IsCompleted);
        Assert.AreEqual(typeof(AggregateException), task.Exception.GetType());
        Assert.AreEqual("TestException", task.Exception.InnerException.Message);
    }

    [Test]
    public void TaskExceptionLongRunning()
    {
        var task = Task.Factory.StartNew(() => throw new("TestException"), TaskCreationOptions.LongRunning);
        TestWait(task);
        Assert.IsTrue(task.IsFaulted);
        Assert.IsTrue(task.IsCompleted);
        Assert.AreEqual(typeof(AggregateException), task.Exception.GetType());
        Assert.AreEqual("TestException", task.Exception.InnerException.Message);
    }

    [Test]
    public void TaskStartWaitAction()
    {
        var syncRoot = new object();
        var list = new List<Task>();
        Action<object> action = n => TestSleep((int)n);
        for (var i = 0; i < 100; i++)
        {
            var t = Task.Factory.StartNew(action, i, TaskCreationOptions.LongRunning);
            list.Add(t);
        }

        Task.WaitAll(list.ToArray());
    }

    [Test]
    public void TaskConcurrrentTest()
    {
        var started = 0;
        var syncRoot = new object();
        var list = new List<Task>();
        var startSignal = new ManualResetEvent(false);
        Action<object> action = n => { Interlocked.Increment(ref started); startSignal.WaitOne(); };
        for (var i = 0; i < 100; i++)
        {
            var t = Task.Factory.StartNew(action, i, TaskCreationOptions.LongRunning);
            list.Add(t);
        }

        for (int i = 0; started < 100; i++)
        {
            Thread.Sleep(10);
            if (i > 1000) Assert.Fail($"Tasks started {started}, expected: 100!");
        }
        startSignal.Set();
        Assert.IsTrue(Task.WaitAll(list.ToArray(), 10000), "Tasks did not complete im time!");
    }

    [Test]
    public void ParallelConcurrrentTest()
    {
        var started = 0;
        var startSignal = new ManualResetEvent(false);

        var task = Task.Factory.StartNew(() =>
        {
            Parallel.For(0, 100, n =>
            {
                Interlocked.Increment(ref started);
                startSignal.WaitOne();
            });
        });

        for (int i = 0; started < 100; i++)
        {
            Thread.Sleep(100);
            if (i > 1000) Assert.Fail($"Tasks started {started}, expected: 100!");
        }
        startSignal.Set();
        Assert.IsTrue(task.Wait(10000), "Tasks did not complete im time!");
    }

    [Test]
    public void TaskStartWaitFunc()
    {
        var syncRoot = new object();
        var list = new List<Task>();
        Func<object, object> func = n => { TestSleep((int)n); return true; };
        for (var i = 0; i < 100; i++)
        {
            var t = Task.Factory.StartNew(func, i, TaskCreationOptions.LongRunning);
            list.Add(t);
        }

        foreach (var t in list)
        {
            t.Wait();
        }
    }

#if !NET20 && !NET35 && !NET40

    [Test]
    public async Task TaskAwaitAction()
    {
        var syncRoot = new object();
        Action<object> action = n => TestSleep((int)n);
        List<Task> tasks = new();
        for (var i = 0; i < 10; i++)
        {
            tasks.Add(Task.Factory.StartNew(action, i, TaskCreationOptions.LongRunning));
        }
        for (var i = 0; i < 10; i++)
        {
            await tasks[i];
        }
    }

    [Test]
    public async Task TaskAwaitFunc()
    {
        var syncRoot = new object();
        var list = new List<Task>();
        Func<object, bool> func = n => { TestSleep((int)n); return true; };
        List<Task> tasks = new();
        for (var i = 0; i < 10; i++)
        {
            tasks.Add(Task.Factory.StartNew(func, i, TaskCreationOptions.LongRunning));
        }
        for (var i = 0; i < 10; i++)
        {
            await tasks[i];
        }
    }

#endif
}
