using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    [TestFixture]
    class TaskTests
    {
        #region Private Methods

        void TestSleep(object syncRoot, int number)
        {
            Thread.Sleep(1000 - number);
        }

        private void TestWait(Task task)
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

        #endregion Private Methods

        #region Public Methods

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
            object syncRoot = new object();
            var list = new List<Task>();
            for (int i = 0; i < 1000; i++)
            {
                var t = Task.Factory.StartNew((n) => TestSleep(syncRoot, (int)n), i);
                list.Add(t);
            }
            Task.WaitAll(list.ToArray());
        }

        [Test]
        public void TaskStartWait2()
        {
            object syncRoot = new object();
            var list = new List<Task>();
            for (int i = 0; i < 1000; i++)
            {
                var t = Task.Factory.StartNew((n) => TestSleep(syncRoot, (int)n), i);
                list.Add(t);
            }
            foreach (var t in list) t.Wait();
        }

        #endregion Public Methods
    }
}
