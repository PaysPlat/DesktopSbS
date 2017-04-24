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
using Microsoft.Win32;

namespace DesktopSbS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static Mutex mutex = new Mutex(true, "{A118F0EB-E2D3-465C-8821-89061A45EE4C}");

        public static MainVoidWindow CurrentWindow
        {
            get { return (Application.Current as App).MainWindow as MainVoidWindow; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                this.Shutdown();
            }
            else
            {
                this.MainWindow = new MainVoidWindow();
            }

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            mutex.ReleaseMutex();
        }

    }
}
