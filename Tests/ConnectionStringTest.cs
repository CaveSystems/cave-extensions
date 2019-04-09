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

        [Test]
        public void ConnectionStringTest1()
        {
            string text = "http://admin:password@localhost:80/somewhere/at/the/forest";
            var string1 = new ConnectionString("http", "admin", "password", "localhost", 80, "somewhere/at/the/forest");
            var string2 = ConnectionString.Parse(text);
            Assert.AreEqual(string1, string2);
            Assert.AreEqual(text, string1.ToString());
        }

        [Test]
        public void ConnectionStringTest2()
        {
            string text = @"file:///c:\somewhere\at\the\window";
            var string1 = new ConnectionString("file", null, null, null, 0, @"c:\somewhere\at\the\window");
            var string2 = ConnectionString.Parse(text);
            Assert.AreEqual(string1, string2);
            Assert.AreEqual(@"file:///c:/somewhere/at/the/window", string1.ToString());
        }

        [Test]
        public void ConnectionStringTest3()
        {
            string text = @"file:////somewhere/at/the/root";
            var string1 = new ConnectionString("file", null, null, null, 0, @"/somewhere/at/the/root");
            var string2 = ConnectionString.Parse(text);
            Assert.AreEqual(string1, string2);
            Assert.AreEqual(text, string1.ToString(ConnectionStringPart.All));
        }

        [Test]
        public void ConnectionStringTest4()
        {
            string text = "http://admin@localhost/somewhere/at/the/forest";
            var string1 = new ConnectionString("http", "admin", null, "localhost", 0, "somewhere/at/the/forest");
            var string2 = ConnectionString.Parse(text);
            Assert.AreEqual(string1, string2);
            Assert.AreEqual(text, string1.ToString(ConnectionStringPart.All));
        }

        [Test]
        public void ConnectionStringTest5()
        {
            string text = "http://localhost//somewhere/at/the/forest:54554:123";
            var string1 = new ConnectionString("http", null, null, "localhost", 0, "/somewhere/at/the/forest:54554:123");
            var string2 = ConnectionString.Parse(text);
            Assert.AreEqual(string1, string2);
            Assert.AreEqual(text, string1.ToString(ConnectionStringPart.All));
        }


        [Test]
        public void ConnectionStringPath()
        {
            string text = @"c:\test";

            var string1 = new ConnectionString(null, null, null, null, 0, @"c:\test");
            var string2 = ConnectionString.Parse(text);
            Assert.AreEqual(string1, string2);
            Assert.AreEqual(@"c:/test", string1.ToString(ConnectionStringPart.All));
        }

        [Test]
        public void ConnectionStringUNC()
        {
            string text = @"\\test\2\3\4";
            string alt = @"//test/2/3/4";
            var string1 = new ConnectionString(null, null, null, null, 0, text);
            var string2 = ConnectionString.Parse(text);
            var string3 = new ConnectionString(null, null, null, null, 0, alt);
            var string4 = ConnectionString.Parse(alt);
            Assert.AreEqual(string1, string2);
            Assert.AreEqual(string2, string3);
            Assert.AreEqual(string3, string4);
            Assert.AreEqual("//test/2/3/4", string1.ToString(ConnectionStringPart.All));
        }
    }
}
