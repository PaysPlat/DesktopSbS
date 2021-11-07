using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using DesktopSbS.Properties;
using Microsoft.Win32;

namespace DesktopSbS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string APP_NAME = "DesktopSbS";

        public const string VERSION = "1.3";

        public const string VERSIONS_URL = "https://desktopsbs.paysplat.fr/versions";


        static Mutex mutex = new Mutex(true, "{A118F0EB-E2D3-465C-8821-89061A45EE4C}");

        public static View.MainVoidWindow CurrentWindow
        {
            get { return (Application.Current as App).MainWindow as View.MainVoidWindow; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            if (e.Args.FirstOrDefault()?.ToLower() == "/read-settings")
            {
                string settings = Util.ReadSettings();
                Console.WriteLine(settings);
                this.Shutdown(1);
            }

            Version win10Version = Util.GetWindowsVersion();
            if (win10Version == null)
            {
                MessageBox.Show("Windows OS version cannot be read");
            }
            else if (win10Version.Major < 10 || (win10Version.Major == 10 && win10Version.Build < 15063))
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

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogException(ex);
            }
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception);
        }

        private void LogException(Exception e)
        { 
            string diagnosticFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{DesktopSbS.App.APP_NAME}.Exception.txt";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{DateTime.Now} -> {e}");

            try
            {
                File.AppendAllText(diagnosticFilePath, sb.ToString());
                Process.Start(diagnosticFilePath);
            }
            catch (Exception ex)
            {
                
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (e.ApplicationExitCode >= 1)
            {
                return;
            }
            mutex.ReleaseMutex();
        }

    }
}
