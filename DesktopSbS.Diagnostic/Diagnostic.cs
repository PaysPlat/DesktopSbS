using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DesktopSbS.Interop;

namespace DesktopSbS.Diagnostic
{
    public class Diagnostic
    {

        private List<WinSbS> windowsToDisplay = new List<WinSbS>();
        private List<WinSbS> windowsRejected = new List<WinSbS>();

        public void WriteDiagnosticOnTextFile()
        {
            string diagnosticFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{DesktopSbS.App.APP_NAME}.Diagnostic.txt";

            StringBuilder sb = new StringBuilder();

            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = $"{Util.ExePath}{DesktopSbS.App.APP_NAME}.exe",
                    Arguments = "/read-settings",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            sb.AppendLine($"{DesktopSbS.App.APP_NAME} diagnostic v{DesktopSbS.App.VERSION}");
            sb.AppendLine($"Windows version: {Util.GetWindowsVersion()}");
            sb.AppendLine($"Date: {DateTime.Now}");
            sb.AppendLine();

            sb.AppendLine("Options");
            sb.AppendLine("------------------------------------");
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string setting = proc.StandardOutput.ReadLine();
                if (!string.IsNullOrEmpty(setting))
                {
                    sb.AppendLine(setting);
                }
            }

            User32.EnumWindows(windowFound, 0);

            sb.AppendLine();
            sb.AppendLine("Windows to display");
            sb.AppendLine("------------------------------------");
            for (int i = this.windowsToDisplay.Count - 1; i >= 0; --i)
            {
                WinSbS win = this.windowsToDisplay[i];
                sb.AppendLine($"{win.Title}, handle: {win.Handle}, {win.SourceRect}, ws: {win.WinStyle}, wsEx: {win.WinStyleEx}");
            }

            sb.AppendLine();
            sb.AppendLine("Windows rejected");
            sb.AppendLine("------------------------------------");
            for (int i = this.windowsRejected.Count - 1; i >= 0; --i)
            {
                WinSbS win = this.windowsRejected[i];
                sb.AppendLine($"{win.Title}, handle: {win.Handle}, {win.SourceRect}, ws: {win.WinStyle}, wsEx: {win.WinStyleEx}");
            }

            try
            {

                File.WriteAllText(diagnosticFilePath, sb.ToString());
                Process.Start(diagnosticFilePath);
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e} : {sb}", "Unable to save diagnostic file");
            }

        }

        private bool windowFound(IntPtr hwnd, int lParam)
        {

            StringBuilder sb = new StringBuilder(100);
            User32.GetWindowText(hwnd, sb, sb.Capacity);
            string title = sb.ToString();

            // don't consider our own windows 
            if (title == "ThumbWindows") return true;

            WS winStyle = (WS)User32.GetWindowLongA(hwnd, User32.GWL_STYLE);
            WSEX winStyleEx = (WSEX)User32.GetWindowLongA(hwnd, User32.GWL_EXSTYLE);

            RECT sourceRect = new RECT();


            User32.GetWindowRect(hwnd, ref sourceRect);

            WinSbS win = new WinSbS(hwnd);
            win.SourceRect = sourceRect;
            win.Title = title;
            win.WinStyle = winStyle;
            win.WinStyleEx = winStyleEx;

            int cloaked = 0;
            DwmApi.DwmGetWindowAttribute(hwnd, DwmApi.DwmWindowAttribute.DWMWA_CLOAKED, out cloaked, sizeof(int));

            if (cloaked == 0
                && !sourceRect.IsSize0()
                && (winStyle & WS.WS_VISIBLE) == WS.WS_VISIBLE
                && (winStyle & WS.WS_ICONIC) == 0
                && (winStyle & WS.WS_DISABLED) == 0)
            {
                this.windowsToDisplay.Add(win);
            }
            else
            {
                this.windowsRejected.Add(win);
            }

            return true; //continue enumeration
        }


    }
}
