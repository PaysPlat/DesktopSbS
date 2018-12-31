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
using DesktopSbS.Model;

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
                this.is3DActive = value && !Options.HideDestCursor;
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
                    if (Options.HideSrcCursor)
                    {
                        CursorWindow.HideCursors();
                    }
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
            this.IsCursorOnScreen = Options.AreaSrcBounds.Contains(cursorInfo.ptScreenPos.X, cursorInfo.ptScreenPos.Y);
            this.UpdateCursorState();

            // DebugWindow.Instance.UpdateMessage($"Is3DActive: {this.Is3DActive}{Environment.NewLine}IsCursorOnScreen: {this.IsCursorOnScreen}{ Environment.NewLine}ScreenBounds: { Options.ScreenBounds}{Environment.NewLine}Mouse Position:{cursorInfo.ptScreenPos}");

            if (this.ThumbLeft == null || this.ThumbRight == null) return;

            SbSComputedVariables scv = Options.ComputedVariables;

            this.OffsetLevel = offsetLevel;

            int screenWidth = Options.ScreenDestBounds.Width;
            int screenHeight = Options.ScreenDestBounds.Height;
            double scale = Options.ScreenScale;

            POINT screenSrcTopLeft = new POINT(Options.AreaSrcBounds.Left, Options.AreaSrcBounds.Top);
            POINT screenDestTopLeft = new POINT(Options.ScreenDestBounds.Left, Options.ScreenDestBounds.Top);

            int parallaxDecal = 2 * Options.ParallaxEffect * offsetLevel;

            this.Position = cursorInfo.ptScreenPos - screenSrcTopLeft;

            POINT offset = this.ThumbLeft.SetCursor(cursorInfo.hCursor);
            this.ThumbRight.SetCursor(cursorInfo.hCursor);

            SWP leftVisible = (this.Position.X + parallaxDecal + arrowStdSize.X < screenWidth) &&
                              (Options.ModeSbS || this.Position.Y + arrowStdSize.Y < screenHeight)
                              ? SWP.SWP_SHOWWINDOW : SWP.SWP_HIDEWINDOW;
            SWP rightVisible = (this.Position.X - parallaxDecal > 0) ? SWP.SWP_SHOWWINDOW : SWP.SWP_HIDEWINDOW;

            User32.SetWindowPos(this.ThumbLeft.Handle, this.Owner?.ThumbLeft.Handle ?? IntPtr.Zero,
                SWP.SWP_ASYNCWINDOWPOS | leftVisible);
                scv.DestPositionX + (int)((this.Position.X + parallaxDecal - offset.X * scale) / scv.RatioX),
                scv.DestPositionY + (int)((this.Position.Y - offset.Y * scale) / scv.RatioY),
                (int)(32 * scale / scv.RatioX),
                (int)(32 * scale / scv.RatioY),

            User32.SetWindowPos(this.ThumbRight.Handle, this.Owner?.ThumbRight.Handle ?? IntPtr.Zero,
                SWP.SWP_ASYNCWINDOWPOS | rightVisible);
                scv.DestPositionX + scv.DecalSbSX + (int)((this.Position.X - parallaxDecal - offset.X * scale) / scv.RatioX),
                scv.DestPositionY + scv.DecalSbSY + (int)((this.Position.Y - offset.Y * scale) / scv.RatioY),
                (int)(32 * scale / scv.RatioX),
                (int)(32 * scale / scv.RatioY),

            //DebugWindow.Instance.UpdateMessage($"Mouse Left: {this.Position.X} Top: {this.Position.Y}");

        }


    }
}
