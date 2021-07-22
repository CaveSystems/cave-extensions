using System;
using System.IO;
using System.Reflection;

#if !NETSTANDARD20
using Microsoft.Win32;
#endif

namespace Cave
{
    /// <summary>Gets an installation guid.</summary>
    public static class InstallationGuid
    {
        #region Static

        static Guid? programGuid;

        /// <summary>Gets the installation identifier.</summary>
        /// <value>The installation identifier.</value>
        /// <exception cref="NotSupportedException">if <see cref="Guid.GetHashCode()" /> failes.</exception>
        public static Guid ProgramGuid
        {
            get
            {
                if (!programGuid.HasValue)
                {
                    var guidBytes = SystemGuid.ToByteArray();
                    long programLong =
                        (AppDomain.CurrentDomain.BaseDirectory?.GetHashCode() ?? 0) ^
                        ((Assembly.GetEntryAssembly()?.FullName.GetHashCode() ?? 0) << 32);
                    var programBytes = BitConverter.GetBytes(programLong);
                    for (var i = 0; i < 8; i++)
                    {
                        guidBytes[guidBytes.Length - i - 1] ^= programBytes[i];
                    }

                    programGuid = new Guid(guidBytes);
                }

                return programGuid.Value;
            }
        }

        /// <summary>Gets the installation unique identifier.</summary>
        /// <returns>unique id as GUID.</returns>
        public static Guid SystemGuid
        {
            get
            {
#if NETSTANDARD20
#else
                if (Platform.IsMicrosoft)
                {
                    var software = Registry.LocalMachine.OpenSubKey(@"SOFTWARE");
                    if (software.GetValue("SystemGuid") is not byte[] data)
                    {
                        data = Guid.NewGuid().ToByteArray();
                        software.SetValue("SystemGuid", data, RegistryValueKind.Binary);
                    }

                    return new Guid(data);
                }
#endif
                var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var fileName = Path.Combine(root, "system.guid");
                if (!File.Exists(fileName))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    File.WriteAllText(fileName, Guid.NewGuid().ToString());
                }

                return new Guid(File.ReadAllLines(fileName)[0]);
            }
        }

        #endregion
    }
}
