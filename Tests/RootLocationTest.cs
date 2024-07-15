using System;
using System.Runtime.InteropServices;
using Cave;
using Cave.Collections;
using Cave.Security;
using NUnit.Framework;

namespace Test;

[TestFixture]
class RootLocationTest
{
    #region Private Methods

    void TestDefaultLocationsWindows()
    {
        var user = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var machine = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        Assert.IsTrue(FileSystem.PathEquals(user, FileSystem.UserAppData));
        Assert.IsTrue(FileSystem.PathEquals(local, FileSystem.LocalUserAppData));
        Assert.IsTrue(FileSystem.PathEquals(machine, FileSystem.LocalMachineAppData));

        Assert.IsTrue(FileSystem.PathEquals(user, FileSystem.UserConfiguration));
        Assert.IsTrue(FileSystem.PathEquals(local, FileSystem.LocalUserConfiguration));
        Assert.IsTrue(FileSystem.PathEquals(machine, FileSystem.LocalMachineConfiguration));

        Assert.IsTrue(FileSystem.PathEquals(user, Environment.ExpandEnvironmentVariables("%appdata%")));
        Assert.IsTrue(FileSystem.PathEquals(local, Environment.ExpandEnvironmentVariables("%localappdata%")));
        Assert.IsTrue(FileSystem.PathEquals(machine, Environment.ExpandEnvironmentVariables("%programdata%")));

        {
            var loc1 = RootLocation.RoamingUserData.GetFolder();
            var loc2 = RootLocation.RoamingUserConfig.GetFolder();
            var loc3 = RootLocation.LocalUserData.GetFolder();
            var loc4 = RootLocation.LocalUserConfig.GetFolder();
            var loc5 = RootLocation.AllUsersData.GetFolder();
            var loc6 = RootLocation.AllUserConfig.GetFolder();

            Assert.IsTrue(FileSystem.PathEquals(user, loc1));
            Assert.AreEqual(loc1, loc2);
            Assert.IsTrue(FileSystem.PathEquals(local, loc3));
            Assert.AreEqual(loc3, loc4);
            Assert.IsTrue(FileSystem.PathEquals(machine, loc5));
            Assert.AreEqual(loc5, loc6);
        }
        {
            var loc1 = RootLocation.RoamingUserData.GetFolder("Company", "Product");
            var loc2 = RootLocation.RoamingUserConfig.GetFolder("Company", "Product");
            var loc3 = RootLocation.LocalUserData.GetFolder("Company", "Product");
            var loc4 = RootLocation.LocalUserConfig.GetFolder("Company", "Product");
            var loc5 = RootLocation.AllUsersData.GetFolder("Company", "Product");
            var loc6 = RootLocation.AllUserConfig.GetFolder("Company", "Product");

            Assert.IsTrue(FileSystem.PathEquals($"{user}/Company/Product", loc1));
            Assert.AreEqual(loc1, loc2);
            Assert.IsTrue(FileSystem.PathEquals($"{local}/Company/Product", loc3));
            Assert.AreEqual(loc3, loc4);
            Assert.IsTrue(FileSystem.PathEquals($"{machine}/Company/Product", loc5));
            Assert.AreEqual(loc5, loc6);
        }
        {
            var loc1 = RootLocation.RoamingUserData.GetFileName("Company/Product", "FileName", ".Extension");
            var loc2 = RootLocation.RoamingUserConfig.GetFileName("Company/Product", "FileName", ".Extension");
            var loc3 = RootLocation.LocalUserData.GetFileName("Company/Product", "FileName", ".Extension");
            var loc4 = RootLocation.LocalUserConfig.GetFileName("Company/Product", "FileName", ".Extension");
            var loc5 = RootLocation.AllUsersData.GetFileName("Company/Product", "FileName", ".Extension");
            var loc6 = RootLocation.AllUserConfig.GetFileName("Company/Product", "FileName", ".Extension");

            Assert.IsTrue(FileSystem.PathEquals($"{user}/Company/Product/FileName.Extension", loc1));
            Assert.AreEqual(loc1, loc2);
            Assert.IsTrue(FileSystem.PathEquals($"{local}/Company/Product/FileName.Extension", loc3));
            Assert.AreEqual(loc3, loc4);
            Assert.IsTrue(FileSystem.PathEquals($"{machine}/Company/Product/FileName.Extension", loc5));
            Assert.AreEqual(loc5, loc6);
        }
        {
            var loc1 = RootLocation.RoamingUserData.GetFileName("full/path/to/test.file");
            var loc2 = RootLocation.RoamingUserConfig.GetFileName("full/path/to/test.file");
            var loc3 = RootLocation.LocalUserData.GetFileName("full/path/to/test.file");
            var loc4 = RootLocation.LocalUserConfig.GetFileName("full/path/to/test.file");
            var loc5 = RootLocation.AllUsersData.GetFileName("full/path/to/test.file");
            var loc6 = RootLocation.AllUserConfig.GetFileName("full/path/to/test.file");

            Assert.IsTrue(FileSystem.PathEquals($"{user}/full/path/to/test.file", loc1));
            Assert.AreEqual(loc1, loc2);
            Assert.IsTrue(FileSystem.PathEquals($"{local}/full/path/to/test.file", loc3));
            Assert.AreEqual(loc3, loc4);
            Assert.IsTrue(FileSystem.PathEquals($"{machine}/full/path/to/test.file", loc5));
            Assert.AreEqual(loc5, loc6);
        }
    }

    #endregion Private Methods

    #region Public Methods

    [Test]
    public void TestDefaultLocations()
    {
        switch (Platform.Type)
        {
            default: throw new NotImplementedException();
            case PlatformType.Windows:
                TestDefaultLocationsWindows();
                break;
        }
    }

    #endregion Public Methods
}
