﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cave
{
    /// <summary>
    /// Provides filesystem functions.
    /// </summary>
    public static class FileSystem
    {
        /// <summary>
        /// Obtains the windows long path prefix.
        /// </summary>
        public const string WindowsLongPathPrefix = @"\\?\";

        /// <summary>
        /// Obtains the windows pysical drive prefix.
        /// </summary>
        public const string WindowsPysicalDrivePrefix = @"\\.\";

        static readonly char[] pathSeparatorChars = new char[] { '/', '\\' };
        static readonly char[] invalidChars = new char[] { '"', '&', '<', '>', '|', ':', '*', '?', };
        static string programFileName;

        /// <summary>Gets a regex for file/directory searching using default filesystem wildcards.</summary>
        /// <param name="fieldValue">The field value including wildcards.</param>
        /// <returns>Returns a new regex instance.</returns>
        public static Regex GetExpression(string fieldValue)
        {
            string valueString = fieldValue.ToString();
            bool lastWasWildcard = false;

            var sb = new StringBuilder();
            sb.Append('^');
            foreach (char c in valueString)
            {
                switch (c)
                {
                    case '*':
                    {
                        if (lastWasWildcard)
                        {
                            continue;
                        }

                        lastWasWildcard = true;
                        sb.Append(".*");
                        continue;
                    }
                    case '?':
                    {
                        sb.Append(".");
                        continue;
                    }
                    case ' ':
                    case '\\':
                    case '_':
                    case '+':
                    case '%':
                    case '|':
                    case '{':
                    case '[':
                    case '(':
                    case ')':
                    case '^':
                    case '$':
                    case '.':
                    case '#':
                    {
                        sb.Append('\\');
                        break;
                    }
                }
                sb.Append(c);
                lastWasWildcard = false;
            }
            sb.Append('$');
            string s = sb.ToString();
            return new Regex(s, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Gets all platform path separator chars.
        /// </summary>
        public static IList<char> PathSeparatorChars { get; } = new ReadOnlyCollection<char>(pathSeparatorChars);

        /// <summary>
        /// Gets invalid chars (in range 32..127) invalid for platform independent paths.
        /// </summary>
        public static IList<char> InvalidChars { get; } = new ReadOnlyCollection<char>(invalidChars);

        #region special paths

        /// <summary>
        /// Gets the full program fileName with path and extension.
        /// </summary>
        public static string ProgramFileName
        {
            get
            {
                if (programFileName == null)
                {
                    programFileName = GetFullPath(MainAssembly.Get().GetAssemblyFilePath());
                }
                return programFileName;
            }
        }

        /// <summary>
        /// Gets the program directory.
        /// </summary>
        public static string ProgramDirectory => Path.GetDirectoryName(ProgramFileName);

        /// <summary>
        /// Gets the program files base path (this may be process dependent on 64 bit os!).
        /// </summary>
        public static string ProgramFiles
        {
            get
            {
                string want = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                return Directory.Exists(want) ? want : Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
        }

        /// <summary>
        /// Gets the directory where the user stores his/her documents.
        /// </summary>
        public static string UserDocuments
        {
            get
            {
                string want = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return Directory.Exists(want) ? want : Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
        }

        /// <summary>
        /// Gets the directory where the user stores his/her roaming profile.
        /// </summary>
        public static string UserAppData
        {
            get
            {
                string want = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Directory.Exists(want) ? want : Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
        }

        /// <summary>
        /// Gets the directory where the application may store user and machine specific settings (no roaming).
        /// </summary>
        public static string LocalUserAppData
        {
            get
            {
                string want = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Directory.Exists(want) ? want : Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
        }

        /// <summary>
        /// Gets the directory where the application may store machine specific settings (no roaming).
        /// </summary>
        public static string LocalMachineAppData
        {
            get
            {
                string want = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                return Directory.Exists(want) ? want : Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
        }

        /// <summary>
        /// Gets the local machine configuration directory.
        /// </summary>
        public static string LocalMachineConfiguration
        {
            get
            {
                switch (Platform.Type)
                {
                    default:
                    case PlatformType.BSD:
                    case PlatformType.Linux:
                    case PlatformType.Solaris:
                    case PlatformType.UnknownUnix:
                        return "/etc/";

                    case PlatformType.Android:
                        return Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                    case PlatformType.Windows:
                    case PlatformType.CompactFramework:
                    case PlatformType.Xbox:
                        return LocalMachineAppData;
                }
            }
        }

        /// <summary>
        /// Gets the local user configuration directory.
        /// </summary>
        public static string LocalUserConfiguration => LocalUserAppData;

        /// <summary>
        /// Gets the configuration directory (this equals <see cref="UserAppData"/>).
        /// </summary>
        public static string UserConfiguration => UserAppData;

        #endregion

        /// <summary>
        /// Checks whether a path is a root of an other path.
        /// </summary>
        /// <param name="fullPath">The full path of the file / directory.</param>
        /// <param name="basePath">The base path of the file / directory.</param>
        /// <returns>A boolean value indicating if the fullpath is relative to the base path.</returns>
        public static bool IsRelative(string fullPath, string basePath)
        {
            if (fullPath == null)
            {
                throw new ArgumentNullException(nameof(fullPath));
            }

            if (basePath == null)
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            string[] fullCheck = fullPath.Split(pathSeparatorChars, StringSplitOptions.RemoveEmptyEntries);
            string[] baseCheck = basePath.Split(pathSeparatorChars, StringSplitOptions.RemoveEmptyEntries);
            if (fullCheck.Contains(".."))
            {
                throw new ArgumentException("FullPath may not contain relative path elements!");
            }

            if (baseCheck.Contains(".."))
            {
                throw new ArgumentException("BasePath may not contain relative path elements!");
            }

            StringComparison comparison = Platform.IsMicrosoft ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            if (baseCheck.Length > fullCheck.Length)
            {
                return false;
            }

            for (int i = 0; i < baseCheck.Length; i++)
            {
                if (!string.Equals(baseCheck[i], fullCheck[i], comparison))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns the absolute path for the specified path string.
        /// </summary>
        /// <param name="path">The file or directory for which to obtain absolute path information.</param>
        /// <returns>The fully qualified location of path.</returns>
        public static string GetFullPath(string path)
        {
            path = path.Replace('\\', '/');
            var parts = (IEnumerable<string>)path.Split('/');

            Stack<string> current;
            if (path.StartsWith("/"))
            {
                current = new Stack<string>();
                if (Platform.IsMicrosoft)
                {
                    current.Push(Path.GetPathRoot(Directory.GetCurrentDirectory()).Substring(0, 2));
                }
            }
            else if (Platform.IsMicrosoft)
            {
                var start = parts.FirstOrDefault() ?? string.Empty;
                if (start.Length == 2 && start[1] == ':')
                {
                    current = new Stack<string>(new[] { start });
                    parts = parts.Skip(1);
                }
                else
                {
                    current = new Stack<string>(Directory.GetCurrentDirectory().Split(pathSeparatorChars, StringSplitOptions.RemoveEmptyEntries));
                }
            }
            else
            {
                current = new Stack<string>(Directory.GetCurrentDirectory().Split(pathSeparatorChars, StringSplitOptions.RemoveEmptyEntries));
            }

            foreach (var part in parts)
            {
                switch (part)
                {
                    case "":
                    case ".": continue;
                    case "..":
                        current.Pop();
                        continue;
                    default:
                        current.Push(part);
                        continue;
                }
            }
            var result = current.Reverse().Join('/');
            if (path.EndsWith("/"))
            {
                result += "/";
            }
            if (Platform.IsMicrosoft)
            {
                return result.Replace('/', '\\');
            }
            else
            {
                return "/" + result;
            }
        }

        /// <summary>
        /// Obtains a relative path.
        /// </summary>
        /// <param name="fullPath">The full path of the file / directory.</param>
        /// <param name="basePath">The base path of the file / directory.</param>
        /// <returns>A new string containing the relative path.</returns>
        public static string GetRelative(string fullPath, string basePath)
        {
            if (fullPath == null)
            {
                throw new ArgumentNullException(nameof(fullPath));
            }

            if (basePath == null)
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            string[] relative = fullPath.Split(pathSeparatorChars, StringSplitOptions.RemoveEmptyEntries);
            string[] baseCheck = basePath.Split(pathSeparatorChars, StringSplitOptions.RemoveEmptyEntries);
            StringComparison comparison = Platform.IsMicrosoft ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            for (int i = 0; i < baseCheck.Length; i++)
            {
                if (!string.Equals(baseCheck[i], relative[i], comparison))
                {
                    throw new ArgumentException(string.Format("BasePath {0} is not a valid base for FullPath {1}!", basePath, fullPath));
                }
            }
            return "." + Path.DirectorySeparatorChar + string.Join(Path.DirectorySeparatorChar.ToString(), relative, baseCheck.Length, relative.Length - baseCheck.Length);
        }

        /// <summary>
        /// Touches (creates needed directories and creates/opens the file).
        /// </summary>
        /// <param name="fileName">The filename to touch.</param>
        public static void TouchFile(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite).Close();
        }

        /// <summary>
        /// Gets the parent path.
        /// </summary>
        /// <remarks>
        /// This function supports long paths.
        /// </remarks>
        /// <param name="path">The path.</param>
        /// <returns>The parent path.</returns>
        public static string GetParent(string path) => Combine(path, "..");

        /// <summary>Combines multiple paths starting with the current directory.</summary>
        /// <param name="paths">The paths.</param>
        /// <returns>The combined paths.</returns>
        /// <remarks>This function supports long paths.</remarks>
        public static string Combine(params string[] paths) => Combine(Path.DirectorySeparatorChar, paths);

        /// <summary>Combines multiple paths starting with the current directory.</summary>
        /// <param name="pathSeparator">The path separator.</param>
        /// <param name="paths">The paths.</param>
        /// <returns>The combined paths.</returns>
        /// <remarks>This function supports long paths.</remarks>
        public static string Combine(char pathSeparator, params string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            string root = null;
            char separator = pathSeparator;

            var resultParts = new LinkedList<string>();
            foreach (string s in paths)
            {
                string path = s;
                if (path == null)
                {
                    continue;
                }

                if (path.Length < 1)
                {
                    continue;
                }

                #region handle rooted paths
                if (path.Contains("://"))
                {
                    separator = '/';
                    path = s.AfterFirst("://");
                    root = s.Substring(0, s.Length - path.Length);
                }
                else
                {
                    switch (path[0])
                    {
                        case '/':
                        case '\\':
                        {// rooted:
                            resultParts.Clear();
                            separator = pathSeparator;
                            root = $"{separator}";
                            break;
                        }
                        default:
                        {
                            if (path.Length >= 2)
                            {
                                if (Platform.IsMicrosoft && path[1] == ':')
                                {
                                    separator = pathSeparator;
                                    resultParts.Clear();
                                    root = path.Substring(0, 2) + separator;
                                    path = path.Substring(2).TrimStart(pathSeparatorChars);
                                }
                            }
                            break;
                        }
                    }
                }
                #endregion

                if (Platform.IsMicrosoft)
                {
                    if (path.StartsWith(WindowsLongPathPrefix))
                    {
                        separator = '\\';
                        resultParts.Clear();
                        root = WindowsLongPathPrefix;
                        path = path.Substring(WindowsLongPathPrefix.Length);
                    }
                    else if (path.StartsWith(WindowsPysicalDrivePrefix))
                    {
                        separator = '\\';
                        resultParts.Clear();
                        root = WindowsPysicalDrivePrefix;
                        path = path.Substring(WindowsPysicalDrivePrefix.Length);
                    }
                }
                string[] parts = path.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
                {
                    if (part == "?")
                    {
                        throw new ArgumentException(string.Format("Invalid path {0}", path), nameof(paths));
                    }

                    if (part == ".")
                    {
                        continue;
                    }

                    if (part == "..")
                    {
                        if (resultParts.Count > 0)
                        {
                            resultParts.RemoveLast();
                            continue;
                        }
                        else
                        {
                            // path is now relative
                            root = null;
                        }
                    }
                    resultParts.AddLast(part);
                }
            }
            if (resultParts.Count == 0)
            {
                return root ?? ".";
            }
            string result = string.Join(separator.ToString(), resultParts.ToArray());
            return root + result;
        }

        /// <summary>
        /// Finds all files that match the criteria specified at the FileMaskList.
        /// The FileMaskList may contain absolute and relative pathss, filenames or masks and the "|r", ":r" recurse subdirectories switch.
        /// </summary>
        /// <param name="fileMask">The file mask.</param>
        /// <param name="mainPath">main path to begin relative searches.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns>A new list of files.</returns>
        /// <exception cref="ArgumentNullException">Thrown if fileMaskList is empty.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown if the mainPath cannot be found.</exception>
        /// <example>
        /// @"c:\somepath\somefile*.ext", @"/absolute/path/file.ext", @"./sub/*.*", @"*.cs|r", @"./somepath/file.ext|r".
        /// </example>
        public static ICollection<FileItem> FindFiles(string fileMask, string mainPath = null, bool recursive = false) => FindFiles(new string[] { fileMask }, mainPath, recursive);

        /// <summary>
        /// Finds all files that match the criteria specified at the FileMaskList.
        /// The FileMaskList may contain absolute and relative pathss, filenames or masks and the "|r", ":r" recurse subdirectories switch.
        /// </summary>
        /// <param name="fileMaskList">The file mask list.</param>
        /// <param name="mainPath">main path to begin relative searches.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns>A new list of files.</returns>
        /// <exception cref="ArgumentNullException">Thrown if fileMaskList is empty.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown if the mainPath cannot be found.</exception>
        /// <example>
        /// @"c:\somepath\somefile*.ext", @"/absolute/path/file.ext", @"./sub/*.*", @"*.cs|r", @"./somepath/file.ext|r".
        /// </example>
        public static IList<FileItem> FindFiles(IEnumerable<string> fileMaskList, string mainPath = null, bool recursive = false)
        {
            if (fileMaskList == null)
            {
                throw new ArgumentNullException(nameof(fileMaskList));
            }

            if (mainPath != null)
            {
                mainPath = GetFullPath(mainPath);
            }

            var result = new List<FileItem>();
            foreach (string fileMask in fileMaskList)
            {
                try
                {
                    SearchOption searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    string mask = fileMask.ToString();
                    if (mask.EndsWith("|r") || mask.EndsWith(":r"))
                    {
                        mask = mask.Substring(0, mask.Length - 2);
                        searchOption = SearchOption.AllDirectories;
                    }
                    string path = ".";
                    if (mask.IndexOfAny(pathSeparatorChars) > -1)
                    {
                        path = Path.GetDirectoryName(mask);
                        mask = Path.GetFileName(mask);
                    }
                    if (mainPath != null)
                    {
                        path = Combine(mainPath, path);
                    }
                    path = GetFullPath(path);
                    if (!Directory.Exists(path))
                    {
                        continue;
                    }

                    foreach (string f in Directory.GetFiles(path, mask, searchOption))
                    {
                        if (mainPath != null)
                        {
                            result.Add(FileItem.FromFullPath(mainPath, f));
                        }
                        else
                        {
                            result.Add(FileItem.FromFullPath(path, f));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new DirectoryNotFoundException(string.Format("Error while trying to resolve '{0}'.", fileMask), ex);
                }
            }
            return result;
        }

        /// <summary>
        /// Finds all files that match the criteria specified at the FileMaskList.
        /// The FileMaskList may contain absolute and relative paths, filenames or masks and the "|r" recurse subdirectories switch.
        /// </summary>
        /// <param name="directoryMaskList">The mask to apply.</param>
        /// <param name="mainPath">main path to begin relative searches.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns>A new list of directories.</returns>
        /// <exception cref="ArgumentNullException">Thrown if directoryMaskList is empty.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown if the mainPath cannot be found.</exception>
        /// <example>
        /// @"c:\somepath\*", @"/absolute/path/dir*", @"./sub/*", @"*|r", @"./somepath/file.ext|r".
        /// </example>
        public static ICollection<DirectoryItem> FindDirectories(IEnumerable<string> directoryMaskList, string mainPath = null, bool recursive = false)
        {
            if (directoryMaskList == null)
            {
                throw new ArgumentNullException(nameof(directoryMaskList));
            }

            if (mainPath != null)
            {
                mainPath = GetFullPath(mainPath);
            }

            var result = new List<DirectoryItem>();
            foreach (string dir in directoryMaskList)
            {
                try
                {
                    SearchOption searchOption = SearchOption.TopDirectoryOnly;
                    string mask = dir;
                    if (mask.EndsWith("|r") || mask.EndsWith(":r") || recursive)
                    {
                        mask = mask.Substring(0, mask.Length - 2);
                        searchOption = SearchOption.AllDirectories;
                    }
                    string path = ".";
                    if (!string.IsNullOrEmpty(mask))
                    {
                        path = Path.GetDirectoryName(mask);
                        mask = Path.GetFileName(mask);
                    }
                    if (string.IsNullOrEmpty(mask))
                    {
                        mask = "*";
                    }

                    string basePath = mainPath ?? GetFullPath(path);
                    path = GetFullPath(Combine(basePath, path));

                    if (!Directory.Exists(path))
                    {
                        continue;
                    }

                    foreach (string directory in Directory.GetDirectories(path, mask, searchOption))
                    {
                        result.Add(DirectoryItem.FromFullPath(basePath, directory));
                    }
                    if (string.IsNullOrEmpty(mask))
                    {
                        var directory = DirectoryItem.FromFullPath(basePath, path);
                        if (!result.Contains(directory))
                        {
                            result.Add(directory);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new DirectoryNotFoundException(string.Format("Error while trying to resolve '{0}'.", dir), ex);
                }
            }
            return result;
        }

        /// <summary>
        /// Obtains a list of relative <see cref="FileItem"/>s from a list of Paths.
        /// </summary>
        /// <param name="basePath">The parent or base path to use.</param>
        /// <param name="paths">The full paths relative to the base path.</param>
        /// <returns>Returns a list of file items.</returns>
        public static ICollection<FileItem> GetRelativeFiles(string basePath, IEnumerable<string> paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            var result = new List<FileItem>();
            foreach (string path in paths)
            {
                result.Add(FileItem.FromFullPath(basePath, path));
            }
            return result;
        }

        /// <summary>
        /// Creates a new temporary directory at the users temp path and returns the full path.
        /// </summary>
        /// <returns>Returns the full path to a new temporary file.</returns>
        public static string CreateNewTempDirectory()
        {
            string basePath = Path.GetTempPath();
            int number = Environment.TickCount;
            while (Directory.Exists(Combine(basePath, (++number).ToString())))
            {
            }

            string result = Combine(basePath, number.ToString());
            Directory.CreateDirectory(result);
            return result;
        }

        /// <summary>
        /// Remove any path root present in the path.
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing path information.</param>
        /// <returns>The path with the root removed if it was present; path otherwise.</returns>
        /// <remarks>Unlike the <see cref="System.IO.Path"/> class the path isnt otherwise checked for validity.</remarks>
        public static string DropRoot(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            string result = path;
            if (Platform.IsMicrosoft && result.StartsWith(WindowsLongPathPrefix))
            {
                result = result.Substring(WindowsLongPathPrefix.Length);
            }

            if ((path[0] == '\\') || (path[0] == '/'))
            {
                // UNC name ?
                if ((path.Length > 1) && ((path[1] == '\\') || (path[1] == '/')))
                {
                    int index = 2;
                    int elements = 2;

                    // Scan for two separate elements \\machine\share\restofpath
                    while ((index <= path.Length) &&
                        (((path[index] != '\\') && (path[index] != '/')) || (--elements > 0)))
                    {
                        index++;
                    }

                    index++;
                    result = index < path.Length ? path.Substring(index) : string.Empty;
                }
            }
            else if ((path.Length > 1) && (path[1] == ':'))
            {
                int dropCount = 2;
                if ((path.Length > 2) && ((path[2] == '\\') || (path[2] == '/')))
                {
                    dropCount = 3;
                }
                result = result.Remove(0, dropCount);
            }
            return result;
        }

        /// <summary>Splits the specified full path.</summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns>Returns a list of path parts.</returns>
        public static IList<string> SplitPath(string fullPath)
        {
            if (fullPath == null)
            {
                return new string[0];
            }

            string[] parts = fullPath.SplitKeepSeparators('\\', '/');
            string root = null;
            var folders = new List<string>();
            for (int i = 0; i < parts.Length; i++)
            {
                if (root == null)
                {
                    root = parts[i];
                    if (parts[i].Contains(":"))
                    {
                        if (!Platform.IsMicrosoft || root.Length > 1)
                        {
                            root += "//";
                            while (parts[i] == "/" || parts[i] == "\\")
                            {
                                ++i;
                            }
                        }
                        else if (Platform.IsMicrosoft)
                        {
                            root += "\\";
                            while (parts[i] == "/" || parts[i] == "\\")
                            {
                                ++i;
                            }
                        }
                    }
                    folders.Add(root);
                    continue;
                }
                if (parts[i] == "\\" || parts[i] == "/")
                {
                    continue;
                }

                folders.Add(parts[i]);
            }
            return folders;
        }

        /// <summary>
        /// Tries to delete a file or directory and remove empty parent directories.
        /// </summary>
        /// <param name="path">The name of the file / directory to remove.</param>
        /// <param name="recursive">Remove all subdirectories or files.</param>
        /// <returns>Returns true on success.</returns>
        public static bool TryDeleteDirectory(string path, bool recursive = false)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    return false;
                }
            }
            else if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, recursive);
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return true;
            }

            TryDeleteDirectory(Path.GetDirectoryName(path), false);
            return true;
        }

        /// <summary>
        /// Returns the names of all files and subdirectories that meet specified criteria.
        /// </summary>
        /// <param name="path">The relative or absolute path to the directory to search. This string is not case-sensitive.</param>
        /// <param name="searchPattern">The search string to match against the names of subdirectories in path. This parameter can contain a combination of valid literal and wildcard characters, but it doesn't support regular expressions.</param>
        /// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories. The default value is TopDirectoryOnly.</param>
        /// <returns>An array of file the file names and directory names that match the specified search criteria, or an empty array if no files or directories are found.</returns>
#if NET35 || NET20
        public static string[] GetFileSystemEntries(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var results = new List<string>();
            results.AddRange(Directory.GetDirectories(path, "*", searchOption));
            results.AddRange(Directory.GetFiles(path, searchPattern, searchOption));
            return results.ToArray();
        }
#else
        public static string[] GetFileSystemEntries(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly) => Directory.GetFileSystemEntries(path, searchPattern, searchOption);
#endif

        /// <summary>
        /// Gets the size, in bytes, of the specified file.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        /// <returns>The size of the current file in bytes.</returns>
        public static long GetSize(string fileName) => new FileInfo(fileName).Length;

        /// <summary>
        /// Returns the date and time the specified file or directory was last written to.
        /// </summary>
        /// <param name="filesystemEntry">The file or directory for which to obtain write date and time information.</param>
        /// <returns>A DateTime structure set to the date and time that the specified file or directory was last written to. This value is expressed in local time.</returns>
        public static DateTime GetLastWriteTime(string filesystemEntry)
        {
            if (Directory.Exists(filesystemEntry))
            {
                return Directory.GetLastWriteTime(filesystemEntry);
            }
            if (File.Exists(filesystemEntry))
            {
                return File.GetLastWriteTime(filesystemEntry);
            }
            throw new FileNotFoundException();
        }

        /// <summary>
        /// Returns the date and time the specified file or directory was last written to.
        /// </summary>
        /// <param name="filesystemEntry">The file or directory for which to obtain write date and time information.</param>
        /// <returns>A DateTime structure set to the date and time that the specified file or directory was last written to. This value is expressed in utc time.</returns>
        public static DateTime GetLastWriteTimeUtc(string filesystemEntry)
        {
            if (Directory.Exists(filesystemEntry))
            {
                return Directory.GetLastWriteTimeUtc(filesystemEntry);
            }
            if (File.Exists(filesystemEntry))
            {
                return File.GetLastWriteTimeUtc(filesystemEntry);
            }
            throw new FileNotFoundException();
        }

        /// <summary>
        /// Returns the creation date and time of the specified file or directory.
        /// </summary>
        /// <param name="filesystemEntry">The file or directory for which to obtain creation date and time information.</param>
        /// <returns>A DateTime structure set to the creation date and time for the specified file or directory. This value is expressed in local time.</returns>
        public static DateTime GetCreationTime(string filesystemEntry)
        {
            if (Directory.Exists(filesystemEntry))
            {
                return Directory.GetCreationTime(filesystemEntry);
            }
            if (File.Exists(filesystemEntry))
            {
                return File.GetCreationTime(filesystemEntry);
            }
            throw new FileNotFoundException();
        }

        /// <summary>
        /// Returns the creation date and time of the specified file or directory.
        /// </summary>
        /// <param name="filesystemEntry">The file or directory for which to obtain creation date and time information.</param>
        /// <returns>A DateTime structure set to the creation date and time for the specified file or directory. This value is expressed in utc time.</returns>
        public static DateTime GetCreationTimeUtc(string filesystemEntry)
        {
            if (Directory.Exists(filesystemEntry))
            {
                return Directory.GetCreationTimeUtc(filesystemEntry);
            }
            if (File.Exists(filesystemEntry))
            {
                return File.GetCreationTimeUtc(filesystemEntry);
            }
            throw new FileNotFoundException();
        }

        /// <summary>
        /// Returns the date and time the specified file or directory was last accessed.
        /// </summary>
        /// <param name="filesystemEntry">The file or directory for which to obtain creation date and time information.</param>
        /// <returns>A DateTime structure set to the date and time that the specified file or directory was last accessed. This value is expressed in local time.</returns>
        public static DateTime GetLastAccessTime(string filesystemEntry)
        {
            if (Directory.Exists(filesystemEntry))
            {
                return Directory.GetLastAccessTime(filesystemEntry);
            }
            if (File.Exists(filesystemEntry))
            {
                return File.GetLastAccessTime(filesystemEntry);
            }
            throw new FileNotFoundException();
        }

        /// <summary>
        /// Returns the date and time the specified file or directory was last accessed.
        /// </summary>
        /// <param name="filesystemEntry">The file or directory for which to obtain creation date and time information.</param>
        /// <returns>A DateTime structure set to the date and time that the specified file or directory was last accessed. This value is expressed in utc time.</returns>
        public static DateTime GetLastAccessTimeUtc(string filesystemEntry)
        {
            if (Directory.Exists(filesystemEntry))
            {
                return Directory.GetLastAccessTimeUtc(filesystemEntry);
            }
            if (File.Exists(filesystemEntry))
            {
                return File.GetLastAccessTimeUtc(filesystemEntry);
            }
            throw new FileNotFoundException();
        }
    }
}
