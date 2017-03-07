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
        private static readonly POINT arrowStdSize = new POINT(12, 19);

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

            bool modeSbS = App.Current.ModeSbS;

            int screenWidth = App.Current.ScreenWidth;
            int screenHeight = App.Current.ScreenHeight;

            double dX = modeSbS ? 2 : 1;
            double dY = modeSbS ? 1 : 2;
            int decalX = modeSbS ? screenWidth / 2 : 0;
            int decalY = modeSbS ? 0 : App.Current.ScreenHeight / 2;

            int parallaxDecal = 2*App.Current.ParallaxEffect * offsetLevel;

            CURSORINFO cursorInfo = new CURSORINFO();
            cursorInfo.cbSize = Marshal.SizeOf(cursorInfo);
            User32.GetCursorInfo(out cursorInfo);

            this.Position = cursorInfo.ptScreenPos;

            POINT offset = this.ThumbLeft.SetCursor(cursorInfo.hCursor);
            this.ThumbRight.SetCursor(cursorInfo.hCursor);

            SWP leftVisible = (modeSbS && this.Position.X + parallaxDecal + arrowStdSize.X < screenWidth) ||
                              (!modeSbS && this.Position.Y+arrowStdSize.Y<screenHeight)
                              ? SWP.SWP_SHOWWINDOW : SWP.SWP_HIDEWINDOW;
            SWP rightVisible = !modeSbS || this.Position.X - parallaxDecal > 0 ? SWP.SWP_SHOWWINDOW : SWP.SWP_HIDEWINDOW;

            User32.SetWindowPos(this.ThumbLeft.Handle, this.Owner?.ThumbLeft.Handle ?? IntPtr.Zero,
                (int)(( this.Position.X + parallaxDecal - offset.X )/ dX  ),
                (int)((this.Position.Y - offset.Y )/ dY ),
                (int)(32 / dX),
                 (int)(32 / dY),
                SWP.SWP_ASYNCWINDOWPOS | leftVisible);

            User32.SetWindowPos(this.ThumbRight.Handle, this.Owner?.ThumbRight.Handle ?? IntPtr.Zero,
               decalX + (int)(( this.Position.X - parallaxDecal - offset.X) / dX ),
               decalY + (int)((this.Position.Y - offset.Y) / dY),
                 (int)(32 / dX),
                 (int)(32 / dY),
                SWP.SWP_ASYNCWINDOWPOS|rightVisible);

            //DebugWindow.Instance.UpdateMessage($"Mouse Left: {this.Position.X} Top: {this.Position.Y}");

        }


    }
}
