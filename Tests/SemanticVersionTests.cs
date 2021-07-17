using Cave;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    class SemanticVersionTests
    {
        [Test]
        public void ParseTest1()
        {
            var ver = SemanticVersion.Parse("1.2-test");
            Assert.AreEqual("1.2-test", ver.ToString());
            Assert.AreEqual("1.2", ver.GetClassicVersion().ToString());
            Assert.AreEqual(1, ver.Major);
            Assert.AreEqual(2, ver.Minor);
            Assert.AreEqual(-1, ver.Patch);
            Assert.AreEqual("-test", ver.Meta);
            Assert.AreEqual(true, ver.IsMetaValid);
        }

        [Test]
        public void ParseTest2()
        {
            var ver = SemanticVersion.Parse("1.2.3");
            Assert.AreEqual("1.2.3", ver.ToString());
            Assert.AreEqual("1.2.3", ver.GetClassicVersion().ToString());
            Assert.AreEqual(1, ver.Major);
            Assert.AreEqual(2, ver.Minor);
            Assert.AreEqual(3, ver.Patch);
            Assert.AreEqual(null, ver.Meta);
            Assert.AreEqual(true, ver.IsMetaValid);
        }

        [Test]
        public void ParseTest3()
        {
            var ver = SemanticVersion.Parse("1.2.3.4.5.6.7.8-test");
            Assert.AreEqual("1.2.3.4.5.6.7.8-test", ver.ToString());
            Assert.AreEqual("1.2.3", ver.GetClassicVersion().ToString());
            Assert.AreEqual(1, ver.Major);
            Assert.AreEqual(2, ver.Minor);
            Assert.AreEqual(3, ver.Patch);
            Assert.AreEqual(".4.5.6.7.8-test", ver.Meta);
            Assert.AreEqual(false, ver.IsMetaValid);
        }

        [Test]
        public void ParseTest4()
        {
            var ver = SemanticVersion.Parse("2015.1234.56789-metadata-v1.2");
            Assert.AreEqual("2015.1234.56789-metadata-v1.2", ver.ToString());
            Assert.AreEqual("2015.1234.56789", ver.GetClassicVersion().ToString());
            Assert.AreEqual(2015, ver.Major);
            Assert.AreEqual(1234, ver.Minor);
            Assert.AreEqual(56789, ver.Patch);
            Assert.AreEqual("-metadata-v1.2", ver.Meta);
            Assert.AreEqual(true, ver.IsMetaValid);
        }

        [Test]
        public void ParseTest5()
        {
            var ver = SemanticVersion.Parse("2015.1234.56789+metadata-test-123+678");
            Assert.AreEqual("2015.1234.56789+metadata-test-123+678", ver.ToString());
            Assert.AreEqual("2015.1234.56789", ver.GetClassicVersion().ToString());
            Assert.AreEqual(2015, ver.Major);
            Assert.AreEqual(1234, ver.Minor);
            Assert.AreEqual(56789, ver.Patch);
            Assert.AreEqual("+metadata-test-123+678", ver.Meta);
            Assert.AreEqual(true, ver.IsMetaValid);
        }

        [Test]
        public void ParseTest6()
        {
            var ver = SemanticVersion.TryParse("2015.1234.56789+metadata-test-123 INVALID!");
            Assert.AreEqual("2015.1234.56789+metadata-test-123 INVALID!", ver.ToString());
            Assert.AreEqual("2015.1234.56789", ver.GetClassicVersion().ToString());
            Assert.AreEqual(2015, ver.Major);
            Assert.AreEqual(1234, ver.Minor);
            Assert.AreEqual(56789, ver.Patch);
            Assert.AreEqual("+metadata-test-123 INVALID!", ver.Meta);
            Assert.AreEqual(false, ver.IsMetaValid);
        }
    }
}
