using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DesktopSbS
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

        static CursorWindow()
        {
            cursors = new Dictionary<IntPtr, Tuple<BitmapImage, POINT>>();

            defaultCursor = new Tuple<BitmapImage, POINT>(new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/aero_arrow_SbS.png", UriKind.Relative)), new POINT());
            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_ARROW), defaultCursor);

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_APPSTARTING), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/aero_working_SbS.png", UriKind.Relative)),
                new POINT(0, 8)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_CROSS), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/cross_SbS.png", UriKind.Relative)),
                new POINT(8, 16)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_HAND), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/aero_link_SbS.png", UriKind.Relative)),
                new POINT(3, 0)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_HELP), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/aero_helpsel_SbS.png", UriKind.Relative)),
                new POINT(0, 0)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_IBEAM), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/text_SbS.png", UriKind.Relative)),
                new POINT(8, 16)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_NO), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/aero_unavail_SbS.png", UriKind.Relative)),
                new POINT(4, 8)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_SIZE), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/aero_move_SbS.png", UriKind.Relative)),
                new POINT(5, 11)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_SIZEALL), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/aero_move_SbS.png", UriKind.Relative)),
                new POINT(5, 11)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_SIZENESW), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/aero_nesw_SbS.png", UriKind.Relative)),
                new POINT(4, 9)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_SIZENS), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/aero_ns_SbS.png", UriKind.Relative)),
                new POINT(2, 11)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_SIZENWSE), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/aero_nwse_SbS.png", UriKind.Relative)),
                new POINT(4, 8)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_SIZEWE), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/aero_ew_SbS.png", UriKind.Relative)),
                new POINT(6,4)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_UPARROW), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/aero_up_SbS.png", UriKind.Relative)),
                new POINT(2, 0)));

            cursors.Add(User32.LoadCursor(IntPtr.Zero, (int)CURSOR_TYPE.IDC_WAIT), new Tuple<BitmapImage, POINT>(
                new BitmapImage(new Uri(@"/DesktopSbS;component/Resources/aero_busy_SbS.png", UriKind.Relative)),
                new POINT(8, 16)));


        }

        public CursorWindow()
        {
            InitializeComponent();
        }

        public POINT SetCursor(IntPtr inCursorType)
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

        protected override void OnSourceInitialized(EventArgs e)
        {
            this.Handle = new WindowInteropHelper(this).Handle;
            int extendedStyle = User32.GetWindowLong(this.Handle, User32.GWL_EXSTYLE);
            User32.SetWindowLong(this.Handle, User32.GWL_EXSTYLE, extendedStyle | (int)WSEX.WS_EX_TRANSPARENT | (int)WSEX.WS_EX_TOOLWINDOW);
        }
    }
}
