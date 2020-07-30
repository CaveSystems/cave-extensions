using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cave
{
    /// <summary>Gets an asynchronous file finder.</summary>
    public class DirectoryFinder
    {
        readonly Queue<DirectoryItem> directoryList = new Queue<DirectoryItem>();
        string baseDirectory;
        bool deepestFirst;
        int depth;
        string directoryMask;
        int maxDepth;
        Task task;

        /// <summary>Gets a value indicating whether the search task is still running.</summary>
        public bool SearchRunning { get; private set; }

        /// <summary>Gets the current progress of the finder. This is a very rough estimation.</summary>
        public float Progress { get; private set; }

        /// <summary>Gets the comparers used to (un)select a directory.</summary>
        public IList<IDirectoryFinderComparer> Comparer { get; private set; } = new List<IDirectoryFinderComparer>();

        /// <summary>Gets or sets the directory mask applied while searching.</summary>
        public string DirectoryMask
        {
            get => directoryMask;
            set => directoryMask = !Started ? value : throw new ReadOnlyException("FileFinder already started!");
        }

        /// <summary>Gets or sets the base directory of the search.</summary>
        public string BaseDirectory
        {
            get => baseDirectory;
            set => baseDirectory = !Started ? CheckDirectory(value) : throw new ReadOnlyException("FileFinder already started!");
        }

        /// <summary>Gets a value indicating whether the finder has been started.</summary>
        public bool Started { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the finder returns the deepest directory first (e.g. first
        ///     /tmp/some/dir then /tmp/some).
        /// </summary>
        public bool DeepestFirst { get => deepestFirst; set => deepestFirst = !Started ? value : throw new ReadOnlyException("Finder was already started!"); }

        /// <summary>Gets a value indicating whether the filefinder has completed the search task and all items have been read.</summary>
        public bool Completed => Started && !SearchRunning;

        /// <summary>Gets or sets the maximum number of directories in queue.</summary>
        public int MaximumDirectoriesQueued { get; set; }

        /// <summary>Gets or sets a value indicating whether logging of messages to <see cref="Trace" /> output is enabled.</summary>
        public bool EnableTrace { get; set; }

        /// <summary>Gets or sets a value indicating whether logging of messages to <see cref="Debug" /> output is enabled.</summary>
        public bool EnableDebug { get; set; }

        static string CheckDirectory(string value)
        {
            var result = FileSystem.GetFullPath(value);
            if (!Directory.Exists(result))
            {
                throw new DirectoryNotFoundException();
            }

            return result;
        }

        [SuppressMessage("Design", "CA1031")]
        void RecursiveSearch(DirectoryItem current)
        {
            try
            {
                if (++depth > maxDepth)
                {
                    maxDepth++;
                }

                string[] dirs;
                try
                {
                    dirs = directoryMask == null ? Directory.GetDirectories(current.FullPath) : Directory.GetDirectories(current.FullPath, DirectoryMask);
                }
                catch (Exception ex)
                {
                    Error?.Invoke(this, new ErrorEventArgs(ex));
                    Verbose($"DirectoryFinder got an error reading {current}: {ex.Message}", true);
                    return;
                }

                foreach (var dir in dirs)
                {
                    var directory = DirectoryItem.FromFullPath(BaseDirectory, dir);
                    foreach (var comparer in Comparer)
                    {
                        if (!comparer.DirectoryMatches(directory))
                        {
                            directory = null;
                            break;
                        }
                    }

                    if (directory != null)
                    {
                        if (deepestFirst)
                        {
                            // recursive search in directory first
                            RecursiveSearch(directory);
                        }

                        var addDirectoryToList = true;
                        var callback = FoundDirectory;
                        if (callback != null)
                        {
                            var arg = new DirectoryItemEventArgs(directory);
                            callback.Invoke(this, arg);
                            addDirectoryToList = !arg.Handled;
                        }

                        // then add items to list
                        while (SearchRunning && addDirectoryToList)
                        {
                            lock (directoryList)
                            {
                                if ((MaximumDirectoriesQueued <= 0) || (directoryList.Count < MaximumDirectoriesQueued))
                                {
                                    directoryList.Enqueue(directory);
                                    Monitor.Pulse(directoryList);
                                    break;
                                }

                                Monitor.Wait(directoryList);
                            }
                        }

                        if (!deepestFirst)
                        {
                            // recursive search later
                            RecursiveSearch(directory);
                        }
                    }
                }

                if (depth == 0)
                {
                    Progress = 1;
                }
                else
                {
                    var newProgress = Math.Max(1 - (--depth / (float) maxDepth), Progress);
                    if (newProgress > Progress)
                    {
                        Progress = Math.Min(Progress + 0.01f, newProgress);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Cannot get directory listing for {0}.\n{1}", current, ex);
            }
        }

        /// <summary>runs the current search.</summary>
        void SearchDirectories()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            Thread.CurrentThread.IsBackground = true;
            Verbose($"Starting directory search at {BaseDirectory}");
            RecursiveSearch(new DirectoryItem(BaseDirectory, "."));
            SearchRunning = false;
            Verbose($"Completed directory search at {BaseDirectory}");
        }

        /// <summary>Starts the finder.</summary>
        public void Start()
        {
            lock (this)
            {
                if (Started)
                {
                    throw new InvalidOperationException("DirectoryFinder already started!");
                }

                Started = true;
                Comparer = new ReadOnlyCollection<IDirectoryFinderComparer>(Comparer.ToArray());
            }

            Verbose($"Start FileFinder at {BaseDirectory}");
            if (!Directory.Exists(BaseDirectory))
            {
                throw new DirectoryNotFoundException();
            }

            SearchRunning = true;
            task = Task.Factory.StartNew(SearchDirectories);
        }

        /// <summary>Retrieves (dequeues) all files already found. This may called repeatedly until Completed==true.</summary>
        /// <param name="wait">Wait until at least one file was found.</param>
        /// <param name="maximum">Maximum number of items to return.</param>
        /// <returns>Returns an array of files.</returns>
        public IList<DirectoryItem> Get(bool wait = false, int maximum = 0)
        {
            lock (directoryList)
            {
                if (wait)
                {
                    while ((directoryList.Count == 0) && SearchRunning)
                    {
                        Monitor.Wait(directoryList);
                    }
                }

                if (maximum > 0)
                {
                    if (directoryList.Count < maximum)
                    {
                        maximum = directoryList.Count;
                    }

                    var result = new List<DirectoryItem>(maximum);
                    for (var i = 0; i < maximum; i++)
                    {
                        result.Add(directoryList.Dequeue());
                    }

                    return result;
                }
                else
                {
                    var result = new DirectoryItem[directoryList.Count];
                    directoryList.CopyTo(result, 0);
                    directoryList.Clear();
                    return result;
                }
            }
        }

        /// <summary>
        ///     Retrieves the next directory found. This function waits until a directory is found or the search thread
        ///     completes without finding any further items.
        /// </summary>
        /// <param name="waitAction">An action to call when entering wait for next search results.</param>
        /// <returns>Returns the next file found or null if the finder completed without finding any further directories.</returns>
        public DirectoryItem GetNext(Action waitAction = null)
        {
            DirectoryItem result = null;
            while (SearchRunning)
            {
                lock (directoryList)
                {
                    if (directoryList.Count > 0)
                    {
                        result = directoryList.Dequeue();
                        break;
                    }

                    if (waitAction == null)
                    {
                        Monitor.Wait(directoryList);
                    }
                }

                waitAction?.Invoke();
            }

            lock (directoryList)
            {
                if ((result == null) && (directoryList.Count > 0))
                {
                    result = directoryList.Dequeue();
                }

                Monitor.Pulse(directoryList);
            }

            return result;
        }

        /// <summary>Called on each error</summary>
        public event EventHandler<ErrorEventArgs> Error;

        /// <summary>The found directory event</summary>
        public event EventHandler<DirectoryItemEventArgs> FoundDirectory;

        /// <summary>Closes the finder.</summary>
        public void Close()
        {
            SearchRunning = false;
            task?.Wait();
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
