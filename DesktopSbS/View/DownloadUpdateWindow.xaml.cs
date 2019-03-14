using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace DesktopSbS.View
{

    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class DownloadUpdateWindow : Window, INotifyPropertyChanged
    {
        private WebClient webClient;


        private int _Progress;
        public int Progress
        {
            get { return this._Progress; }
            set
            {
                if (this._Progress != value)
                {
                    this._Progress = value;
                    this.RaisePropertyChanged();
                }
            }
        }


        public DownloadUpdateWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            webClient = new WebClient();

        }


        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            webClient.CancelAsync();
        }


        public void DownloadAndStart(string url, string filePath)
        {
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileAsync(new Uri(url), filePath, filePath);
            this.ShowDialog();
            
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.UserState is string filePath
                && File.Exists(filePath))
            {
                if (!e.Cancelled
                    && e.Error == null
                    )
                {
                    Process.Start(filePath);
                }
                else
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch
                    {

                    }
                }
            }
            this.Close();
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => this.Progress = e.ProgressPercentage);
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
