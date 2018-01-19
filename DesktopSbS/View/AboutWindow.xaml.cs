using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace DesktopSbS.View
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

        public string Version
        {
            get { return App.VERSION; }
        }

        public AboutWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.hideNextTime.IsChecked = Options.HideAboutOnStartup;
            
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

                Options.HideAboutOnStartup = this.hideNextTime.IsChecked == true;
            if (App.CurrentWindow != null)
            {
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
