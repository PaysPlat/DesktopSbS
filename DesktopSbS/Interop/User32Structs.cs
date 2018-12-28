using System;
using System.Runtime.InteropServices;

namespace DesktopSbS.Interop
{

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public ShowWindowCommands showCmd;
        public POINT ptMinPosition;
        public POINT ptMaxPosition;
        public RECT rcNormalPosition;
    }

    public enum ShowWindowCommands : int
    {
        Hide = 0,
        Normal = 1,
        Minimized = 2,
        Maximized = 3,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CURSORINFO
    {
        public Int32 cbSize; // Specifies the size, in bytes, of the structure. 
        public Int32 flags; // Specifies the cursor state. This parameter can be one of the following values:
        public IntPtr hCursor; // Handle to the cursor. 
        public POINT ptScreenPos; // A POINT structure that receives the screen coordinates of the cursor. 
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static POINT operator -(POINT a, POINT b)
        {
            return new POINT(a.X - b.X, a.Y - b.Y);
        }

        public static POINT operator +(POINT a, POINT b)
        {
            return new POINT(a.X + b.X, a.Y + b.Y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public bool IsSize0()
        {
            return this.Left == 0 &&
                   this.Top == 0 &&
                   this.Right == 0 &&
                   this.Bottom == 0;
        }

        public bool IsMaximized()
        {
            return this.Left <= Options.ScreenSrcBounds.Left &&
                   this.Top <= Options.ScreenSrcBounds.Top &&
                   this.Right >= Options.ScreenSrcBounds.Right &&
                   this.Bottom >= Options.ScreenSrcBounds.Bottom;

        }

        public static RECT operator *(RECT r, double d)
        {
            return new RECT(
                (int)(r.Left * d),
                (int)(r.Top * d),
                (int)(r.Right * d),
                (int)(r.Bottom * d)
                );

        }

        public override int GetHashCode()
        {
            return 11 * Left + 13 * Top + 17 * Right + 19 * Bottom;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RECT)) return false;
            RECT other = (RECT)obj;

            return this.Left == other.Left &&
                   this.Top == other.Top &&
                   this.Right == other.Right &&
                   this.Bottom == other.Bottom;
        }

        public override string ToString()
        {
            return $"Left: {Left} Top: {Top} Right: {Right} Bottom:{Bottom}";
        }
    }

}
