using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DesktopSbS.Interop;
using DesktopSbS.View;

namespace DesktopSbS
{
    public class CursorSbS
    {
        private static readonly POINT arrowStdSize = new POINT(12, 19);

        public View.CursorWindow ThumbLeft { get; private set; }
        public View.CursorWindow ThumbRight { get; private set; }

        public POINT Position { get; set; }


        public WinSbS Owner { get; set; }

        public int OffsetLevel { get; private set; }

        public void RegisterThumbs()
        {


            if (this.ThumbLeft == null) this.ThumbLeft = new View.CursorWindow();
            if (this.ThumbRight == null) this.ThumbRight = new View.CursorWindow();

            this.ThumbLeft.Show();
            this.ThumbRight.Show();

            this.UpdateThumbs(1);


        }

        public void UnRegisterThumbs()
        {

            this.ThumbLeft.Hide();
            this.ThumbRight.Hide();

        }

        private bool is3DActive;
        public bool Is3DActive
        {
            get
            {
                return this.is3DActive;
            }
            set
            {
                this.is3DActive = value && !Options.IgnoreCursor;
                this.UpdateCursorState();                
            }
        }

        public bool IsCursorOnScreen { get; set; }


        private bool isCursorActive = false;
        public void UpdateCursorState()
        {
            bool newCursorActive = this.Is3DActive && this.IsCursorOnScreen;

            if (this.isCursorActive != newCursorActive)
            {
                this.isCursorActive = newCursorActive;
                if (this.isCursorActive)
                {
#if !DEBUG
                    CursorWindow.HideCursors();
#endif
                    this.RegisterThumbs();
                }
                else
                {
                    this.UnRegisterThumbs();
                    CursorWindow.ShowCursors();
                }
            }


        }


        public void UpdateThumbs(int offsetLevel)
        {
            CURSORINFO cursorInfo = new CURSORINFO();
            cursorInfo.cbSize = Marshal.SizeOf(cursorInfo);
            User32.GetCursorInfo(out cursorInfo);
            this.IsCursorOnScreen = Options.ScreenBounds.Contains(cursorInfo.ptScreenPos.X, cursorInfo.ptScreenPos.Y);
            this.UpdateCursorState();

            // DebugWindow.Instance.UpdateMessage($"Is3DActive: {this.Is3DActive}{Environment.NewLine}IsCursorOnScreen: {this.IsCursorOnScreen}{ Environment.NewLine}ScreenBounds: { Options.ScreenBounds}{Environment.NewLine}Mouse Position:{cursorInfo.ptScreenPos}");

            if (this.ThumbLeft == null || this.ThumbRight == null) return;

            this.OffsetLevel = offsetLevel;

            bool modeSbS = Options.ModeSbS;
            int screenWidth = Options.ScreenBounds.Width;
            int screenHeight = Options.ScreenBounds.Height;
            double scale = Options.ScreenScale;


            POINT screenTopLeft = new POINT(Options.ScreenBounds.Left, Options.ScreenBounds.Top);

            double dX = modeSbS ? 2 : 1;
            double dY = modeSbS ? 1 : 2;
            int decalX = modeSbS ? screenWidth / 2 : 0;
            int decalY = modeSbS ? 0 : screenHeight / 2;

            int parallaxDecal = 2 * Options.ParallaxEffect * offsetLevel;

            this.Position = cursorInfo.ptScreenPos - screenTopLeft;

            POINT offset = this.ThumbLeft.SetCursor(cursorInfo.hCursor);
            this.ThumbRight.SetCursor(cursorInfo.hCursor);

            SWP leftVisible = (modeSbS && this.Position.X + parallaxDecal + arrowStdSize.X < screenWidth) ||
                              (!modeSbS && this.Position.Y + arrowStdSize.Y < screenHeight)
                              ? SWP.SWP_SHOWWINDOW : SWP.SWP_HIDEWINDOW;
            SWP rightVisible = !modeSbS || this.Position.X - parallaxDecal > 0 ? SWP.SWP_SHOWWINDOW : SWP.SWP_HIDEWINDOW;

            User32.SetWindowPos(this.ThumbLeft.Handle, this.Owner?.ThumbLeft.Handle ?? IntPtr.Zero,
                screenTopLeft.X + (int)((this.Position.X + parallaxDecal - offset.X * scale) / dX),
                screenTopLeft.Y + (int)((this.Position.Y - offset.Y * scale) / dY),
                (int)(32 * scale / dX),
                 (int)(32 * scale / dY),
                SWP.SWP_ASYNCWINDOWPOS | leftVisible);

            User32.SetWindowPos(this.ThumbRight.Handle, this.Owner?.ThumbRight.Handle ?? IntPtr.Zero,
               screenTopLeft.X + decalX + (int)((this.Position.X - parallaxDecal - offset.X * scale) / dX),
               screenTopLeft.Y + decalY + (int)((this.Position.Y - offset.Y * scale) / dY),
                 (int)(32 * scale / dX),
                 (int)(32 * scale / dY),
                SWP.SWP_ASYNCWINDOWPOS | rightVisible);

            //DebugWindow.Instance.UpdateMessage($"Mouse Left: {this.Position.X} Top: {this.Position.Y}");

        }


    }
}
