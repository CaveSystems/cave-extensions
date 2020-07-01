using System;
using System.Globalization;
using System.IO;

namespace Cave
{
    /// <summary>
    /// Provides access to a file location.
    /// </summary>
    public class FileLocation
    {
        /// <summary>Performs an implicit conversion from <see cref="FileLocation"/> to <see cref="string"/>.</summary>
        /// <param name="location">The location.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(FileLocation location) => location.ToString();

        /// <summary>
        /// Gets the program directory.
        /// </summary>
        static string ProgramDirectory => Path.GetDirectoryName(MainAssembly.Get().GetAssemblyFilePath());

        /// <summary>Gets or sets the root.</summary>
        /// <value>The root.</value>
        public RootLocation Root { get; set; }

        /// <summary>Gets or sets the name of the company.</summary>
        /// <value>The name of the company.</value>
        public string CompanyName { get; set; }

        /// <summary>Gets or sets the name of the file.</summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>Gets or sets the extension.</summary>
        /// <value>The extension.</value>
        public string Extension { get; set; }

        /// <summary>Gets or sets the sub folder to use.</summary>
        /// <value>The sub folder.</value>
        public string SubFolder { get; set; }

        /// <summary>Initializes a new instance of the <see cref="FileLocation" /> class.</summary>
        /// <param name="root">The root folder. Is unset, this will be set to roaming user.</param>
        /// <param name="companyName">Name of the company. If unset, this will be set to the assemblies company name.</param>
        /// <param name="subFolder">The sub folder.</param>
        /// <param name="fileName">Name of the file. If unset, this will be set to the assemblies product name.</param>
        /// <param name="extension">The extension.</param>
        public FileLocation(RootLocation root = RootLocation.Program, string companyName = null, string subFolder = null, string fileName = null, string extension = null)
        {
            Root = root;
            SubFolder = subFolder;
            Extension = extension;
            switch (Platform.Type)
            {
                case PlatformType.BSD:
                case PlatformType.Linux:
                case PlatformType.UnknownUnix:
                    FileName = fileName ?? AssemblyVersionInfo.Program.Product.ToLower(CultureInfo.InvariantCulture).ReplaceInvalidChars(ASCII.Strings.Letters + ASCII.Strings.Digits, "-");
                    break;
                default:
                    CompanyName = companyName ?? AssemblyVersionInfo.Program.Company.ReplaceChars(Path.GetInvalidPathChars(), "_");
                    FileName = fileName ?? AssemblyVersionInfo.Program.Product.ReplaceChars(Path.GetInvalidFileNameChars(), "_");
                    break;
            }
        }

        /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Root == RootLocation.Program
                ? FileSystem.Combine(GetRoot(), SubFolder, FileName + Extension)
                : FileSystem.Combine(GetRoot(), CompanyName, SubFolder, FileName + Extension);
        }

        /// <summary>Gets the folder.</summary>
        /// <value>The folder.</value>
        public string Folder
        {
            get
            {
                return Root == RootLocation.Program
                    ? FileSystem.Combine(GetRoot(), SubFolder)
                    : FileSystem.Combine(GetRoot(), CompanyName, SubFolder);
            }
        }

        string GetRoot()
        {
            switch (Platform.Type)
            {
                case PlatformType.Android: return GetRootAndroid(Root);
                case PlatformType.Windows:
                case PlatformType.Xbox:
                case PlatformType.CompactFramework: return GetRootWindows(Root);
                default: return GetRootUnix(Root);
            }
        }

        static string GetRootAndroid(RootLocation root)
        {
            switch (root)
            {
                case RootLocation.AllUserConfig:
                case RootLocation.AllUsersData:
                {
                    string path = FileSystem.Combine(ProgramDirectory, ".AllUsers");
                    Directory.CreateDirectory(path);
                    return path;
                }
                case RootLocation.LocalUserConfig:
                case RootLocation.LocalUserData:
                case RootLocation.RoamingUserConfig:
                case RootLocation.RoamingUserData:
                {
                    string path = FileSystem.Combine(ProgramDirectory, ".User_" + Environment.UserName);
                    Directory.CreateDirectory(path);
                    return path;
                }

                case RootLocation.Program: return ProgramDirectory;

                default: throw new ArgumentOutOfRangeException($"RootLocation {root} unknown");
            }
        }

        static string GetRootWindows(RootLocation root)
        {
            switch (root)
            {
                case RootLocation.AllUserConfig:
                case RootLocation.AllUsersData: return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

                case RootLocation.LocalUserConfig:
                case RootLocation.LocalUserData: return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                case RootLocation.RoamingUserConfig:
                case RootLocation.RoamingUserData: return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                case RootLocation.Program: return ProgramDirectory;

                default: throw new ArgumentOutOfRangeException($"RootLocation {root} unknown");
            }
        }

        static string GetRootUnix(RootLocation root)
        {
            switch (root)
            {
                case RootLocation.AllUserConfig: return "/etc";
                case RootLocation.AllUsersData: return "/var/lib";

                case RootLocation.LocalUserConfig:
                case RootLocation.LocalUserData: return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                case RootLocation.RoamingUserConfig:
                case RootLocation.RoamingUserData: return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                case RootLocation.Program: return ProgramDirectory;

                default: throw new ArgumentOutOfRangeException($"RootLocation {root} unknown");
            }
        }
    }
}
