using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Cave;

/// <summary>Shell extensions.</summary>
public static class Shell
{
    #region Static

    /// <summary>Runs a process with redirected output and error stream.</summary>
    /// <param name="filename">Filename to start.</param>
    /// <param name="arguments">Arguments.</param>
    /// <param name="stdout">Action to call for each incoming line at the output stream.</param>
    /// <param name="stderr">Action to call for each incoming line at the error stream.</param>
    /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
    /// <returns>Returns a result dataset.</returns>
    public static int RunRedirected(string filename, string arguments, Action<string> stdout, Action<string> stderr, int timeoutMilliseconds = default)
    {
        Debug.WriteLine($"Run {filename} {arguments}");
        var startInfo = new ProcessStartInfo
        {
            WorkingDirectory = Directory.GetCurrentDirectory(),
            FileName = filename,
            Arguments = arguments,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        using var outputWaitHandle = new ManualResetEvent(false);
        using var errorWaitHandle = new ManualResetEvent(false);
        using var process = Process.Start(startInfo);
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

        try
        {
            process.Kill();
        }
        catch
        {
            Debug.WriteLine($"Error killing process {filename}");
        }

        throw new TimeoutException($"Process {filename} timed out.");
    }

    /// <summary>Runs a process with redirected output and error stream.</summary>
    /// <param name="filename">Filename to start.</param>
    /// <param name="arguments">Arguments.</param>
    /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
    /// <returns>Returns a result dataset.</returns>
    public static ShellResult RunRedirected(string filename, string arguments, int timeoutMilliseconds = default)
    {
        var stdoutLines = new LinkedList<string>();
        var stderrLines = new LinkedList<string>();
        var result = new ShellResult();
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

    #endregion
}
