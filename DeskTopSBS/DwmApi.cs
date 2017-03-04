using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DeskTopSBS
{
    public static class DwmApi
    {
        [Flags]
        public enum WS : ulong
        {
            WS_BORDER = 0x00800000L,
            WS_CAPTION = 0x00C00000L,
            WS_CHILD = 0x40000000L,
            WS_CHILDWINDOW = 0x40000000L,
            WS_CLIPCHILDREN = 0x02000000L,
            WS_CLIPSIBLINGS = 0x04000000L,
            WS_DISABLED = 0x08000000L,
            WS_DLGFRAME = 0x00400000L,

            WS_GROUP =
            0x00020000L,

            WS_HSCROLL =
            0x00100000L,

            WS_ICONIC =
            0x20000000L,

            WS_MAXIMIZE =
            0x01000000L,

            WS_MAXIMIZEBOX =
            0x00010000L,

            WS_MINIMIZE =
            0x20000000L,

            WS_MINIMIZEBOX =
            0x00020000L,

            WS_OVERLAPPED =
            0x00000000L,


            WS_POPUP =
            0x80000000L,


            WS_SIZEBOX =
            0x00040000L,

            WS_SYSMENU =
            0x00080000L,

            WS_TABSTOP =
            0x00010000L,

            WS_THICKFRAME =
            0x00040000L,

            WS_TILED =
            0x00000000L,


            WS_VISIBLE =
            0x10000000L,

            WS_VSCROLL =
            0x00200000L,

        }

        [Flags]
        public enum WSEX : ulong
        {
            WS_EX_ACCEPTFILES =
            0x00000010L,

            WS_EX_APPWINDOW =
            0x00040000L,

            WS_EX_CLIENTEDGE =
            0x00000200L,

            WS_EX_COMPOSITED =
            0x02000000L,

            WS_EX_CONTEXTHELP =
            0x00000400L,

            WS_EX_CONTROLPARENT =
            0x00010000L,

            WS_EX_DLGMODALFRAME =
            0x00000001L,

            WS_EX_LAYERED =
            0x00080000,

            WS_EX_LAYOUTRTL =
            0x00400000L,

            WS_EX_LEFT =
            0x00000000L,

            WS_EX_LEFTSCROLLBAR =
            0x00004000L,

            WS_EX_LTRREADING =
            0x00000000L,

            WS_EX_MDICHILD =
            0x00000040L,

            WS_EX_NOACTIVATE =
            0x08000000L,

            WS_EX_NOINHERITLAYOUT =
            0x00100000L,

            WS_EX_NOPARENTNOTIFY =
            0x00000004L,

            WS_EX_NOREDIRECTIONBITMAP =
            0x00200000L,
            
            WS_EX_RIGHT =
            0x00001000L,

            WS_EX_RIGHTSCROLLBAR =
            0x00000000L,

            WS_EX_RTLREADING =
            0x00002000L,

            WS_EX_STATICEDGE =
            0x00020000L,

            WS_EX_TOOLWINDOW =
            0x00000080L,

            WS_EX_TOPMOST =
            0x00000008L,

            WS_EX_TRANSPARENT =
            0x00000020L,

            WS_EX_WINDOWEDGE =
            0x00000100L,
        }

        #region Constants

        public static readonly int GWL_STYLE = -16;

        public static readonly int DWM_TNP_SOURCECLIENTAREAONLY = 0x10;
        public static readonly int DWM_TNP_VISIBLE = 0x8;
        public static readonly int DWM_TNP_OPACITY = 0x4;
        public static readonly int DWM_TNP_RECTSOURCE = 0x2;
        public static readonly int DWM_TNP_RECTDESTINATION = 0x1;

        public static readonly WS TARGETWINDOW = WS.WS_VISIBLE;

        #endregion

        #region DWM functions

        [DllImport("dwmapi.dll")]
        public static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUnregisterThumbnail(IntPtr thumb);

        [DllImport("dwmapi.dll")]
        public static extern int DwmQueryThumbnailSourceSize(IntPtr thumb, out PSIZE size);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props);

        #endregion

        #region Interop structs

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_THUMBNAIL_PROPERTIES
        {
            public int dwFlags;
            public User32.RECT rcDestination;
            public User32.RECT rcSource;
            public byte opacity;
            public bool fVisible;
            public bool fSourceClientAreaOnly;
        }



        [StructLayout(LayoutKind.Sequential)]
        public struct PSIZE
        {
            public int x;
            public int y;
        }

        #endregion





    }
}
