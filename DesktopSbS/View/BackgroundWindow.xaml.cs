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
    public partial class BackgroundWindow : Window
    {
        public IntPtr Handle { get; private set; }



        public BackgroundWindow()
        {
            InitializeComponent();

            this.Left = Options.ScreenDestBounds.Left;
            this.Top = Options.ScreenDestBounds.Top;
            this.Width = Options.ScreenDestBounds.Width;
            this.Height = Options.ScreenDestBounds.Height;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            this.Handle = new WindowInteropHelper(this).Handle;
            int extendedStyle = User32.GetWindowLong(this.Handle, User32.GWL_EXSTYLE);
            User32.SetWindowLong(this.Handle, User32.GWL_EXSTYLE, extendedStyle | (int)WSEX.WS_EX_TRANSPARENT | (int)WSEX.WS_EX_TOOLWINDOW);
        }
    }
}
