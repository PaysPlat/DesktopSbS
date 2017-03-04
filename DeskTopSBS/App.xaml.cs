using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace DeskTopSBS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private List<WinSBS> windows = new List<WinSBS>();
        private List<WinSBS> tmpWindows = new List<WinSBS>();

      
        private bool requestAbort = false;

        private Thread threadUpdateWindows;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            

            this.threadUpdateWindows = new Thread(asyncUpdateWindows);
            this.threadUpdateWindows.Start();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this.requestAbort = true;
        }

        private void asyncUpdateWindows()
        {
            while (!this.requestAbort)
            {
                DateTime start = DateTime.Now;
                this.Dispatcher.Invoke(this.updateWindows);
                DateTime end = DateTime.Now;

                int elapsedMS = (int)(end - start).TotalMilliseconds;

                Thread.Sleep(Math.Max(0, 20 - elapsedMS));
            }
        }


        private void updateWindows()
        {
            this.tmpWindows = new List<WinSBS>();
            User32.EnumWindows(windowFound, 0);
            
            for (int i = this.tmpWindows.Count-1; i >=0 ; --i)
            {
                if (i < this.tmpWindows.Count - 1)
                {
                    this.tmpWindows[i].Owner = this.tmpWindows[i + 1];
                }
                else
                {
                    this.tmpWindows[i].Owner = null;
                }

                int oldIndex = this.windows.FindIndex(w => w.Handle == this.tmpWindows[i].Handle);
                if (oldIndex < 0)
                {
                    this.tmpWindows[i].RegisterThumbs();
                }
                else
                {
                    this.tmpWindows[i].CopyThumbInstances(this.windows[oldIndex]);

                    if (!this.windows[oldIndex].SourceRect.Equals(this.tmpWindows[i].SourceRect) ||
                        this.windows[oldIndex].Owner?.Handle != this.tmpWindows[i].Owner?.Handle)
                    {
                        this.tmpWindows[i].UpdateThumbs();
                    }
                    this.windows.RemoveAt(oldIndex);

                }

            }

            for (int i = 0; i < this.windows.Count; ++i)
            {
                this.windows[i].UnRegisterThumbs();
            }

          

            this.windows = this.tmpWindows;

        }

        private bool windowFound(IntPtr hwnd, int lParam)
        {

            StringBuilder sb = new StringBuilder(100);
            User32.GetWindowText(hwnd, sb, sb.Capacity);
            string title = sb.ToString();

            if (title == "ThumbWindows") return true;

            WS winStyle = (WS)User32.GetWindowLongA(hwnd, User32.GWL_STYLE);
            WSEX winStyleEx = (WSEX)User32.GetWindowLongA(hwnd, User32.GWL_EXSTYLE);

            User32.RECT sourceRect = new User32.RECT();

            User32.GetWindowRect(hwnd, ref sourceRect);

            if (!sourceRect.IsEmpty()
                && (winStyle & WS.WS_VISIBLE) == WS.WS_VISIBLE
                && (winStyle & WS.WS_ICONIC) == 0
                 && (winStyle & WS.WS_DISABLED) == 0
                 && (winStyleEx & WSEX.WS_EX_NOREDIRECTIONBITMAP) == 0)
            {

                WinSBS win = new WinSBS(hwnd);
                win.SourceRect = sourceRect;
                win.Title = title;
                this.tmpWindows.Add(win);
            }

            return true; //continue enumeration
        }

       

    }
}
