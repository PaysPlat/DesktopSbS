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
               0,
               0,
               App.Current.ScreenWidth,
               App.Current.ScreenHeight,
               SWP.SWP_ASYNCWINDOWPOS);
        }
    }
}
