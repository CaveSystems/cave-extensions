using Cave;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class ConnectionStringTest
    {
        [Test]
        public void Parse()
        {
            var c = ConnectionString.Parse("mysql://user:pass@server/path");
            Assert.AreEqual("mysql", c.Protocol);
            Assert.AreEqual("user", c.UserName);
            Assert.AreEqual("pass", c.Password);
            Assert.AreEqual("server", c.Server);
            Assert.AreEqual("path", c.Location);
        }

        [Test]
        public void Parse2()
        {
            var c = ConnectionString.Parse("memory://");
            Assert.AreEqual("memory", c.Protocol);
            Assert.AreEqual(null, c.UserName);
            Assert.AreEqual(null, c.Password);
            Assert.AreEqual(null, c.Server);
            Assert.AreEqual(null, c.Location);
        }

        [Test]
        public void Parse3()
        {
            var c = ConnectionString.Parse("memory:///path/2/3/4/");
            Assert.AreEqual("memory", c.Protocol);
            Assert.AreEqual(null, c.UserName);
            Assert.AreEqual(null, c.Password);
            Assert.AreEqual(null, c.Server);
            Assert.AreEqual("path/2/3/4/", c.Location);
        }

        [Test]
        public void Parse4()
        {
            var c = ConnectionString.Parse("memory://server:123");
            Assert.AreEqual("memory", c.Protocol);
            Assert.AreEqual(null, c.UserName);
            Assert.AreEqual(null, c.Password);
            Assert.AreEqual("server", c.Server);
            Assert.AreEqual((ushort)123, c.Port);
            Assert.AreEqual(null, c.Location);
        }

        [Test]
        public void Parse5()
        {
            var c = ConnectionString.Parse("memory://user:pass@server:123");
            Assert.AreEqual("memory", c.Protocol);
            Assert.AreEqual("user", c.UserName);
            Assert.AreEqual("pass", c.Password);
            Assert.AreEqual("server", c.Server);
            Assert.AreEqual((ushort)123, c.Port);
            Assert.AreEqual(null, c.Location);
        }

        [Test]
        public void Parse6()
        {
            ConnectionString.TryParse("mysql://user:pass@server/path", out ConnectionString c);
            Assert.AreEqual("mysql", c.Protocol);
            Assert.AreEqual("user", c.UserName);
            Assert.AreEqual("pass", c.Password);
            Assert.AreEqual("server", c.Server);
            Assert.AreEqual("path", c.Location);
        }

        [Test]
        public void Parse7()
        { 
            ConnectionString.TryParse("memory://", out ConnectionString c);
            Assert.AreEqual("memory", c.Protocol);
            Assert.AreEqual(null, c.UserName);
            Assert.AreEqual(null, c.Password);
            Assert.AreEqual(null, c.Server);
            Assert.AreEqual(null, c.Location);
        }

        [Test]
        public void Parse8()
        {
            ConnectionString.TryParse("memory:///path/2/3/4/", out ConnectionString c);
            Assert.AreEqual("memory", c.Protocol);
            Assert.AreEqual(null, c.UserName);
            Assert.AreEqual(null, c.Password);
            Assert.AreEqual(null, c.Server);
            Assert.AreEqual("path/2/3/4/", c.Location);
        }

        [Test]
        public void Parse9()
        {
            ConnectionString.TryParse("memory://server:123", out ConnectionString c);
            Assert.AreEqual("memory", c.Protocol);
            Assert.AreEqual(null, c.UserName);
            Assert.AreEqual(null, c.Password);
            Assert.AreEqual("server", c.Server);
            Assert.AreEqual((ushort)123, c.Port);
            Assert.AreEqual(null, c.Location);
        }

        [Test]
        public void Parse10()
        {
            ConnectionString.TryParse("memory://user:pass@server:123", out ConnectionString c);
            Assert.AreEqual("memory", c.Protocol);
            Assert.AreEqual("user", c.UserName);
            Assert.AreEqual("pass", c.Password);
            Assert.AreEqual("server", c.Server);
            Assert.AreEqual((ushort)123, c.Port);
            Assert.AreEqual(null, c.Location);
        }
    }
}
