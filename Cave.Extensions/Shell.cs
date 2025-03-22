#if !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Cave;

/// <summary>Shell extensions.</summary>
public static class Shell
{
    /// <summary>Provides a runner for shell commands.</summary>
    public class ConsoleRunner
    {
        /// <summary>
        /// Throw exceptions if process cannot be created, times out or has any other problem. (Will not throw if execution succeeds with any <see cref="ShellResult.ExitCode"/>.
        /// </summary>
        public bool ThrowException { get; init; } = true;

        /// <summary>Collect output and error strings and put them to <see cref="ShellResult.StdErr"/> and <see cref="ShellResult.StdOut"/>.</summary>
        public bool CollectStrings { get; init; } = true;

        /// <summary>Filename to start process with.</summary>
        public string? FileName { get; init; }

        /// <summary>Arguments to use.</summary>
        public string? Arguments { get; init; }

        /// <summary>Action to be run on each output line.</summary>
        public Action<string>? StdOut { get; init; }

        /// <summary>Action to be run on each error line.</summary>
        public Action<string>? StdErr { get; init; }

        /// <summary>Action to be run if the process completes with an exit code.</summary>
        public Action<int>? Completed { get; init; }

        /// <summary>Timeout for the process.</summary>
        public int TimeoutMilliseconds { get; init; }

        /// <summary>Environment variables to be used.</summary>
        public Dictionary<string, string> Environment { get; } = new();

        internal void OnStdOut(string line) => StdOut?.Invoke(line);

        internal void OnStdErr(string line) => StdErr?.Invoke(line);

        internal void OnCompleted(int code) => Completed?.Invoke(code);

        /// <summary>Runs a process with redirected output and error stream.</summary>
        public ShellResult RunRedirected(bool collectOutput) => Shell.RunRedirected(this);
    }

    #region Static

    /// <summary>Runs a process with redirected output and error newline encoded text streams. Do not use this for binary input, output and error streams!</summary>
    public static ShellResult RunRedirected(ConsoleRunner runner)
    {
        var stdoutLines = new LinkedList<string>();
        var stderrLines = new LinkedList<string>();
        Debug.WriteLine($"Run {runner.FileName} {runner.Arguments}");
        var startInfo = new ProcessStartInfo
        {
            WorkingDirectory = Directory.GetCurrentDirectory(),
            FileName = runner.FileName,
            Arguments = runner.Arguments ?? string.Empty,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        foreach (var env in runner.Environment)
        {
            startInfo.EnvironmentVariables[env.Key] = env.Value;
        }
        using var outputWaitHandle = new ManualResetEvent(false);
        using var errorWaitHandle = new ManualResetEvent(false);
        Process process;
        try
        {
            process = Process.Start(startInfo) ?? throw new InvalidOperationException("Could not start process.");

            Debug.WriteLine($"Start reading from process [{process.Id}] {runner.FileName}");
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    if (runner.CollectStrings) stderrLines.AddLast(e.Data);
                    runner.OnStdErr(e.Data);
                }
                else
                {
                    _ = errorWaitHandle.Set();
                }
            };
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    if (runner.CollectStrings) stdoutLines.AddLast(e.Data);
                    runner.OnStdOut(e.Data);
                }
                else
                {
                    _ = outputWaitHandle.Set();
                }
            };
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            Debug.WriteLine($"Wait for exit [{process.Id}] {runner.FileName}");
            var timeout = runner.TimeoutMilliseconds < 1 ? -1 : runner.TimeoutMilliseconds;
            if (process.WaitForExit(timeout) && outputWaitHandle.WaitOne(timeout) && errorWaitHandle.WaitOne(timeout))
            {
                Debug.WriteLine($"Process {runner.FileName} exited with code {process.ExitCode}.");
                runner.OnCompleted(process.ExitCode);
                return new ShellResult()
                {
                    ExitCode = process.ExitCode,
                    StdErr = [.. stderrLines],
                    StdOut = [.. stdoutLines],
                };
            }

            try
            {
                process.Kill();
            }
            catch
            {
                Debug.WriteLine($"Error killing process {runner.FileName}");
            }

            throw new TimeoutException($"Process {runner.FileName} timed out.");
        }
        catch (Exception ex)
        {
            if (runner.ThrowException) throw;
            return new ShellResult() { Exception = ex };
        }
    }

    /// <summary>
    /// Runs a process with redirected output and error stream. This function throws exceptions. Do not use this for binary input, output and error streams!
    /// </summary>
    /// <param name="filename">Filename to start.</param>
    /// <param name="arguments">Arguments.</param>
    /// <param name="stdout">Action to call for each incoming line at the output stream.</param>
    /// <param name="stderr">Action to call for each incoming line at the error stream.</param>
    /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
    /// <returns>Returns the exit code on success. On errors an exception will be thrown.</returns>
    public static int RunRedirected(string filename, string? arguments = null, Action<string>? stdout = null, Action<string>? stderr = null, int timeoutMilliseconds = default) => RunRedirected(new ConsoleRunner()
    {
        ThrowException = true,
        CollectStrings = false,
        FileName = filename,
        Arguments = arguments,
        StdOut = stdout,
        StdErr = stderr,
        TimeoutMilliseconds = timeoutMilliseconds
    }).ExitCode;

    /// <summary>
    /// Runs a process with redirected output and error stream. This function will not throw exceptions. Do not use this for binary input, output and error streams!
    /// </summary>
    /// <param name="filename">Filename to start.</param>
    /// <param name="arguments">Arguments.</param>
    /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
    /// <returns>Returns a result dataset.</returns>
    public static ShellResult RunRedirected(string filename, string arguments, int timeoutMilliseconds = default) => RunRedirected(new ConsoleRunner()
    {
        ThrowException = false,
        CollectStrings = true,
        FileName = filename,
        Arguments = arguments,
        TimeoutMilliseconds = timeoutMilliseconds
    });

    #endregion Static
}

#endif
