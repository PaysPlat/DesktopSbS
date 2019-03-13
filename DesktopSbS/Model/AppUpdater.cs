using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopSbS.Model
{
    public static class AppUpdater
    {

        public static void CheckForUpdates(bool automaticMode)
        {
            RequestObject updateRequest = new RequestObject
            {
                WebRequest = WebRequest.CreateHttp(App.VERSIONS_URL),
                Parameter = automaticMode,
                OnSuccess = ReadUpdateResponseStream,
                OnFailure = p =>
                {
                    if ((p as bool?) == false)
                    {
                        MessageBox.Show($"Update URL \"{App.VERSIONS_URL}\" could not be opened.", "Network error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            };
            updateRequest.BeginGetResponse(ResponseReceived);
        }


        private static void ResponseReceived(IAsyncResult asyncResult)
        {
            if (asyncResult.IsCompleted
                && asyncResult.AsyncState is RequestObject requestObject
                && requestObject.WebRequest.HaveResponse
                && requestObject.OnSuccess != null)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)requestObject.WebRequest.EndGetResponse(asyncResult);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream streamResponse = response.GetResponseStream();
                        requestObject.OnSuccess(streamResponse, requestObject.Parameter);

                        // Close the stream object
                        streamResponse.Close();

                    }
                    // Release the HttpWebResponse
                    response.Close();
                }
                catch (WebException exception)
                {
                    requestObject.OnFailure?.Invoke(requestObject.Parameter);
                }
            }
        }


        private static void ReadUpdateResponseStream(Stream updateStream, object parameter)
        {
            StreamReader streamReader = new StreamReader(updateStream);

            bool automaticMode = (parameter as bool?) == true;

            Version referenceVersion = automaticMode ? Options.LatestVersion : Options.CurrentVersion;

            Tuple<Version, DateTime?, string> maxAvailableVersion = new Tuple<Version, DateTime?, string>(referenceVersion, null, null);

            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                string[] tab = line.Split('\t');
                if (tab.Length >= 3)
                {
                    if (Version.TryParse(tab[1], out Version version) && version > maxAvailableVersion.Item1)
                    {
                        DateTime? versionDate = null;
                        if (DateTime.TryParse(tab[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                        {
                            versionDate = date;
                        }
                        maxAvailableVersion = new Tuple<Version, DateTime?, string>(version, versionDate, tab[2]);
                    }
                }
            }
            streamReader.Close();

            if (maxAvailableVersion.Item1 > referenceVersion)
            {
                Options.LatestVersion = maxAvailableVersion.Item1;
                Options.Save();

                bool uawResult = false;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    View.UpdateAvailableWindow uaw = new View.UpdateAvailableWindow
                    {
                        NewVersion = maxAvailableVersion.Item1,
                        ReleaseDate = maxAvailableVersion.Item2,
                    };
                    uawResult = uaw.ShowDialog() == true;
                });

                if (uawResult)
                {
                    string fileName = Path.GetFileName(maxAvailableVersion.Item3);
                    string extension = Path.GetExtension(fileName);
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        FileName = fileName,
                        OverwritePrompt = true,
                        AddExtension = true,
                        Filter = $"{extension.Replace(".", string.Empty).ToUpper()} files (*{extension})|*{extension}",
                        DefaultExt = extension

                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string filePath = saveFileDialog.FileName;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            View.DownloadUpdateWindow duw = new View.DownloadUpdateWindow();
                            duw.DownloadAndStart(maxAvailableVersion.Item3, filePath);
                        });
                    }
                }
            }
            else if (!automaticMode)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    View.UpdateAvailableWindow uaw = new View.UpdateAvailableWindow
                    {
                        NewVersion = null,
                        ReleaseDate = null
                    };
                    uaw.ShowDialog();
                });
            }

        }



        class RequestObject
        {
            public HttpWebRequest WebRequest { get; set; }

            public object Parameter { get; set; }

            public Action<Stream, object> OnSuccess { get; set; }

            public Action<object> OnFailure { get; set; }

            public IAsyncResult BeginGetResponse(AsyncCallback responseReceived)
            {
                return this.WebRequest.BeginGetResponse(responseReceived, this);
            }

        }

    }
}

