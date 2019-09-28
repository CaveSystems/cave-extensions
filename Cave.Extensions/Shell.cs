using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Cave
{
    /// <summary>
    /// Shell extensions.
    /// </summary>
    public static class Shell
    {
        /// <summary>
        /// Runs a process with redirected output and error stream.
        /// </summary>
        /// <param name="filename">Filename to start.</param>
        /// <param name="arguments">Arguments.</param>
        /// <param name="stdout">Action to call for each incoming line at the output stream.</param>
        /// <param name="stderr">Action to call for each incoming line at the error stream.</param>
        /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
        /// <returns>Returns a result dataset.</returns>
        public static int RunRedirected(string filename, string arguments, Action<string> stdout, Action<string> stderr, int timeoutMilliseconds = default)
        {
            Debug.WriteLine($"Run {filename} {arguments}");
            var startInfo = new ProcessStartInfo()
            {
                WorkingDirectory = Directory.GetCurrentDirectory(),
                FileName = filename,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            using (var outputWaitHandle = new ManualResetEvent(false))
            using (var errorWaitHandle = new ManualResetEvent(false))
            using (var process = Process.Start(startInfo))
            {
                Debug.WriteLine($"Start reading from process [{process.Id}] {filename}");
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        stderr?.Invoke(e.Data);
                    }
                    else
                    {
                        errorWaitHandle.Set();
                    }
                };
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        stdout?.Invoke(e.Data);
                    }
                    else
                    {
                        outputWaitHandle.Set();
                    }
                };
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                Debug.WriteLine($"Wait for exit [{process.Id}] {filename}");
                if (timeoutMilliseconds < 1)
                {
                    timeoutMilliseconds = -1;
                }

                if (process.WaitForExit(timeoutMilliseconds) && outputWaitHandle.WaitOne(timeoutMilliseconds) && errorWaitHandle.WaitOne(timeoutMilliseconds))
                {
                    Debug.WriteLine($"Process {filename} exited with code {process.ExitCode}.");
                    return process.ExitCode;
                }
                else
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                    }
                    throw new TimeoutException($"Process {filename} timed out.");
                }
            }
        }

        /// <summary>
        /// Runs a process with redirected output and error stream.
        /// </summary>
        /// <param name="filename">Filename to start.</param>
        /// <param name="arguments">Arguments.</param>
        /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
        /// <returns>Returns a result dataset.</returns>
        public static Result RunRedirected(string filename, string arguments, int timeoutMilliseconds = default)
        {
            var stdoutLines = new LinkedList<string>();
            var stderrLines = new LinkedList<string>();
            var result = new Result();
            try
            {
                result.ExitCode = RunRedirected(filename, arguments, s => stdoutLines.AddLast(s), s => stderrLines.AddLast(s), timeoutMilliseconds);
                result.StdOut = stdoutLines.ToArray();
                result.StdErr = stderrLines.ToArray();
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                Debug.WriteLine($"Process {filename} exited with exception {ex}.");
            }
            return result;
        }

        /// <summary>
        /// A process result.
        /// </summary>
        public class Result
        {
            /// <summary>
            /// Gets or sets all output lines.
            /// </summary>
            public string[] StdOut { get; protected internal set; }

            /// <summary>
            /// Gets or sets all error lines.
            /// </summary>
            public string[] StdErr { get; protected internal set; }

            /// <summary>
            /// Gets or sets the exit code of the process.
            /// </summary>
            public int ExitCode { get; protected internal set; }

            /// <summary>
            /// Gets or sets the exception catched on process start.
            /// </summary>
            public Exception Exception { get; protected internal set; }
        }
    }
}
