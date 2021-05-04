using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Win32;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace BeWallpaper
{
    class Program
    {
        private const string EXEEXT = ".exe";

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine($"{Path.GetFileName(Assembly.GetEntryAssembly().Location)} exe-path");
                return;
            }
            var exe = args[0];
            var ext = Path.GetExtension(exe);
            if (string.Compare(ext, EXEEXT, true) != 0)
            {
                Console.WriteLine("only support exe");
                return;
            }
            var workingdir = Path.GetDirectoryName(exe);
            if (string.IsNullOrWhiteSpace(workingdir))
            {
                workingdir = Environment.CurrentDirectory;
            }
            var process = Process.Start(new ProcessStartInfo(exe)
            {
                UseShellExecute = false,
                WorkingDirectory = workingdir,
                WindowStyle = ProcessWindowStyle.Maximized,
                CreateNoWindow = true,
            });

            if (process == null)
            {
                Console.WriteLine("null process");
                return;
            }

            var targetWindow = IntPtr.Zero;
            if (!SpinWait.SpinUntil(() => process.MainWindowHandle != null && process.MainWindowHandle != IntPtr.Zero, TimeSpan.FromSeconds(3)))
            {
                var windows = Utils.GetProcessMainWindows(process.Id);
                if (windows.Count == 0)
                {
                    try
                    {
                        process.Kill();
                        process.Dispose();
                    }
                    catch { }
                    return;
                }
                targetWindow = windows.First();
            }
            if (targetWindow == IntPtr.Zero)
                targetWindow = process.MainWindowHandle;

            Utils.BeWallpaper(targetWindow);
            Console.WriteLine("OK");
        }
    }
}
