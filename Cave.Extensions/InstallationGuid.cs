using System;
using System.IO;

namespace Cave;

/// <summary>Gets an installation guid.</summary>
public static class InstallationGuid
{
    #region Private Fields

    static Guid? programGuid;

    #endregion Private Fields

    #region Public Properties

    /// <summary>Gets the installation identifier.</summary>
    /// <value>The installation identifier.</value>
    /// <exception cref="NotSupportedException">if <see cref="Guid.GetHashCode()"/> fails.</exception>
    public static Guid ProgramGuid
    {
        get
        {
            if (!programGuid.HasValue)
            {
                var guidBytes = SystemGuid.ToByteArray();

                var programLong = ((long)FileSystem.ProgramFileName.GetHashCode() << 32) | (long)MainAssembly.Get().GetName().FullName.GetHashCode();
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
            var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var fileName = Path.Combine(root, "system.guid");
            if (!File.Exists(fileName))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName) ?? throw new InvalidOperationException("Could not access localappdata!"));
                File.WriteAllText(fileName, Guid.NewGuid().ToString());
            }

            return new(File.ReadAllLines(fileName)[0]);
        }
    }

    #endregion Public Properties
}
