using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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


            for (int i = 0; i < this.tmpWindows.Count; ++i)
            {
                int oldIndex = this.windows.FindIndex(w => w.Handle == this.tmpWindows[i].Handle);
                if (oldIndex < 0)
                {
                    this.tmpWindows[i].RegisterThumbs();
                }
                else
                {
                    User32.RECT newRect = this.tmpWindows[i].SourceRect;
                    this.tmpWindows[i] = this.windows[oldIndex];
                    this.windows.RemoveAt(oldIndex);

                    if (!newRect.Equals(this.tmpWindows[i].SourceRect))
                    {
                        this.tmpWindows[i].SourceRect = newRect;
                        this.tmpWindows[i].UpdateThumbs();
                    }

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

            DwmApi.WS winStyle = (DwmApi.WS)User32.GetWindowLongA(hwnd, DwmApi.GWL_STYLE);
            DwmApi.WSEX winStyleEx = (DwmApi.WSEX)User32.GetWindowLongA(hwnd, -20);

            User32.RECT sourceRect = new User32.RECT();

            User32.GetWindowRect(hwnd, ref sourceRect);

            if (/*this.handle != hwnd
                &&*/ !sourceRect.IsEmpty()
                && (winStyle & DwmApi.WS.WS_VISIBLE) == DwmApi.TARGETWINDOW
                && (winStyle & DwmApi.WS.WS_ICONIC) == 0
                 && (winStyle & DwmApi.WS.WS_DISABLED) == 0
                 && (winStyleEx & DwmApi.WSEX.WS_EX_NOREDIRECTIONBITMAP) == 0)
            {

                WinSBS win = new WinSBS(hwnd);
                win.SourceRect = sourceRect;
                this.tmpWindows.Add(win);
            }

            return true; //continue enumeration
        }



    }
}
