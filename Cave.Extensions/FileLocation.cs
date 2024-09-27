using System;
using System.IO;

namespace Cave;

/// <summary>Gets access to a file location.</summary>
public class FileLocation : IEquatable<FileLocation>
{
    #region Private Fields

    static string? programDirectory;

    #endregion Private Fields

    #region Private Properties

    /// <summary>Gets the program directory.</summary>
    static string ProgramDirectory => programDirectory ??= Path.GetDirectoryName(MainAssembly.Get().GetAssemblyFilePath()) ?? Directory.GetCurrentDirectory();

    #endregion Private Properties

    #region Private Methods

    static string GetRootAndroid(RootLocation root)
    {
        switch (root)
        {
            case RootLocation.AllUserConfig:
            case RootLocation.AllUsersData:
            {
                var path = FileSystem.Combine(ProgramDirectory, ".AllUsers");
                _ = Directory.CreateDirectory(path);
                return path;
            }
            case RootLocation.LocalUserConfig:
            case RootLocation.LocalUserData:
            case RootLocation.RoamingUserConfig:
            case RootLocation.RoamingUserData:
            {
                var path = FileSystem.Combine(ProgramDirectory, ".User_" + Environment.UserName);
                _ = Directory.CreateDirectory(path);
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

    string GetRoot() => Platform.Type switch
    {
        PlatformType.Android => GetRootAndroid(Root),
        PlatformType.Windows or PlatformType.Xbox or PlatformType.CompactFramework => GetRootWindows(Root),
        _ => GetRootUnix(Root)
    };

    #endregion Private Methods

    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="FileLocation"/> class.</summary>
    /// <param name="root">The root folder. Is unset, this will be set to roaming user.</param>
    /// <param name="subFolders">The sub folders.</param>
    /// <param name="fileName">Name of the file. If unset, this will be set to the assemblies product name.</param>
    /// <param name="extension">The extension.</param>
    public FileLocation(RootLocation root, string? subFolders, string? fileName, string? extension)
    {
        if (extension is not null && !extension.StartsWith('.')) throw new ArgumentOutOfRangeException(nameof(extension), "Extension needs to start with a point!");
        Root = root;
        SubFolders = subFolders;
        Extension = extension;
        FileName = fileName;
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets or sets the valid characters used at auto generated file locations (from assemblies product and company name).</summary>
    public static string ValidChars { get; set; } = ASCII.Strings.Letters + ASCII.Strings.Digits;

    /// <summary>Gets or sets the extension.</summary>
    /// <value>The extension.</value>
    public string? Extension { get; init; }

    /// <summary>Gets or sets the name of the file.</summary>
    /// <value>The name of the file.</value>
    public string? FileName { get; init; }

    /// <summary>Gets the folder.</summary>
    /// <value>The folder.</value>
    public string Folder => FileSystem.Combine(GetRoot(), SubFolders);

    /// <summary>Gets or sets the root.</summary>
    /// <value>The root.</value>
    public RootLocation Root { get; init; }

    /// <summary>Gets or sets the sub folders to use.</summary>
    /// <value>The sub folders.</value>
    public string? SubFolders { get; init; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Creates the default location used to store files.</summary>
    /// <remarks>See <see cref="ValidChars"/> for name conversion.</remarks>
    /// <param name="root">Root location to use</param>
    /// <param name="companyName">Company name to use. This will default to the current assemblies <see cref="AssemblyVersionInfo.Company"/>.</param>
    /// <param name="productName">Product name to use. This will default to the current assemblies <see cref="AssemblyVersionInfo.Product"/>.</param>
    /// <param name="fileNameWithExtension">Filename and extension to use</param>
    /// <returns>Returns a new <see cref="FileLocation"/> instance</returns>
    public static FileLocation Create(RootLocation root = RootLocation.LocalUserConfig, string? companyName = null, string? productName = null, string? fileNameWithExtension = null) => Create(root, companyName, productName, fileNameWithExtension is null ? null : Path.GetFileNameWithoutExtension(fileNameWithExtension), fileNameWithExtension is null ? null : Path.GetExtension(fileNameWithExtension));

    /// <summary>Creates the default location used to store files.</summary>
    /// <remarks>See <see cref="ValidChars"/> for name conversion.</remarks>
    /// <param name="root">Root location to use</param>
    /// <param name="companyName">Company name to use. This will default to the current assemblies <see cref="AssemblyVersionInfo.Company"/>.</param>
    /// <param name="productName">Product name to use. This will default to the current assemblies <see cref="AssemblyVersionInfo.Product"/>.</param>
    /// <param name="fileName">Filename to use.</param>
    /// <param name="extension">Extzension to use.</param>
    /// <returns>Returns a new <see cref="FileLocation"/> instance</returns>
    public static FileLocation Create(RootLocation root = RootLocation.LocalUserConfig, string? companyName = null, string? productName = null, string? fileName = null, string? extension = null)
    {
        var fileNameWithExtension = fileName is null && extension is null ? null : $"{fileName}{extension}";
        return Create(root: root, subFolders: FileSystem.Combine(companyName, productName), fileNameWithExtension: fileNameWithExtension);
    }

    /// <summary>Creates the default location used to store files.</summary>
    /// <remarks>See <see cref="ValidChars"/> for name conversion.</remarks>
    /// <param name="root">Root location to use</param>
    /// <param name="subFolders">The sub folders.</param>
    /// <param name="fileNameWithExtension">Filename and extension to use</param>
    /// <returns>Returns a new <see cref="FileLocation"/> instance</returns>
    public static FileLocation Create(RootLocation root = RootLocation.LocalUserConfig, string? subFolders = null, string? fileNameWithExtension = null)
        => new(root: root, subFolders: subFolders, fileName: fileNameWithExtension is null ? null : Path.GetFileNameWithoutExtension(fileNameWithExtension), extension: fileNameWithExtension is null ? null : Path.GetExtension(fileNameWithExtension));

    /// <summary>Creates the default location used to store files for the current assembly.</summary>
    /// <remarks>See <see cref="ValidChars"/> for name conversion.</remarks>
    /// <param name="root">Root location to use</param>
    /// <param name="fileNameWithExtension">Filename and extension to use</param>
    /// <returns>Returns a new <see cref="FileLocation"/> instance</returns>
    public static FileLocation CreateDefault(RootLocation root = RootLocation.LocalUserConfig, string? fileNameWithExtension = null)
    {
        var asm = AssemblyVersionInfo.Program;
        var folder = FileSystem.Combine(
            asm.Company.ReplaceInvalidChars(ValidChars, "/").Split('/').JoinPascalCase(),
            asm.Product.ReplaceInvalidChars(ValidChars, "/").Split('/').JoinPascalCase());
        return Create(root, folder, fileNameWithExtension);
    }

    /// <summary>Performs an implicit conversion from <see cref="FileLocation"/> to <see cref="string"/>.</summary>
    /// <param name="location">The location.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator string(FileLocation location) => location?.ToString() ?? string.Empty;

    /// <inheritdoc/>
    public bool Equals(FileLocation? other) => other is not null && FileSystem.PathEquals(this, other);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is FileLocation fileLocation && Equals(fileLocation);

    /// <inheritdoc/>
    public override int GetHashCode() => ToString().GetHashCode();

    /// <summary>Returns a <see cref="string"/> that represents this instance.</summary>
    /// <returns>A <see cref="string"/> that represents this instance.</returns>
    public override string ToString() => FileSystem.Combine(GetRoot(), SubFolders, FileName + Extension);

    #endregion Public Methods
}
