using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using DesktopSbS.Hook;
using DesktopSbS.Interop;
using DesktopSbS.Model;

namespace DesktopSbS.View
{
    /// <summary>
    /// Logique d'interaction pour MainVoidWindow.xaml
    /// </summary>
    public partial class MainVoidWindow : Window
    {
        public MainVoidWindow()
        {
            InitializeComponent();
            this.init();
        }
        private const int renderPassTimeMs = 20;
        private GlobalKeyboardHook keyboardHook;

        private List<WinSbS> windows = new List<WinSbS>();
        private List<WinSbS> tmpWindows = new List<WinSbS>();

        private bool hasToUpdate = false;

        private bool isAboutOpened = false;

        private CursorSbS cursorSbS;

        private BackgroundWindow backgroundWindow;

        private bool requestAbort = false;


        private bool is3DActive = false;
        public bool Is3DActive
        {
            get
            {
                return this.is3DActive;
            }
            set
            {
                if (this.is3DActive != value)
                {
                    this.is3DActive = value;
                    this.cursorSbS.Is3DActive = value;
                    if (this.is3DActive)
                    {
                        this.backgroundWindow.Show();
                    }
                    else
                    {
                        this.backgroundWindow.Hide();
                    }
                }
            }
        }

        private Thread threadUpdateWindows;

        private void init()
        {
            if (!Options.HideAboutOnStartup)
            {
                AboutWindow.Instance.ShowDialog();
            }

            this.keyboardHook = new GlobalKeyboardHook();
            this.keyboardHook.KeyDown += KeyboardHook_KeyDown;

            this.cursorSbS = new CursorSbS();

            this.backgroundWindow = new BackgroundWindow();

            this.Is3DActive = true;

            this.threadUpdateWindows = new Thread(asyncUpdateWindows);
            this.threadUpdateWindows.IsBackground = true;
            this.threadUpdateWindows.Start();

        }

        private void KeyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            Key key = e.Key == Key.System ? e.SystemKey : e.Key;
            ShortcutCommands? shortcutFound = Options.KeyboardShortcuts?.
                FirstOrDefault(t =>
                e.KeyboardDevice.Modifiers == t.Item1
                && key == t.Item2)?.Item3;

            if (shortcutFound.HasValue)
            {
                switch (shortcutFound.Value)
                {
                    case ShortcutCommands.About:
                        this.Dispatcher.Invoke(() =>
                        {
                            AboutWindow.Instance.hideNextTime.IsChecked = false;
                            this.Is3DActive = false;
                            this.isAboutOpened = true;
                            AboutWindow.Instance.ShowDialog();
                            this.isAboutOpened = false;
                            if (App.Current != null) this.Is3DActive = true;
                        });
                        break;
                    case ShortcutCommands.DecreaseParallax:
                        Options.ParallaxEffect--;
                        this.hasToUpdate = true;
                        break;
                    case ShortcutCommands.IncreaseParallax:
                        Options.ParallaxEffect++;
                        this.hasToUpdate = true;
                        break;
                    case ShortcutCommands.ShutDown:
                        this.requestAbort = true;
                        break;
                    case ShortcutCommands.SwitchSbSMode:
                        Options.ModeSbS = !Options.ModeSbS;
                        this.hasToUpdate = true;
                        break;
                    case ShortcutCommands.Pause3D:
                        this.Is3DActive = !this.Is3DActive;
                        break;
                    case ShortcutCommands.HideDestCursor:
                        Options.HideDestCursor = !Options.HideDestCursor;
                        this.cursorSbS.Is3DActive = this.Is3DActive;
                        break;
                    case ShortcutCommands.KeepRatio:
                        Options.KeepRatio = !Options.KeepRatio;
                        this.hasToUpdate = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private void asyncUpdateWindows()
        {

            while (!this.requestAbort)
            {
                DateTime start = DateTime.Now;
                this.updateWindows();
                DateTime end = DateTime.Now;

                int elapsedMS = (int)(end - start).TotalMilliseconds;
                // this.Dispatcher.Invoke(()=> DebugWindow.Instance.UpdateMessage($"Elapsed ms: {elapsedMS}"));
                Thread.Sleep(Math.Max(0, renderPassTimeMs - elapsedMS));
            }

            this.Dispatcher.Invoke(App.Current.Shutdown);
        }


        private void updateWindows()
        {
            this.tmpWindows = new List<WinSbS>();

            if (this.Is3DActive)
            {
                User32.EnumWindows(windowFound, 0);
            }

            int updateAllIndex = -1;

            int offsetLevel = 0;

            WinSbS taskBarWindow = null;

            WinSbS tmpWindow = this.tmpWindows.FirstOrDefault(w => w.SourceRect.IsMaximized());
            try
            {
                if (tmpWindow != null &&
                    Options.ExcludedApplications?.Contains(Path.GetFileName(User32.GetFilePath(tmpWindow.Handle))) == true)
                {
                    this.tmpWindows.Clear();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e}");
            }


            for (int i = this.tmpWindows.Count - 1; i >= 0; --i)
            {
                tmpWindow = this.tmpWindows[i];

                if (i < this.tmpWindows.Count - 1)
                {
                    tmpWindow.Owner = this.tmpWindows[i + 1];
                }
                else
                {
                    tmpWindow.Owner = null;
                }


                if (tmpWindow.SourceRect.Left <= Options.AreaSrcBounds.Left && tmpWindow.SourceRect.Right >= Options.AreaSrcBounds.Right)
                {
                    offsetLevel = 0;
                }
                else
                {
                    offsetLevel++;
                }
                tmpWindow.OffsetLevel = offsetLevel;

                int oldIndex = this.windows.FindIndex(w => w.Handle == tmpWindow.Handle);
                if (oldIndex < 0)
                {
                    App.Current.Dispatcher.Invoke(tmpWindow.RegisterThumbs);
                }
                else
                {
                    tmpWindow.CopyThumbInstances(this.windows[oldIndex]);

                    if (updateAllIndex < 0 && this.windows[oldIndex].Owner?.Handle != tmpWindow.Owner?.Handle)
                    {
                        updateAllIndex = i;
                    }
                    else if (!this.windows[oldIndex].SourceRect.Equals(tmpWindow.SourceRect))
                    {
                        tmpWindow.UpdateThumbs();
                    }
                    this.windows.RemoveAt(oldIndex);

                }

                if (tmpWindow.SourceRect.Left <= Options.ScreenSrcBounds.Left &&
                        tmpWindow.SourceRect.Right >= Options.ScreenSrcBounds.Right &&
                        tmpWindow.SourceRect.Bottom - tmpWindow.SourceRect.Top == Options.ScreenSrcBounds.Height - Options.ScreenSrcWorkspace.Height)
                {

                    taskBarWindow = tmpWindow;
                }

            }
            for (int i = 0; i < this.windows.Count; ++i)
            {
                App.Current.Dispatcher.Invoke(this.windows[i].UnRegisterThumbs);
            }

            this.windows = this.tmpWindows;

            if (this.isAboutOpened)
            {
                return;
            }


            if (this.hasToUpdate)
            {
                Options.ComputedVariables.UpdateVariables();
                updateAllIndex = this.windows.Count - 1;
                this.hasToUpdate = false;
            }

            if (updateAllIndex > -1)
            {
                for (int i = updateAllIndex; i >= 0; --i)
                {
                    if (this.windows[i] != taskBarWindow) this.windows[i].UpdateThumbs();
                }
            }

            taskBarWindow?.UpdateThumbs(true);
            this.cursorSbS.UpdateThumbs((this.windows.Any() ? this.windows.Max(w => w.OffsetLevel) : 0) + 1);


        }

        private bool windowFound(IntPtr hwnd, int lParam)
        {

            StringBuilder sb = new StringBuilder(100);
            User32.GetWindowText(hwnd, sb, sb.Capacity);
            string title = sb.ToString();

            // don't consider our own windows 
            if (title == "ThumbWindows") return true;

            WS winStyle = (WS)User32.GetWindowLongA(hwnd, User32.GWL_STYLE);
            WSEX winStyleEx = (WSEX)User32.GetWindowLongA(hwnd, User32.GWL_EXSTYLE);

            RECT sourceRect = new RECT();
            User32.GetWindowRect(hwnd, ref sourceRect);

            // Detection of cloaked win10 windows => not rendered 
            int cloaked = 0;
            DwmApi.DwmGetWindowAttribute(hwnd, DwmApi.DwmWindowAttribute.DWMWA_CLOAKED, out cloaked, sizeof(int));

            if (cloaked == 0
                && !sourceRect.IsSize0()
                && (winStyle & WS.WS_VISIBLE) == WS.WS_VISIBLE
                && (winStyle & WS.WS_ICONIC) == 0
                && (winStyle & WS.WS_DISABLED) == 0)
            {
                WinSbS win = new WinSbS(hwnd);
                win.SourceRect = sourceRect;
                win.Title = title;

                this.tmpWindows.Add(win);
            }

            return true; //continue enumeration
        }

        protected override void OnClosed(EventArgs e)
        {

            base.OnClosed(e);
            Options.Save();
            this.Is3DActive = false;

        }
    }

}
