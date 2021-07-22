using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Test.Backports
{
    [TestFixture]
    class TaskTests
    {
        void TestSleep(int number) => Thread.Sleep(1000 - number);

        void TestWait(Task task)
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
            var task = Task.Factory.StartNew(() => throw new Exception("TestException"));
            TestWait(task);
            Assert.IsTrue(task.IsFaulted);
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(typeof(AggregateException), task.Exception.GetType());
            Assert.AreEqual("TestException", task.Exception.InnerException.Message);
        }

        [Test]
        public void TaskExceptionLongRunning()
        {
            var task = Task.Factory.StartNew(() => throw new Exception("TestException"), TaskCreationOptions.LongRunning);
            TestWait(task);
            Assert.IsTrue(task.IsFaulted);
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(typeof(AggregateException), task.Exception.GetType());
            Assert.AreEqual("TestException", task.Exception.InnerException.Message);
        }

        [Test]
        public void TaskStartWait()
        {
            ThreadPool.SetMinThreads(100, 100);
            var syncRoot = new object();
            var list = new List<Task>();
            for (var i = 0; i < 1000; i++)
            {
                var t = Task.Factory.StartNew(n => TestSleep((int)n), i);
                list.Add(t);
            }

            Task.WaitAll(list.ToArray());
        }

        [Test]
        public void TaskStartWait2()
        {
            ThreadPool.SetMinThreads(100, 100);
            var syncRoot = new object();
            var list = new List<Task>();
            for (var i = 0; i < 1000; i++)
            {
                var t = Task.Factory.StartNew(n => TestSleep((int)n), i);
                list.Add(t);
            }

            foreach (var t in list)
            {
                t.Wait();
            }
        }
    }
}
