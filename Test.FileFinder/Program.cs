using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cave;

namespace Test.FileFinder
{
    class Program
    {
        static void CheckFileFinder()
        {
            var sw = StopWatch.StartNew();
            var ff = new Cave.FileFinder
            {
                BaseDirectory = "/"
            };
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

        static void CheckDirectoryFinder()
        {
            var sw = StopWatch.StartNew();
            var df = new Cave.DirectoryFinder
            {
                BaseDirectory = "/"
            };
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

        static float lastProgress;

        static void ShowProgress(float progress, IStopWatch sw)
        {
            if (progress != lastProgress)
            {
                lastProgress = progress;
                Console.Title = progress.ToString("P");
                Console.WriteLine(sw.Elapsed.FormatTime() + " " + progress.ToString("P"));
            }
        }

        static void Main(string[] args)
        {
            CheckFileFinder();
            CheckDirectoryFinder();
        }
    }
}
