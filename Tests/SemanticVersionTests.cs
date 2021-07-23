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
            Assert.AreEqual("1.2", ver.GetNormalizedVersion().ToString());
            Assert.AreEqual(1, ver.Major);
            Assert.AreEqual(2, ver.Minor);
            Assert.AreEqual(-1, ver.Patch);
            Assert.AreEqual("-test", ver.Meta);
            Assert.AreEqual(true, ver.IsMetaValid);
            Assert.IsTrue(ver.GetNormalizedVersion() < ver);
            Assert.IsTrue(ver.GetNormalizedVersion() <= ver);
        }

        [Test]
        public void ParseTest2()
        {
            var ver = SemanticVersion.Parse("1.2.3");
            Assert.AreEqual("1.2.3", ver.ToString());
            Assert.AreEqual("1.2.3", ver.GetNormalizedVersion().ToString());
            Assert.AreEqual(1, ver.Major);
            Assert.AreEqual(2, ver.Minor);
            Assert.AreEqual(3, ver.Patch);
            Assert.AreEqual(null, ver.Meta);
            Assert.AreEqual(true, ver.IsMetaValid);
            Assert.AreEqual((SemanticVersion)ver.GetNormalizedVersion(), ver);
            Assert.IsTrue(ver.GetNormalizedVersion() == ver);
            Assert.IsTrue(ver.GetNormalizedVersion() <= ver);
        }

        [Test]
        public void ParseTest3()
        {
            var ver = SemanticVersion.Parse("1.2.3.4.5.6.7.8-test");
            Assert.AreEqual("1.2.3.4.5.6.7.8-test", ver.ToString());
            Assert.AreEqual("1.2.3", ver.GetNormalizedVersion().ToString());
            Assert.AreEqual(1, ver.Major);
            Assert.AreEqual(2, ver.Minor);
            Assert.AreEqual(3, ver.Patch);
            Assert.AreEqual(".4.5.6.7.8-test", ver.Meta);
            Assert.AreEqual(false, ver.IsMetaValid);
            Assert.IsTrue(ver.GetNormalizedVersion() < ver);
            Assert.IsTrue(ver.GetNormalizedVersion() <= ver);
        }

        [Test]
        public void ParseTest4()
        {
            var ver = SemanticVersion.Parse("2015.1234.56789-metadata-v1.2");
            Assert.AreEqual("2015.1234.56789-metadata-v1.2", ver.ToString());
            Assert.AreEqual("2015.1234.56789", ver.GetNormalizedVersion().ToString());
            Assert.AreEqual(2015, ver.Major);
            Assert.AreEqual(1234, ver.Minor);
            Assert.AreEqual(56789, ver.Patch);
            Assert.AreEqual("-metadata-v1.2", ver.Meta);
            Assert.AreEqual(true, ver.IsMetaValid);
            Assert.IsTrue(ver.GetNormalizedVersion() < ver);
            Assert.IsTrue(ver.GetNormalizedVersion() <= ver);
        }

        [Test]
        public void ParseTest5()
        {
            var ver = SemanticVersion.Parse("2015.1234.56789+metadata-test-123+678");
            Assert.AreEqual("2015.1234.56789+metadata-test-123+678", ver.ToString());
            Assert.AreEqual("2015.1234.56789", ver.GetNormalizedVersion().ToString());
            Assert.AreEqual(2015, ver.Major);
            Assert.AreEqual(1234, ver.Minor);
            Assert.AreEqual(56789, ver.Patch);
            Assert.AreEqual("+metadata-test-123+678", ver.Meta);
            Assert.AreEqual(true, ver.IsMetaValid);
            Assert.IsTrue(ver.GetNormalizedVersion() < ver);
            Assert.IsTrue(ver.GetNormalizedVersion() <= ver);
        }

        [Test]
        public void ParseTest6()
        {
            var ver = SemanticVersion.TryParse("2015.1234.56789+metadata-test-123 INVALID!");
            Assert.AreEqual("2015.1234.56789+metadata-test-123 INVALID!", ver.ToString());
            Assert.AreEqual("2015.1234.56789", ver.GetNormalizedVersion().ToString());
            Assert.AreEqual(2015, ver.Major);
            Assert.AreEqual(1234, ver.Minor);
            Assert.AreEqual(56789, ver.Patch);
            Assert.AreEqual("+metadata-test-123 INVALID!", ver.Meta);
            Assert.AreEqual(false, ver.IsMetaValid);
            Assert.IsTrue(ver.GetNormalizedVersion() < ver);
            Assert.IsTrue(ver.GetNormalizedVersion() <= ver);
        }

        [Test]
        public void EqualTest()
        {
            var version1 = SemanticVersion.Parse("1.2");
            var version2 = SemanticVersion.Parse("1.2.0");
            Assert.IsTrue(version1 == version2);
            Assert.IsTrue(Equals(version1, version2));
            Assert.IsFalse(version1 < version2);
            Assert.IsFalse(version1 > version2);

            var version3 = SemanticVersion.Parse("1.2-meta");
            var version4 = SemanticVersion.Parse("1.2.0-meta");
            Assert.IsTrue(version3 == version4);
            Assert.IsTrue(Equals(version3, version4));
            Assert.IsFalse(version3 < version4);
            Assert.IsFalse(version3 > version4);
        }

        [Test]
        public void CompareTests()
        {
            var version1 = SemanticVersion.Parse("1.2.54-test+version-12");
            var version2 = SemanticVersion.Parse("1.2.55");
            var version3 = SemanticVersion.Parse("1.2.55-test+version-12");
            var version4 = SemanticVersion.Parse("1.2.56-test+version-12");
            var version5 = SemanticVersion.Parse("1.2.56-test+version-12a");
            var version6 = SemanticVersion.Parse("1.3");

            var versions = new[] { version1, version2, version3, version4, version5, version6 };
            for (var y = 0; y < versions.Length; y++)
            {
                for (var x = 0; x < versions.Length; x++)
                {
                    var verX = versions[x];
                    var verY = versions[y];
                    if (x == y)
                    {
                        Assert.IsTrue(verX == verY);
                        Assert.IsTrue(verX >= verY);
                        Assert.IsTrue(verX <= verY);
                        Assert.IsFalse(verX > verY);
                        Assert.IsFalse(verX < verY);
                        Assert.IsFalse(verX != verY);
                        Assert.AreEqual(0, verX.CompareTo(verY));
                    }
                    else if (x > y)
                    {
                        Assert.IsTrue(verX != verY);
                        Assert.IsFalse(verX == verY);
                        Assert.IsTrue(verX > verY);
                        Assert.IsTrue(verX >= verY);
                        Assert.IsFalse(verX < verY);
                        Assert.IsFalse(verX <= verY);
                        Assert.IsTrue(verX.CompareTo(verY) > 0);
                    }
                    else // x < y
                    {
                        Assert.IsTrue(verX != verY);
                        Assert.IsFalse(verX == verY);
                        Assert.IsTrue(verX < verY);
                        Assert.IsTrue(verX <= verY);
                        Assert.IsFalse(verX > verY);
                        Assert.IsFalse(verX >= verY);
                        Assert.IsTrue(verX.CompareTo(verY) < 0);
                    }
                }
            }
        }
    }
}
