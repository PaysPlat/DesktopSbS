using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DesktopSbS.Hook;
using Europe_CommonWPF;
using Europe_CommonWPF.Business;
using Microsoft.Win32;

namespace DesktopSbS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current
        {
            get { return Application.Current as App; }
        }

        private ConfFile options = new ConfFile("options.ini");

        private GlobalKeyboardHook keyboardHook;

        private List<WinSbS> windows = new List<WinSbS>();
        private List<WinSbS> tmpWindows = new List<WinSbS>();

        private bool hasToUpdate = false;

        private CursorSbS cursorSbS;

        private int parallaxEffect;
        public int ParallaxEffect
        {
            get { return this.parallaxEffect; }
            private set { this.parallaxEffect = Math.Max(0, value); }
        }

        public int TaskBarHeight { get; private set; }

        public int ScreenWidth { get; private set; }

        public int ScreenHeight { get; private set; }

        public double ScreenScale { get; private set; } = 1;

        private bool requestAbort = false;

        private Thread threadUpdateWindows;
        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);

            this.ParallaxEffect = this.options.GetInt("ParallaxEffect");
            this.TaskBarHeight = this.options.GetInt("TaskBarHeight", 40);

            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                this.ScreenScale = graphics.DpiX / 96.0;
            }

            this.ScreenWidth = (int)(SystemParameters.PrimaryScreenWidth * this.ScreenScale); //this.options.GetInt("ScreenWidth", 1920);
            this.ScreenHeight = (int)(SystemParameters.PrimaryScreenHeight * this.ScreenScale); //this.options.GetInt("ScreenHeight", 1080);



            //foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            //{
            //    uint x, y;
            //    screen.GetDpi(DpiType.Effective, out x, out y);
            //}

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
            ModifierKeys ctrlAlt = ModifierKeys.Control | ModifierKeys.Alt;
            if ((e.KeyboardDevice.Modifiers & ctrlAlt) >= ctrlAlt)
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.C:
                        this.requestAbort = true;
                        break;
                    case System.Windows.Input.Key.W:
                        this.ParallaxEffect--;
                        this.hasToUpdate = true;
                        break;
                    case System.Windows.Input.Key.X:
                        this.ParallaxEffect++;
                        this.hasToUpdate = true;
                        break;

                }

            }
        }

        protected override void OnExit(ExitEventArgs e)
        {

            base.OnExit(e);
            this.options.Set("ParallaxEffect", this.ParallaxEffect);
            this.options.saveToFile();

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
                // this.Dispatcher.Invoke(()=> DebugWindow.Instance.UpdateMessage($"Elapsed ms: {elapsedMS}"));
                Thread.Sleep(Math.Max(0, 20 - elapsedMS));
            }
            this.Dispatcher.Invoke(this.Shutdown);
        }


        private void updateWindows()
        {
            this.tmpWindows = new List<WinSbS>();
            User32.EnumWindows(windowFound, 0);

            int updateAllIndex = -1;

            int offsetLevel = 0;

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

                if (tmpWindows[i].SourceRect.Left <= 0 && tmpWindows[i].SourceRect.Right >= this.ScreenWidth)
                {
                    offsetLevel = 0;
                }
                else
                {
                    offsetLevel++;
                }
                tmpWindows[i].OffsetLevel = offsetLevel;

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

            if (this.hasToUpdate)
            {
                updateAllIndex = this.windows.Count - 1;
                this.hasToUpdate = false;
            }

            if (updateAllIndex > -1)
            {
                for (int i = updateAllIndex; i >= 0; --i)
                {
                    this.windows[i].UpdateThumbs();
                }
            }

            this.cursorSbS.UpdateThumbs(this.windows.Max(w => w.OffsetLevel) + 1);
        }

        private bool windowFound(IntPtr hwnd, int lParam)
        {

            StringBuilder sb = new StringBuilder(100);
            User32.GetWindowText(hwnd, sb, sb.Capacity);
            string title = sb.ToString();

            if (title == "ThumbWindows") return true;

            WS winStyle = (WS)User32.GetWindowLongA(hwnd, User32.GWL_STYLE);
            WSEX winStyleEx = (WSEX)User32.GetWindowLongA(hwnd, User32.GWL_EXSTYLE);

            RECT sourceRect = new RECT();

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
            cursorsKey.SetValue("AppStarting", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("Arrow", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("Crosshair", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("Hand", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("Help", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("IBeam", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("No", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("NWPen", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("SizeAll", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("SizeNESW", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("SizeNS", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("SizeNWSE", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("SizeWE", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("UpArrow", $@"{Util.ExePath}Resources\blank.cur");
            cursorsKey.SetValue("Wait", $@"{Util.ExePath}Resources\blank.cur");
            User32.SystemParametersInfo(User32.SPI_SETCURSORS, 0, 0, User32.SPIF_UPDATEINIFILE | User32.SPIF_SENDCHANGE);

        }
        private void showCursors()
        {
            RegistryKey cursorsKey = Registry.CurrentUser.OpenSubKey("Control Panel").OpenSubKey("Cursors", true);
            cursorsKey.SetValue("AppStarting", @"%SystemRoot%\cursors\aero_working.ani");
            cursorsKey.SetValue("Arrow", @"%SystemRoot%\cursors\aero_arrow.cur");
            cursorsKey.SetValue("Crosshair", "");
            cursorsKey.SetValue("Hand", @"%SystemRoot%\cursors\aero_link.cur");
            cursorsKey.SetValue("Help", @"%SystemRoot%\cursors\aero_helpsel.cur");
            cursorsKey.SetValue("IBeam", "");
            cursorsKey.SetValue("No", @"%SystemRoot%\cursors\aero_unavail.cur");
            cursorsKey.SetValue("NWPen", @"%SystemRoot%\cursors\aero_pen.cur");
            cursorsKey.SetValue("SizeAll", @"%SystemRoot%\cursors\aero_move.cur");
            cursorsKey.SetValue("SizeNESW", @"%SystemRoot%\cursors\aero_nesw.cur");
            cursorsKey.SetValue("SizeNS", @"%SystemRoot%\cursors\aero_ns.cur");
            cursorsKey.SetValue("SizeNWSE", @"%SystemRoot%\cursors\aero_nwse.cur");
            cursorsKey.SetValue("SizeWE", @"%SystemRoot%\cursors\aero_ew.cur");
            cursorsKey.SetValue("UpArrow", @"%SystemRoot%\cursors\aero_up.cur");
            cursorsKey.SetValue("Wait", @"%SystemRoot%\cursors\aero_busy.ani");
            User32.SystemParametersInfo(User32.SPI_SETCURSORS, 0, 0, User32.SPIF_UPDATEINIFILE | User32.SPIF_SENDCHANGE);
        }
    }
}
