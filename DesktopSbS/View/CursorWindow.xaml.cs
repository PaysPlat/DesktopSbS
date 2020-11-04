using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using DesktopSbS.Interop;

namespace DesktopSbS.View
{
    /// <summary>
    /// Interaction logic for CursorWindow.xaml
    /// </summary>
    public partial class CursorWindow : Window
    {
        private static Dictionary<IntPtr, Tuple<BitmapImage, POINT>> cursors;

        public IntPtr Handle { get; private set; }

        private static Tuple<BitmapImage, POINT> defaultCursor;
        private Tuple<BitmapImage, POINT> currentCursor;
        private IntPtr currentCursorType = IntPtr.Zero;

        private object lockSetCursor = new object();

        static CursorWindow()
        {
            cursors = new Dictionary<IntPtr, Tuple<BitmapImage, POINT>>();

            defaultCursor = new Tuple<BitmapImage, POINT>(new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/aero_arrow.png", UriKind.Relative)), new POINT());
            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_ARROW), defaultCursor);

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_APPSTARTING), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/aero_working.png", UriKind.Relative)),
                new POINT(0, 8)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_CROSS), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/crosshair.png", UriKind.Relative)),
                new POINT(16, 16)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_HAND), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/aero_link.png", UriKind.Relative)),
                new POINT(7, 0)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_HELP), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/aero_helpsel.png", UriKind.Relative)),
                new POINT(0, 0)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_IBEAM), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/ibeam.png", UriKind.Relative)),
                new POINT(16, 16)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_NO), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/aero_unavail.png", UriKind.Relative)),
                new POINT(8, 8)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_SIZE), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/aero_move.png", UriKind.Relative)),
                new POINT(11, 11)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_SIZEALL), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/aero_move.png", UriKind.Relative)),
                new POINT(11, 11)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_SIZENESW), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/aero_nesw.png", UriKind.Relative)),
                new POINT(8, 8)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_SIZENS), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/aero_ns.png", UriKind.Relative)),
                new POINT(4, 11)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_SIZENWSE), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/aero_nwse.png", UriKind.Relative)),
                new POINT(8, 8)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_SIZEWE), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/aero_ew.png", UriKind.Relative)),
                new POINT(11,4)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_UPARROW), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/aero_up.png", UriKind.Relative)),
                new POINT(4, 0)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_WAIT), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/Cursors/aero_busy.png", UriKind.Relative)),
                new POINT(16, 16)));


        }


        public static void HideCursors()
        {
            string blankCurPath = $@"{Util.ExePath}Resources\blank.cur";
            RegistryKey cursorsKey = Registry.CurrentUser.OpenSubKey("Control Panel").OpenSubKey("Cursors", true);
            cursorsKey.SetValue("AppStarting",blankCurPath);
            cursorsKey.SetValue("Arrow",blankCurPath);
            cursorsKey.SetValue("Crosshair",blankCurPath);
            cursorsKey.SetValue("Hand",blankCurPath);
            cursorsKey.SetValue("Help",blankCurPath);
            cursorsKey.SetValue("IBeam",blankCurPath);
            cursorsKey.SetValue("No",blankCurPath);
            cursorsKey.SetValue("NWPen",blankCurPath);
            cursorsKey.SetValue("SizeAll",blankCurPath);
            cursorsKey.SetValue("SizeNESW",blankCurPath);
            cursorsKey.SetValue("SizeNS",blankCurPath);
            cursorsKey.SetValue("SizeNWSE",blankCurPath);
            cursorsKey.SetValue("SizeWE",blankCurPath);
            cursorsKey.SetValue("UpArrow",blankCurPath);
            cursorsKey.SetValue("Wait",blankCurPath);
            User32.SystemParametersInfo(User32.SPI_SETCURSORS, 0, 0, User32.SPIF_UPDATEINIFILE | User32.SPIF_SENDCHANGE);

        }
        public static void ShowCursors()
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

        public CursorWindow()
        {
            InitializeComponent();
        }



        public POINT SetCursor(IntPtr inCursorType)
        {
            lock (lockSetCursor)
            {
                if (inCursorType != this.currentCursorType)
                {
                    this.currentCursorType = inCursorType;
                    if (!cursors.TryGetValue(this.currentCursorType, out this.currentCursor))
                    {
                        this.currentCursor = defaultCursor;
                    }
                    this.CursorImage.Source = this.currentCursor.Item1;
                }
                return this.currentCursor.Item2;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            this.Handle = new WindowInteropHelper(this).Handle;
            int extendedStyle = User32.GetWindowLong(this.Handle, User32.GWL_EXSTYLE);
            User32.SetWindowLong(this.Handle, User32.GWL_EXSTYLE, extendedStyle | (int)WSEX.WS_EX_TRANSPARENT | (int)WSEX.WS_EX_TOOLWINDOW);
        }
    }
}
