using System;
using Cave;
using NUnit.Framework;

namespace Test;

[TestFixture]
class AppDomTests
{
    [Test]
    public void SystemUri()
    {
        var type = typeof(Uri);
        var result = AppDom.FindType(type.AssemblyQualifiedName);
        Assert.AreEqual(type, result);
    }
}
