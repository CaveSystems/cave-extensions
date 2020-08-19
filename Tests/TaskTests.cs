using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Test
{
    [TestFixture]
    class TaskTests
    {
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
    }
}
