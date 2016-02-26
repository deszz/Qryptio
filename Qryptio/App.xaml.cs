using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows;

namespace Qryptio
{
    public partial class App : Application
    {
        public static bool HasAdminPrivelegies
        {
            get
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
        public static void RestartWithAdminPrivelegies()
        {
            bool exit = true;

            var info = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location) { Verb = "runas" };
            var proc = new Process() { StartInfo = info };

            try
            {
                proc.Start();
            }
            catch (Win32Exception)
            {
                // operation cancelled
                exit = false;
            }
            finally
            {
                if (exit)
                    Current.Shutdown(0);
            }
        }

        public static string InitFile
        { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var args = Environment.GetCommandLineArgs();

            if (args.Length > 1 && File.Exists(args[1]))
                InitFile = args[1];

            base.OnStartup(e);
        }
    }
}
