using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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


        public static readonly string ExePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\";


        private GlobalKeyboardHook keyboardHook;

        private List<WinSbS> windows = new List<WinSbS>();
        private List<WinSbS> tmpWindows = new List<WinSbS>();


        private CursorSbS cursorSbS;


        private bool requestAbort = false;

        private Thread threadUpdateWindows;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.keyboardHook = new GlobalKeyboardHook();
            this.keyboardHook.KeyDown += KeyboardHook_KeyDown;

            this.hideCursors();
            this.cursorSbS = new CursorSbS();
            this.cursorSbS.RegisterThumbs();

            this.threadUpdateWindows = new Thread(asyncUpdateWindows);
            this.threadUpdateWindows.IsBackground = true;
            this.threadUpdateWindows.Start();

        }

        private void KeyboardHook_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Pause)
            {
                this.requestAbort = true;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this.showCursors();

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
            this.Dispatcher.Invoke(this.Shutdown);
        }


        private void updateWindows()
        {
            this.tmpWindows = new List<WinSbS>();
            User32.EnumWindows(windowFound, 0);

            int updateAllIndex = -1;

            for (int i = this.tmpWindows.Count - 1; i >= 0; --i)
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

                    if (updateAllIndex < 0 && this.windows[oldIndex].Owner?.Handle != this.tmpWindows[i].Owner?.Handle)
                    {
                        updateAllIndex = i;
                    }
                    else if (!this.windows[oldIndex].SourceRect.Equals(this.tmpWindows[i].SourceRect))
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

            if (updateAllIndex > -1)
            {
                for (int i = updateAllIndex; i >= 0; --i)
                {
                    this.windows[i].UpdateThumbs();
                }
            }



            // this.cursorSbS.Owner = this.windows.FirstOrDefault();
            this.cursorSbS.Position = User32.GetMousePositionOnScreen();
            this.cursorSbS.UpdateThumbs();


            // DebugWindow.Instance.UpdateMessage($"Active Window: {this.cursorSbS.Owner} Handle: {this.cursorSbS.Owner.Handle}{Environment.NewLine}Exe: {User32.GetFilePath(this.cursorSbS.Owner.Handle)}");
            //DebugWindow.Instance.UpdateMessage(count.ToString());
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

                WinSbS win = new WinSbS(hwnd);
                win.SourceRect = sourceRect;
                win.Title = title;
                this.tmpWindows.Add(win);
            }

            return true; //continue enumeration
        }

        private void hideCursors()
        {
            RegistryKey cursorsKey = Registry.CurrentUser.OpenSubKey("Control Panel").OpenSubKey("Cursors", true);
            cursorsKey.SetValue("AppStarting", $@"{App.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("Arrow", $@"{App.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("Hand", $@"{App.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("Help", $@"{App.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("No", $@"{App.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("NWPen", $@"{App.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("SizeAll", $@"{App.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("SizeNESW", $@"{App.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("SizeNS", $@"{App.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("SizeNSWE", $@"{App.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("SizeWE", $@"{App.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("UpArrow", $@"{App.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("Wait", $@"{App.ExePath}Resources\blank.cur");
            User32.SystemParametersInfo(User32.SPI_SETCURSORS, 0, 0, User32.SPIF_UPDATEINIFILE | User32.SPIF_SENDCHANGE);

        }
        private void showCursors()
        {
            RegistryKey cursorsKey = Registry.CurrentUser.OpenSubKey("Control Panel").OpenSubKey("Cursors", true);
            cursorsKey.SetValue("AppStarting", @"%SystemRoot%\cursors\aero_working.cur");
            cursorsKey.SetValue("Arrow", @"%SystemRoot%\cursors\aero_arrow.cur");
            cursorsKey.SetValue("Hand",  @"%SystemRoot%\cursors\aero_link.cur");
            cursorsKey.SetValue("Help",  @"%SystemRoot%\cursors\aero_helpsel.cur");
            cursorsKey.SetValue("No",  @"%SystemRoot%\cursors\aero_unavail.cur");
            cursorsKey.SetValue("NWPen",  @"%SystemRoot%\cursors\aero_pen.cur");
            cursorsKey.SetValue("SizeAll",  @"%SystemRoot%\cursors\aero_move.cur");
            cursorsKey.SetValue("SizeNESW",  @"%SystemRoot%\cursors\aero_nesw.cur");
            cursorsKey.SetValue("SizeNS",  @"%SystemRoot%\cursors\aero_ns.cur");
            cursorsKey.SetValue("SizeNSWE",  @"%SystemRoot%\cursors\aero_nswe.cur");
            cursorsKey.SetValue("SizeWE",  @"%SystemRoot%\cursors\aero_ew.cur");
            cursorsKey.SetValue("UpArrow",  @"%SystemRoot%\cursors\aero_up.cur");
            cursorsKey.SetValue("Wait",  @"%SystemRoot%\cursors\aero_busy.cur");
            User32.SystemParametersInfo(User32.SPI_SETCURSORS, 0, 0, User32.SPIF_UPDATEINIFILE | User32.SPIF_SENDCHANGE);
        }
    }
}
