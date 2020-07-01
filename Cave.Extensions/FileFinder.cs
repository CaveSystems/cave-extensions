﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cave
{
    /// <summary>
    /// Provides an asynchronous file finder.
    /// </summary>
    public sealed class FileFinder : IDisposable
    {
        readonly LinkedList<FileItem> fileList = new LinkedList<FileItem>();
        readonly LinkedList<string> directoryList = new LinkedList<string>();
        string baseDirectory = ".";
        string fileMask;
        string directoryMask;
        Task[] tasks;

        void SearchFiles()
        {
            var callback = FoundFile;
            bool useComparer = Comparer.Count > 0;
            while (true)
            {
                string currentDir;
                lock (directoryList)
                {
                    while (directoryList.Count == 0)
                    {
                        if (!DirectorySearchRunning)
                        {
                            FileSearchRunning = false;
                            Verbose($"FileFinder completed.");
                            return;
                        }
                        Monitor.Wait(directoryList);
                    }
                    currentDir = directoryList.First.Value;
                    directoryList.RemoveFirst();
                    DirectoriesRead++;
                }

                Verbose($"FileFinder retrieving files at {currentDir}.");
                string[] files;
                try
                {
                    files = FileMask == null ? Directory.GetFiles(currentDir) : Directory.GetFiles(currentDir, FileMask);
                }
                catch (Exception ex)
                {
                    Error?.Invoke(this, new ErrorEventArgs(ex));
                    Verbose($"FileFinder got an error reading {currentDir}: {ex.Message}", true);
                    continue;
                }
                foreach (string fileName in files)
                {
                    var file = FileItem.FromFullPath(BaseDirectory, fileName);
                    Verbose($"FileFinder found file {file}");

                    if (useComparer)
                    {
                        foreach (IFileFinderComparer comparer in Comparer)
                        {
                            if (!comparer.FileMatches(file))
                            {
                                continue;
                            }
                        }
                    }

                    if (callback != null)
                    {
                        var args = new FileItemEventArgs(file);
                        callback.Invoke(this, args);
                        if (args.Handled)
                        {
                            continue;
                        }
                    }

                    while (FileSearchRunning)
                    {
                        lock (fileList)
                        {
                            if ((MaximumFilesQueued <= 0) || (fileList.Count < MaximumFilesQueued))
                            {
                                FilesSeen++;
                                fileList.AddLast(file);
                                Monitor.Pulse(fileList);
                                break;
                            }
                            else
                            {
                                Monitor.Wait(fileList);
                            }
                        }
                    }
                }
            }
            throw new Exception("THIS SHOULD NEVER HAPPEN");
        }

        void SearchDirectories()
        {
            lock (directoryList)
            {
                directoryList.AddLast(BaseDirectory);
                Monitor.Pulse(directoryList);
            }

            var callback = FoundDirectory;
            var list = new LinkedList<string>();
            list.AddFirst(BaseDirectory);
            while (list.Count > 0)
            {
                string currentDir = list.First.Value;
                list.RemoveFirst();

                Verbose($"FileFinder entering directory {currentDir}.");
                string[] dirs;
                try
                {
                    dirs = DirectoryMask == null ? Directory.GetDirectories(currentDir) : Directory.GetDirectories(currentDir, DirectoryMask);
                }
                catch (Exception ex)
                {
                    Error?.Invoke(this, new ErrorEventArgs(ex));
                    Verbose($"FileFinder got an error reading {currentDir}: {ex.Message}", true);
                    continue;
                }

                if (callback != null)
                {
                    var argList = dirs.Select(dir => new DirectoryItemEventArgs(DirectoryItem.FromFullPath(BaseDirectory, dir)));
                    foreach (var arg in argList)
                    {
                        callback.Invoke(this, arg);
                    }
                    dirs = argList.Where(a => !a.Handled).Select(a => a.Directory.ToString()).ToArray();
                }

                DirectoriesSeen += dirs.Length;
                lock (directoryList)
                {
                    foreach (string dir in dirs)
                    {
                        list.AddLast(dir);
                        directoryList.AddLast(dir);
                    }
                    Monitor.Pulse(directoryList);
                }
            }
            DirectorySearchRunning = false;
            Verbose($"FileFinder completed directory listing.");
        }

        /// <summary>
        /// Called on each error
        /// </summary>
        public event EventHandler<ErrorEventArgs> Error;

        /// <summary>The found file event</summary>
        public event EventHandler<FileItemEventArgs> FoundFile;

        /// <summary>The found directory event</summary>
        public event EventHandler<DirectoryItemEventArgs> FoundDirectory;

        /// <summary>
        /// Gets or sets the maximum number of files in queue.
        /// </summary>
        public int MaximumFilesQueued { get; set; }

        /// <summary>Gets a value indicating whether [the directory search is running].</summary>
        /// <value>
        /// <c>true</c> if [directory search is running]; otherwise, <c>false</c>.
        /// </value>
        public bool DirectorySearchRunning { get; private set; }

        /// <summary>Gets a value indicating whether [the file search is running].</summary>
        /// <value><c>true</c> if [file search is running]; otherwise, <c>false</c>.</value>
        public bool FileSearchRunning { get; private set; }

        /// <summary>
        /// Gets the number of files seen while searching.
        /// </summary>
        public int FilesSeen { get; private set; }

        /// <summary>
        /// Gets the number of files read.
        /// </summary>
        public int FilesRead { get; private set; }

        /// <summary>
        /// Gets the number of directories seen while searching.
        /// </summary>
        public int DirectoriesSeen { get; private set; }

        /// <summary>
        /// Gets the number of directories read.
        /// </summary>
        public int DirectoriesRead { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether logging of messages to <see cref="Trace"/> output is enabled.
        /// </summary>
        public bool EnableTrace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether logging of messages to <see cref="Debug"/> output is enabled.
        /// </summary>
        public bool EnableDebug { get; set; }

        float progress;

        /// <summary>
        /// Gets the current progress (search and reading).
        /// This is a very rough estimation.
        /// </summary>
        public float Progress
        {
            get
            {
                var dirProgress = DirectoriesRead / (DirectoriesSeen + 1f);
                var fileProgress = FilesRead / (FilesSeen + 1f);
                var newProgress = fileProgress * Math.Min(fileProgress, dirProgress);

                if (newProgress > progress)
                {
                    progress = newProgress;
                }

                return progress;
            }
        }

        /// <summary>
        /// Retrieves (dequeues) all files already found. This may called repeatedly until Completed==true.
        /// </summary>
        /// <param name="wait">Wait until at least one file was found.</param>
        /// <param name="maximum">Maximum number of items to return.</param>
        /// <returns>Returns an array of files.</returns>
        public IList<FileItem> Get(bool wait = false, int maximum = 0)
        {
            lock (fileList)
            {
                if (wait)
                {
                    while (fileList.Count == 0 && FileSearchRunning)
                    {
                        Monitor.Wait(fileList);
                    }
                }
                if (maximum > 0)
                {
                    if (fileList.Count < maximum)
                    {
                        maximum = fileList.Count;
                    }
                    var result = new List<FileItem>(maximum);
                    for (int i = 0; i < maximum; i++)
                    {
                        result.Add(fileList.First.Value);
                        fileList.RemoveFirst();
                    }
                    FilesRead += result.Count;
                    return result;
                }
                else
                {
                    var result = new FileItem[fileList.Count];
                    fileList.CopyTo(result, 0);
                    fileList.Clear();
                    FilesRead += result.Length;
                    return result;
                }
            }
        }

        /// <summary>
        /// Retrieves the next file found.
        /// This function waits until a file is found or the search thread completes without finding any further items.
        /// </summary>
        /// <param name="waitAction">An action to call when entering wait for next search results.</param>
        /// <returns>Returns the next file found or null if the finder completed without finding any further files.</returns>
        public FileItem GetNext(Action waitAction = null)
        {
            while (FileSearchRunning)
            {
                lock (fileList)
                {
                    if (fileList.Count > 0)
                    {
                        FileItem result = fileList.First.Value;
                        fileList.RemoveFirst();
                        return result;
                    }
                    if (waitAction == null)
                    {
                        Monitor.Wait(fileList);
                    }
                }
                waitAction?.Invoke();
            }
            lock (fileList)
            {
                if (fileList.Count > 0)
                {
                    FileItem result = fileList.First.Value;
                    fileList.RemoveFirst();
                    FilesRead++;
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets or sets the file mask applied while searching.
        /// </summary>
        public string FileMask { get => fileMask; set => fileMask = !Started ? value : throw new ReadOnlyException("FileFinder already started!"); }

        /// <summary>
        /// Gets or sets the directory mask applied while searching.
        /// </summary>
        public string DirectoryMask { get => directoryMask; set => directoryMask = !Started ? value : throw new ReadOnlyException("FileFinder already started!"); }

        /// <summary>
        /// Gets or sets the base directory of the search.
        /// </summary>
        public string BaseDirectory { get => baseDirectory; set => baseDirectory = !Started ? CheckDirectory(value) : throw new ReadOnlyException("FileFinder already started!"); }

        /// <summary>
        /// Gets a value indicating whether the filefinder has been started.
        /// </summary>
        public bool Started { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the filefinder has completed the search task and all items have been read.
        /// </summary>
        public bool Completed => !FileSearchRunning && FilesQueued == 0;

        /// <summary>
        /// Gets the comparers used to (un)select a directory.
        /// </summary>
        public IList<IFileFinderComparer> Comparer { get; private set; } = new List<IFileFinderComparer>();

        /// <summary>
        /// Starts the finder.
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                if (Started)
                {
                    throw new InvalidOperationException("FileFinder already started!");
                }

                Started = true;
                Comparer = new ReadOnlyCollection<IFileFinderComparer>(Comparer.ToArray());
            }

            Verbose($"Start FileFinder at {BaseDirectory}");
            if (!Directory.Exists(BaseDirectory))
            {
                throw new DirectoryNotFoundException();
            }

            DirectorySearchRunning = true;
            FileSearchRunning = true;
            tasks = new[]
            {
                Task.Factory.StartNew(SearchDirectories),
                Task.Factory.StartNew(SearchFiles),
            };
        }

        /// <summary>
        /// Gets the number of queued files.
        /// </summary>
        public int FilesQueued
        {
            get
            {
                lock (fileList)
                {
                    return fileList.Count;
                }
            }
        }

        /// <summary>
        /// Closes the finder.
        /// </summary>
        public void Close()
        {
            DirectorySearchRunning = false;
            FileSearchRunning = false;
            lock (fileList)
            {
                fileList.Clear();
                Monitor.PulseAll(fileList);
            }
            lock (directoryList)
            {
                directoryList.Clear();
                Monitor.PulseAll(directoryList);
            }
            if (tasks != null)
            {
                Task.WaitAll(tasks);
            }
        }

        /// <summary>
        /// Releases all resources used by the this instance.
        /// </summary>
        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        string CheckDirectory(string value)
        {
            var result = FileSystem.GetFullPath(value);
            if (!Directory.Exists(result))
            {
                throw new DirectoryNotFoundException();
            }
            return result;
        }

        void Verbose(string message, bool error = false)
        {
            if (EnableTrace)
            {
                if (error)
                {
                    Trace.TraceError(message);
                }
                else
                {
                    Trace.WriteLine(message);
                }
            }
            if (EnableDebug)
            {
                Debug.WriteLine(message);
            }
        }
    }
}
