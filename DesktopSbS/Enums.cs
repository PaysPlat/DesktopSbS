using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSbS
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

    [Flags]
    public enum SWP :int
    {
        SWP_ASYNCWINDOWPOS =
        0x4000,

        SWP_DEFERERASE =
        0x2000,

        SWP_DRAWFRAME =
        0x0020,

        SWP_FRAMECHANGED =
        0x0020,

        SWP_HIDEWINDOW =
        0x0080,

        SWP_NOACTIVATE =
        0x0010,

        SWP_NOCOPYBITS =
        0x0100,

        SWP_NOMOVE =
        0x0002,

        SWP_NOOWNERZORDER =
        0x0200,

        SWP_NOREDRAW =
        0x0008,

        SWP_NOREPOSITION =
        0x0200,

        SWP_NOSENDCHANGING =
        0x0400,

        SWP_NOSIZE =
        0x0001,

        SWP_NOZORDER =
        0x0004,

        SWP_SHOWWINDOW =
        0x0040

    }


    public enum DpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }

    public enum DeviceCap
    {
        /// <summary>
        /// Logical pixels inch in X
        /// </summary>
        LOGPIXELSX = 88,
        /// <summary>
        /// Logical pixels inch in Y
        /// </summary>
        LOGPIXELSY = 90

        // Other constants may be founded on pinvoke.net
    }

    public enum CURSOR_TYPE
    {
        IDC_APPSTARTING =
        32650,

        IDC_ARROW =
        32512,

        IDC_CROSS =
        32515,

        IDC_HAND =
        32649,

        IDC_HELP =
        32651,

        IDC_IBEAM =
        32513,

        IDC_ICON =
        32641,

        IDC_NO =
        32648,

        IDC_SIZE =
        32640,

        IDC_SIZEALL =
        32646,

        IDC_SIZENESW =
        32643,

        IDC_SIZENS =
        32645,

        IDC_SIZENWSE =
        32642,

        IDC_SIZEWE =
        32644,

        IDC_UPARROW =
        32516,

        IDC_WAIT =
        32514,


    }
}
