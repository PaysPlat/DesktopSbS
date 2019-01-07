﻿using System;
using System.Windows;
using System.Windows.Interop;
using DesktopSbS.Interop;

namespace DesktopSbS.View
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        private static DebugWindow instance = null;

        public static DebugWindow Instance
        {
            get
            {
#if DEBUG
                if (instance == null)
                {
                    instance = new DebugWindow();
                    instance.Show();
                }
#endif
                return instance;
            }
        }


        public IntPtr Handle { get; private set; }

        public DebugWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            this.Handle = new WindowInteropHelper(this).Handle;
            int extendedStyle = User32.GetWindowLong(this.Handle, User32.GWL_EXSTYLE);
            User32.SetWindowLong(this.Handle, User32.GWL_EXSTYLE, extendedStyle | (int)WSEX.WS_EX_TRANSPARENT | (int)WSEX.WS_EX_TOOLWINDOW);
        }

        public void UpdateMessage(string inMessage)
        {
            this.DebugText.Text = inMessage;
            User32.SetWindowPos(this.Handle, IntPtr.Zero,
               Options.ScreenSrcBounds.Left,
               Options.ScreenSrcBounds.Top,
               Options.ScreenSrcBounds.Width,
               Options.ScreenSrcBounds.Height,
               SWP.SWP_ASYNCWINDOWPOS);
        }
    }
}
