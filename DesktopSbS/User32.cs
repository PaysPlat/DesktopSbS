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

        #region Window

        public static RECT GetMonitorRect(POINT pt)
        {
            IntPtr mh = MonitorFromPoint(pt, 0);
            MONITORINFO mi = new MONITORINFO();
            mi.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            mi.dwFlags = 0;
            bool result = GetMonitorInfo(mh, ref mi);
            return mi.rcMonitor;
        }

        [DllImport(dll, CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hmonitor,ref MONITORINFO info);

        [DllImport(dll, ExactSpelling = true)]
        public static extern IntPtr MonitorFromPoint(POINT pt, int flags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szDevice = new char[32];
        }


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

        public const int SWP_ASYNCWINDOWPOS = 0x4000;
        public const int SWP_FRAMECHANGED = 0x20;
        public const int SWP_NOSIZE = 0x1;

        [DllImport(dll)]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

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
