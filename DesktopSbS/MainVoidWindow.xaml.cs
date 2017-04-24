using DesktopSbS.Hook;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace DesktopSbS
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

        public List<string> ExcludedApplications { get; private set; }

        public bool HideAboutOnStartup { get; set; }

        public int TaskBarHeight { get; private set; }

        public int ScreenWidth { get; private set; }

        public int ScreenHeight { get; private set; }

        public double ScreenScale { get; private set; } = 1;

        public bool ModeSbS { get; private set; } = true;

        private bool requestAbort = false;

        private bool is3DActive = false;
        public bool Is3DActive
        {
            get
            {
                return is3DActive;
            }
            set
            {
                if (this.is3DActive != value)
                {
                    this.is3DActive = value;

                    if (this.is3DActive)
                    {
#if !DEBUG
                        CursorWindow.HideCursors();
#endif
                        this.cursorSbS.RegisterThumbs();
                    }
                    else
                    {
                        this.cursorSbS.UnRegisterThumbs();
                        CursorWindow.ShowCursors();
                    }

                }
            }
        }

        private Thread threadUpdateWindows;

        private void init()
        {
            this.ModeSbS = this.options.GetBool("ModeSbS", true);
            this.ParallaxEffect = this.options.GetInt("ParallaxEffect");
            this.TaskBarHeight = this.options.GetInt("TaskBarHeight", 40);
            this.ExcludedApplications = this.options.GetListString("ExcludedApplications", new List<string>());
            this.HideAboutOnStartup = this.options.GetBool("HideAboutOnStartup", false);

            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                this.ScreenScale = graphics.DpiX / 96.0;
            }

            this.ScreenWidth = this.options.GetInt("ScreenWidth", (int)(SystemParameters.PrimaryScreenWidth * this.ScreenScale));
            this.ScreenHeight = this.options.GetInt("ScreenHeight", (int)(SystemParameters.PrimaryScreenHeight * this.ScreenScale));

            this.keyboardHook = new GlobalKeyboardHook();
            this.keyboardHook.KeyDown += KeyboardHook_KeyDown;

            this.cursorSbS = new CursorSbS();

            if (!this.HideAboutOnStartup)
            {
                AboutWindow.Instance.Show();
            }
            else
            {
                this.Is3DActive = true;
            }


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
                    case System.Windows.Input.Key.V:
                        this.ModeSbS = !this.ModeSbS;
                        this.hasToUpdate = true;
                        break;
                    case System.Windows.Input.Key.W:
                        this.ParallaxEffect--;
                        this.hasToUpdate = true;
                        break;
                    case System.Windows.Input.Key.X:
                        this.ParallaxEffect++;
                        this.hasToUpdate = true;
                        break;
                    case System.Windows.Input.Key.B:
                        this.Is3DActive = !this.Is3DActive;

                        break;
                    case System.Windows.Input.Key.F1:
                        this.Dispatcher.Invoke(AboutWindow.Instance.Show);
                        break;

                }

            }
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
                    this.ExcludedApplications.Contains(Path.GetFileName(User32.GetFilePath(tmpWindow.Handle))))
                {
                    this.tmpWindows.Clear();
                }
            }
            catch (Exception e)
            {

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

                if (tmpWindow.SourceRect.Left <= 0 && tmpWindow.SourceRect.Right >= this.ScreenWidth)
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
                    tmpWindow.RegisterThumbs();
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

                if (tmpWindow.SourceRect.Left <= 0 &&
                        tmpWindow.SourceRect.Right >= this.ScreenWidth &&
                        tmpWindow.SourceRect.Bottom - tmpWindow.SourceRect.Top == this.TaskBarHeight)
                {
                    taskBarWindow = tmpWindow;
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

        protected override void OnClosed(EventArgs e)
        {
           
            base.OnClosed(e);
            this.options.Set("ModeSbS", this.ModeSbS);
            this.options.Set("ParallaxEffect", this.ParallaxEffect);
            this.options.Set("HideAboutOnStartup", this.HideAboutOnStartup );
            this.options.saveToFile();

            this.Is3DActive = false;

        }
    }

}
