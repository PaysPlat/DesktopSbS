using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DesktopSbS
{
    public static class User32
    {
        private const string dll = "user32.dll";

        #region Cursors

        public const int SPI_SETCURSORS = 0x0057;
        public const int SPIF_UPDATEINIFILE = 0x01;
        public const int SPIF_SENDCHANGE = 0x02;

        [DllImport(dll)]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);
        [DllImport(dll)]
        public static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport(dll)]
        public static extern int ShowCursor(bool bShow);

        [DllImport(dll)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        #endregion

        #region Styles

        public const int GWL_EXSTYLE = -20;
        public const int GWL_STYLE = -16;

        [DllImport(dll)]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport(dll)]
        public static extern ulong GetWindowLongA(IntPtr hWnd, int nIndex);

        [DllImport(dll)]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        #endregion

        #region Monitor

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, DeviceCap nIndex);

        public static void GetDpi(this System.Windows.Forms.Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        }

        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In]System.Drawing.Point pt, [In]uint dwFlags);

        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX, [Out]out uint dpiY);



        #endregion

        #region Window

        public static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hwnd, ref placement);
            return placement;
        }

        [DllImport(dll, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(
            IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public static readonly IntPtr NOT_TOPMOST = new IntPtr(-2);

        [DllImport(dll)]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int Y, int cx, int cy, SWP wFlags);

        [DllImport(dll)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport(dll)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport(dll)]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        [DllImport(dll)]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport(dll)]
        public static extern int EnumWindows(EnumWindowsCallback lpEnumFunc, int lParam);
        public delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);

        #endregion

        #region Handle info


        [DllImport(dll)]
        public static extern void GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport(dll)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out uint processId);

        public static string GetFilePath(IntPtr hwnd)
        {
            try
            {
                uint pid = 0;
                GetWindowThreadProcessId(hwnd, out pid);
                Process proc = Process.GetProcessById((int)pid); //Gets the process by ID.
                return proc.MainModule.FileName.ToString();   //Returns the path.
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #endregion



    }
}
