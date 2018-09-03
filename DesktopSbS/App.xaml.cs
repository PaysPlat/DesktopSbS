using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DesktopSbS.Hook;
using DesktopSbS.Interop;
using Microsoft.Win32;

namespace DesktopSbS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public const string VERSION = "0.5";


        static Mutex mutex = new Mutex(true, "{A118F0EB-E2D3-465C-8821-89061A45EE4C}");

        public static View.MainVoidWindow CurrentWindow
        {
            get { return (Application.Current as App).MainWindow as View.MainVoidWindow; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Version win10Version = Util.GetWindowsVersion();
            if (win10Version.Major < 10 || (win10Version.Major == 10 && win10Version.Build < 15063))
            {
                View.VersionWarningWindow vww = new View.VersionWarningWindow();
                vww.ShowDialog();
                this.Shutdown(1);
            }

            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                MessageBox.Show("Only one instance of DesktopSbS can run");
                this.Shutdown(1);
            }
            else
            {
                this.MainWindow = new View.MainVoidWindow();
            }

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (e.ApplicationExitCode >= 1)
                return;
            mutex.ReleaseMutex();
        }

    }
}
