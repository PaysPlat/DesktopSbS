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

namespace DeskTopSBS
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
                if (instance == null)
                {
                    instance = new DebugWindow();
                    instance.Show();
                }
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
            User32.SetWindowLong(this.Handle, User32.GWL_EXSTYLE, extendedStyle | User32.WS_EX_TRANSPARENT | User32.WS_EX_TOOLWINDOW);
        }

        public void UpdateMessage(string inMessage)
        {
            this.DebugText.Text = inMessage;
            User32.SetWindowPos(this.Handle, IntPtr.Zero,
               0,
               0,
               1920,
               1080,
               0);
        }
    }
}
