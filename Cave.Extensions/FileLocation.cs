using System;
using System.IO;

namespace Cave;

/// <summary>Gets access to a file location.</summary>
public class FileLocation
{
    #region Static

    /// <summary>Performs an implicit conversion from <see cref="FileLocation" /> to <see cref="string" />.</summary>
    /// <param name="location">The location.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator string(FileLocation location) => location?.ToString();

    static string GetRootAndroid(RootLocation root)
    {
        switch (root)
        {
            case RootLocation.AllUserConfig:
            case RootLocation.AllUsersData:
            {
                var path = FileSystem.Combine(ProgramDirectory, ".AllUsers");
                Directory.CreateDirectory(path);
                return path;
            }
            case RootLocation.LocalUserConfig:
            case RootLocation.LocalUserData:
            case RootLocation.RoamingUserConfig:
            case RootLocation.RoamingUserData:
            {
                var path = FileSystem.Combine(ProgramDirectory, ".User_" + Environment.UserName);
                Directory.CreateDirectory(path);
                return path;
            }
            case RootLocation.Program: return ProgramDirectory;
            default: throw new ArgumentOutOfRangeException($"RootLocation {root} unknown");
        }
    }

    static string GetRootUnix(RootLocation root) => root switch
    {
        RootLocation.AllUserConfig => "/etc",
        RootLocation.AllUsersData => "/var/lib",
        RootLocation.LocalUserConfig or RootLocation.LocalUserData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        RootLocation.RoamingUserConfig or RootLocation.RoamingUserData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        RootLocation.Program => ProgramDirectory,
        _ => throw new ArgumentOutOfRangeException($"RootLocation {root} unknown")
    };

    static string GetRootWindows(RootLocation root) => root switch
    {
        RootLocation.AllUserConfig or RootLocation.AllUsersData => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        RootLocation.LocalUserConfig or RootLocation.LocalUserData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        RootLocation.RoamingUserConfig or RootLocation.RoamingUserData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        RootLocation.Program => ProgramDirectory,
        _ => throw new ArgumentOutOfRangeException($"RootLocation {root} unknown")
    };

    /// <summary>Gets the program directory.</summary>
    static string ProgramDirectory => Path.GetDirectoryName(MainAssembly.Get().GetAssemblyFilePath());

    #endregion

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="FileLocation" /> class.</summary>
    /// <param name="root">The root folder. Is unset, this will be set to roaming user.</param>
    /// <param name="companyName">Name of the company. If unset, this will be set to the assemblies company name.</param>
    /// <param name="subFolder">The sub folder.</param>
    /// <param name="fileName">Name of the file. If unset, this will be set to the assemblies product name.</param>
    /// <param name="extension">The extension.</param>
    public FileLocation(RootLocation root = RootLocation.Program, string companyName = null, string subFolder = null, string fileName = null,
        string extension = null)
    {
        Root = root;
        SubFolder = subFolder;
        Extension = extension;
        switch (Platform.Type)
        {
            case PlatformType.BSD:
            case PlatformType.Linux:
            case PlatformType.UnknownUnix:
                FileName = fileName ?? AssemblyVersionInfo.Program.Product.ToLowerInvariant()
                   .ReplaceInvalidChars(ASCII.Strings.Letters + ASCII.Strings.Digits, "-");
                break;
            default:
                CompanyName = companyName ?? AssemblyVersionInfo.Program.Company.ReplaceChars(Path.GetInvalidPathChars(), "_");
                FileName = fileName ?? AssemblyVersionInfo.Program.Product.ReplaceChars(Path.GetInvalidFileNameChars(), "_");
                break;
        }
    }

    #endregion

    #region Properties

    /// <summary>Gets the folder.</summary>
    /// <value>The folder.</value>
    public string Folder =>
        Root == RootLocation.Program
            ? FileSystem.Combine(GetRoot(), SubFolder)
            : FileSystem.Combine(GetRoot(), CompanyName, SubFolder);

    /// <summary>Gets or sets the name of the company.</summary>
    /// <value>The name of the company.</value>
    public string CompanyName { get; set; }

    /// <summary>Gets or sets the extension.</summary>
    /// <value>The extension.</value>
    public string Extension { get; set; }

    /// <summary>Gets or sets the name of the file.</summary>
    /// <value>The name of the file.</value>
    public string FileName { get; set; }

    /// <summary>Gets or sets the root.</summary>
    /// <value>The root.</value>
    public RootLocation Root { get; set; }

    /// <summary>Gets or sets the sub folder to use.</summary>
    /// <value>The sub folder.</value>
    public string SubFolder { get; set; }

    #endregion

    #region Overrides

    /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString() =>
        Root == RootLocation.Program
            ? FileSystem.Combine(GetRoot(), SubFolder, FileName + Extension)
            : FileSystem.Combine(GetRoot(), CompanyName, SubFolder, FileName + Extension);

    #endregion

    #region Members

    string GetRoot() => Platform.Type switch
    {
        PlatformType.Android => GetRootAndroid(Root),
        PlatformType.Windows or PlatformType.Xbox or PlatformType.CompactFramework => GetRootWindows(Root),
        _ => GetRootUnix(Root)
    };

    #endregion
}
