using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace DesktopSbS.View
{

    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class UpdateAvailableWindow : Window
    {

        public Version NewVersion { get; set; }

        public DateTime? ReleaseDate { get; set; }
       
        public UpdateAvailableWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }


        private void SaveCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        
    }
}
