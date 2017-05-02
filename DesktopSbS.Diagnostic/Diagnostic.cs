using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopSbS.Diagnostic
{
    public class Diagnostic
    {

        private List<WinSbS> windowsToDisplay = new List<WinSbS>();
        private List<WinSbS> windowsRejected = new List<WinSbS>();

        public void WriteDiagnosticOnTextFile()
        {
            string diagnosticFilePath = Util.ExePath + "diagnostic.txt";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("DesktopSbS diagnostic");
            sb.AppendLine($"Date: {DateTime.Now}");
            sb.AppendLine();

            sb.AppendLine("Options");
            sb.AppendLine("------------------------------------");
            sb.AppendLine($"ScreenWidth: {DesktopSbS.Options.ScreenWidth}");
            sb.AppendLine($"ScreenHeight: {DesktopSbS.Options.ScreenHeight}");
            sb.AppendLine($"ScreenScale: {DesktopSbS.Options.ScreenScale}");
            sb.AppendLine($"TaskBarHeight: {DesktopSbS.Options.TaskBarHeight}");
            sb.AppendLine($"HideAboutOnStartup: {DesktopSbS.Options.HideAboutOnStartup}");
            sb.AppendLine($"ModeSbS: {DesktopSbS.Options.ModeSbS}");
            sb.AppendLine($"ParallaxEffect: {DesktopSbS.Options.ParallaxEffect}");
            sb.AppendLine($"ExcludedApplications: {DesktopSbS.Options.ExcludedApplications}");

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
                MessageBox.Show(sb.ToString(), "Unable to save diagnostic file");
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


            if (!sourceRect.IsEmpty()
                && (winStyle & WS.WS_VISIBLE) == WS.WS_VISIBLE
                && (winStyle & WS.WS_ICONIC) == 0
                && (winStyle & WS.WS_DISABLED) == 0
                && (winStyleEx & WSEX.WS_EX_NOREDIRECTIONBITMAP) == 0)
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
