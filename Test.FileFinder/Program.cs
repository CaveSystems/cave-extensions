using System;
using System.Threading;
using Cave;

namespace Test.FileFinder;

class Program
{
    #region Static

    static float lastProgress;

    static void CheckDirectoryFinder()
    {
        var sw = StopWatch.StartNew();
        var df = new DirectoryFinder { BaseDirectory = "/" };
        //ff.EnableDebug = true;
        df.Start();
        var count = 0;
        while (!df.Completed)
        {
            Thread.Sleep(1000);
            var files = df.Get(true);
            ShowProgress(df.Progress, sw);
            count += files.Count;
        }

        df.Close();
        Console.WriteLine(count + " files");
        Console.WriteLine(sw.Elapsed.FormatTime());
        Console.WriteLine(sw.Elapsed.ToString());
    }

    static void CheckFileFinder()
    {
        var sw = StopWatch.StartNew();
        var ff = new Cave.FileFinder { BaseDirectory = "/" };
        //ff.EnableDebug = true;
        ff.Start();
        var count = 0;
        while (!ff.Completed)
        {
            Thread.Sleep(1000);
            var files = ff.Get(true);
            ShowProgress(ff.Progress, sw);
            count += files.Count;
        }

        ff.Close();
        Console.WriteLine(count + " files");
        Console.WriteLine(sw.Elapsed.FormatTime());
        Console.WriteLine(sw.Elapsed.ToString());
    }

    static void Main()
    {
        CheckFileFinder();
        CheckDirectoryFinder();
    }

    static void ShowProgress(float progress, IStopWatch sw)
    {
        if (progress != lastProgress)
        {
            lastProgress = progress;
            Console.Title = progress.ToString("P");
            Console.WriteLine(sw.Elapsed.FormatTime() + " " + progress.ToString("P"));
        }
    }

    #endregion
}