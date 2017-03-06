using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DesktopSbS
{
    public class CursorSbS
    {
        public CursorWindow ThumbLeft { get; private set; }
        public CursorWindow ThumbRight { get; private set; }

        public POINT Position { get; set; }


        public WinSbS Owner { get; set; }

        public int OffsetLevel { get; private set; }

        public void RegisterThumbs()
        {


            this.ThumbLeft = new CursorWindow();
            this.ThumbRight = new CursorWindow();

            this.ThumbLeft.Show();
            this.ThumbRight.Show();

            this.UpdateThumbs(1);


        }

        public void UnRegisterThumbs()
        {

            this.ThumbLeft.Close();
            this.ThumbRight.Close();

        }



        public void UpdateThumbs(int offsetLevel)
        {
            this.OffsetLevel = offsetLevel;
     
            int parallaxDecal = App.Current.ParallaxEffect * offsetLevel;

            CURSORINFO cursorInfo = new CURSORINFO();
            cursorInfo.cbSize = Marshal.SizeOf(cursorInfo);
            User32.GetCursorInfo(out cursorInfo);

                this.Position = cursorInfo.ptScreenPos;

            POINT offset = this.ThumbLeft.SetCursor(cursorInfo.hCursor);
            this.ThumbRight.SetCursor(cursorInfo.hCursor);

       
            DebugWindow.Instance.UpdateMessage($"Mouse Left: {this.Position.X} Top: {this.Position.Y}");

            User32.SetWindowPos(this.ThumbLeft.Handle, this.Owner?.ThumbLeft.Handle ?? IntPtr.Zero,
                this.Position.X / 2 - offset.X + parallaxDecal,
                this.Position.Y - offset.Y,
                16,
                32,
                User32.SWP_ASYNCWINDOWPOS| User32.SWP_FRAMECHANGED |User32.SWP_NOSIZE);

           
            int screenWidthMiddle = App.Current.ScreenWidth/ 2;

            User32.SetWindowPos(this.ThumbRight.Handle, this.Owner?.ThumbRight.Handle ?? IntPtr.Zero,
                screenWidthMiddle + this.Position.X / 2 - offset.X - parallaxDecal,
                this.Position.Y - offset.Y,
                16,
                32,
                User32.SWP_ASYNCWINDOWPOS|User32.SWP_FRAMECHANGED | User32.SWP_NOSIZE);
        }


    }
}
