using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DesktopSbS.Hook
{
    public class GlobalMouseHook
{
        public class MouseButtonPosEventArgs : MouseButtonEventArgs
        {
            private Point pos;
            public Point Pos
            {
                get { return this.pos; }
            }
            public MouseButtonPosEventArgs(MouseDevice pMouseDevice, int pTimestamp, MouseButton pMouseButton, Point pPos)
                : base(pMouseDevice, pTimestamp, pMouseButton)
            {
                this.pos = pPos;
            }

        }


    private delegate int LowLevelMouseProc(int nCode, IntPtr wParam, ref MSLLHOOKSTRUCT lParam);

    public delegate void MouseButtonPosEventHandler(object sender, MouseButtonPosEventArgs e);

    public event MouseButtonPosEventHandler MouseDown;

    public event MouseButtonPosEventHandler MouseUp;

    /// <summary>
    /// Handle to the hook, need this to unhook and call the next hook
    /// </summary>
    private IntPtr hhook = IntPtr.Zero;

    private LowLevelMouseProc mhp;

    public GlobalMouseHook()
    {
        mhp = new LowLevelMouseProc(HookCallback);
        hook();


    }

    ~GlobalMouseHook()
    {
        unhook();
    }
 

    private void hook()
    {
        IntPtr hInstance = LoadLibrary("User32");
        hhook = SetWindowsHookEx(WH_MOUSE_LL, mhp, hInstance, 0);
    }

    /// <summary>
    /// Uninstalls the global hook
    /// </summary>
    public void unhook()
    {
        if (hhook != IntPtr.Zero)
        {
            UnhookWindowsHookEx(hhook);
        }
    }

   private int HookCallback(
      int nCode, IntPtr wParam, ref MSLLHOOKSTRUCT lParam)
    {
        if (nCode >= 0)
        {
            MouseButtonPosEventArgs mbpea = null;
            if (MouseDown != null)
            {
                if (MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
                {
                    mbpea = new MouseButtonPosEventArgs(Mouse.PrimaryDevice, (int)lParam.time, MouseButton.Left,lParam.pt);

                }
                else if (MouseMessages.WM_RBUTTONDOWN == (MouseMessages)wParam)
                {
                    mbpea = new MouseButtonPosEventArgs(Mouse.PrimaryDevice, (int)lParam.time, MouseButton.Right, lParam.pt);
                }
                if (mbpea != null)
                {
                    MouseDown(this, mbpea);
                    if (mbpea.Handled) return 1;
                }
            }
            if (MouseUp != null)
            {
                if (MouseMessages.WM_LBUTTONUP == (MouseMessages)wParam)
                {
                    mbpea = new MouseButtonPosEventArgs(Mouse.PrimaryDevice, (int)lParam.time, MouseButton.Left, lParam.pt);
                }
                else if (MouseMessages.WM_RBUTTONUP == (MouseMessages)wParam)
                {
                    mbpea = new MouseButtonPosEventArgs(Mouse.PrimaryDevice, (int)lParam.time, MouseButton.Right, lParam.pt);
                }
                if (mbpea != null)
                {
                    
                    mbpea.GetPosition(null);
                    MouseUp(this, mbpea);
                    if (mbpea.Handled) return 1;
                }
            }


        }
        return CallNextHookEx(hhook, nCode, wParam, ref lParam);
    }

    private const int WH_MOUSE_LL = 14;

    private enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;

        public static implicit operator Point (POINT pt)
        {
            return new Point(pt.x, pt.y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
      LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int CallNextHookEx(IntPtr hhk, int nCode,
      IntPtr wParam, ref MSLLHOOKSTRUCT lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the library</param>
        /// <returns>A handle to the library</returns>
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

}

}
