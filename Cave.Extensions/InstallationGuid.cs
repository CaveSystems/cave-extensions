using System;
using System.IO;
using System.Reflection;
#if !(NETSTANDARD2_0_OR_GREATER || NET50)
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
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
                        (AppDomain.CurrentDomain.BaseDirectory?.GetHashCode() ?? 0) ^
#else
                        AppContext.BaseDirectory.GetHashCode() ^
#endif
#if (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD1_6_OR_GREATER)
                        (0xDEAD << 32);
#else
                        ((Assembly.GetEntryAssembly()?.FullName.GetHashCode() ?? 0) << 32);
#endif               
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
#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET50
#else
                if (Platform.IsMicrosoft)
                {
#pragma warning disable CA1416
                    var software = Registry.LocalMachine.OpenSubKey(@"SOFTWARE");
                    if (software.GetValue("SystemGuid") is not byte[] data)
                    {
                        data = Guid.NewGuid().ToByteArray();
                        software.SetValue("SystemGuid", data, RegistryValueKind.Binary);
                    }
#pragma warning restore CA1416
                    return new(data);
                }
#endif
#if NETCOREAPP1_0 || NETCOREAPP1_1 || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
                var root = AppContext.BaseDirectory;
#else
                var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#endif
                var fileName = Path.Combine(root, "system.guid");
                if (!File.Exists(fileName))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    File.WriteAllText(fileName, Guid.NewGuid().ToString());
                }

                return new(File.ReadAllLines(fileName)[0]);
            }
        }

        #endregion
    }
}
