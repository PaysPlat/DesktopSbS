using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeskTopSBS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr handle;

        private List<WinSBS> windows = new List<WinSBS>();
        private List<WinSBS> tmpWindows = new List<WinSBS>();

        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }



        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {



            DateTime start = DateTime.Now;

            this.updateWindows();

            DateTime end = DateTime.Now;
            TimeSpan timeTaken = (TimeSpan)(end - start);


            //this.ImgLeft.Source = bmpSource;
            //this.ImgRight.Source = bmpSource;
        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            int extendedStyle = User32.GetWindowLong(hwnd, User32.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, User32.GWL_EXSTYLE, extendedStyle | User32.WS_EX_TRANSPARENT);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.handle = new WindowInteropHelper(this).Handle;

            IntPtr desktopHandle = User32.GetDesktopWindow();

        }

        private void updateWindows()
        {
            this.tmpWindows = new List<WinSBS>();
            User32.EnumWindows(windowFound, 0);

            bool hasModif = false;
            
            for (int i = 0; i < this.tmpWindows.Count; ++i)
            {
                if (this.windows.Count <= i ||
                    this.windows[i].Handle != this.tmpWindows[i].Handle )
                {
                    hasModif = true;
                    break;
                }
            }
            if (hasModif)
            {
                for (int i = 0; i < this.windows.Count; ++i)
                {
                    this.windows[i].UnRegisterThumbs();
                }
                for (int i = this.tmpWindows.Count - 1; i >= 0; --i)
                {
                    this.tmpWindows[i].RegisterThumbs(this.handle);
                    this.tmpWindows[i].UpdateThumbs();
                }
            }
            else
            {
                for (int i = 0; i < this.tmpWindows.Count; ++i)
                {
                    if (!this.windows[i].SourceRect.Equals(this.tmpWindows[i].SourceRect))
                    {
                        this.tmpWindows[i].UpdateThumbs();
                    }

                }
            }
            this.windows = this.tmpWindows;

           
        }

        private bool windowFound(IntPtr hwnd, int lParam)
        {

            DwmApi.WS winStyle = (DwmApi.WS)User32.GetWindowLongA(hwnd, DwmApi.GWL_STYLE);
            DwmApi.WSEX winStyleEx = (DwmApi.WSEX)User32.GetWindowLongA(hwnd, -20);

            User32.RECT sourceRect = new User32.RECT();

            User32.GetWindowRect(hwnd, ref sourceRect);

            if (this.handle != hwnd
                && !sourceRect.IsEmpty()
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
