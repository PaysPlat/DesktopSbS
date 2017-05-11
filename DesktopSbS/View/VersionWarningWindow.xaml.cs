using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace DesktopSbS.View
{

    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class VersionWarningWindow : Window
    {

       
        public VersionWarningWindow()
        {
            InitializeComponent();
             
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

      
        

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
