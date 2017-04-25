using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DesktopSbS
{

    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {

        private static AboutWindow instance = null;

        public static AboutWindow Instance
        {
            get
            {
                if (instance == null) instance = new AboutWindow();
                return instance;
            }
        }

        public AboutWindow()
        {
            InitializeComponent();
            this.hideNextTime.IsChecked = App.CurrentWindow.HideAboutOnStartup;
            
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

            if (App.CurrentWindow != null)
            {
                App.CurrentWindow.HideAboutOnStartup = this.hideNextTime.IsChecked == true;
                App.CurrentWindow.Is3DActive = true;
            }
            instance = null;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
