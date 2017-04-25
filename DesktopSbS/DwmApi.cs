using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopSbS
{
    public static class DwmApi
    {
        private const string dll = "dwmapi.dll";


        #region Constants


        public static readonly int DWM_TNP_SOURCECLIENTAREAONLY = 0x10;
        public static readonly int DWM_TNP_VISIBLE = 0x8;
        public static readonly int DWM_TNP_OPACITY = 0x4;
        public static readonly int DWM_TNP_RECTSOURCE = 0x2;
        public static readonly int DWM_TNP_RECTDESTINATION = 0x1;


        #endregion

        #region DWM functions

        [DllImport(dll)]
        public static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

        [DllImport(dll)]
        public static extern int DwmUnregisterThumbnail(IntPtr thumb);

        [DllImport(dll)]
        public static extern int DwmQueryThumbnailSourceSize(IntPtr thumb, out PSIZE size);

        [DllImport(dll)]
        public static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props);

        #endregion

        #region Interop structs

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_THUMBNAIL_PROPERTIES
        {
            public int dwFlags;
            public RECT rcDestination;
            public RECT rcSource;
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
