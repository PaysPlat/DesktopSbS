using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DeskTopSBS
{
   public static class User32
    {
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = (-20);


        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref User32.RECT rect);
        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

      
        [DllImport("user32.dll")]
        public static extern int EnumWindows(EnumWindowsCallback lpEnumFunc, int lParam);
       public  delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);

        [DllImport("user32.dll")]
        public static extern ulong GetWindowLongA(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern void GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT 
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            
             
            public bool IsEmpty()
            {
                return this.Left == 0 &&
                       this.Top == 0 &&
                       this.Right == 0 &&
                       this.Bottom == 0;
            }

            public override int GetHashCode()
            {
                return 11*Left + 13*Top + 17*Right + 19*Bottom;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is RECT)) return false;
                RECT other = (RECT) obj;

                return this.Left == other.Left &&
                       this.Top == other.Top &&
                       this.Right == other.Right &&
                       this.Bottom == other.Bottom;
            }
        }


    }
}
