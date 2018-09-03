using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using DesktopSbS.Interop;
using DesktopSbS.View;

namespace DesktopSbS
{
    public class WinSbS
    {
        private const bool
            DISPLAY_LEFT = true,
            DISPLAY_RIGHT = true;

        public string Title { get; set; }

        public IntPtr Handle { get; private set; }

        public View.ThumbWindow ThumbLeft { get; private set; }
        public View.ThumbWindow ThumbRight { get; private set; }

        public WinSbS Owner { get; set; }

        public int OffsetLevel { get; set; }

        public WS WinStyle { get; set; }
        public WSEX WinStyleEx { get; set; }

        public RECT SourceRect { get; set; }

        public WinSbS(IntPtr inHandle)
        {
            this.Handle = inHandle;
        }

        public void CopyThumbInstances(WinSbS inOriginal)
        {
            this.ThumbLeft = inOriginal.ThumbLeft;
            this.ThumbRight = inOriginal.ThumbRight;
        }

        public void RegisterThumbs()
        {
            User32.SetWindowPos(this.Handle, User32.NOT_TOPMOST, 0, 0, 0, 0, SWP.SWP_NOMOVE | SWP.SWP_NOSIZE);

            IntPtr thumbLeft = IntPtr.Zero,
                thumbRight = IntPtr.Zero;

            this.ThumbLeft = new View.ThumbWindow();
            this.ThumbRight = new View.ThumbWindow();

            this.ThumbLeft.Show();
            this.ThumbRight.Show();


            int tlRes = DISPLAY_LEFT ? DwmApi.DwmRegisterThumbnail(this.ThumbLeft.Handle, this.Handle, out thumbLeft) : 0;
            int trRes = DISPLAY_RIGHT ? DwmApi.DwmRegisterThumbnail(this.ThumbRight.Handle, this.Handle, out thumbRight) : 0;

            if (tlRes == 0 && trRes == 0)
            {
                this.ThumbLeft.Thumb = thumbLeft;
                this.ThumbRight.Thumb = thumbRight;
                this.UpdateThumbs();
            }



        }

        public void UnRegisterThumbs()
        {
            DwmApi.DwmUnregisterThumbnail(this.ThumbLeft.Thumb);
            DwmApi.DwmUnregisterThumbnail(this.ThumbRight.Thumb);

            this.ThumbLeft.Close();
            this.ThumbRight.Close();

        }



        public void UpdateThumbs(bool isTaskBar = false)
        {
            //int screenWidth = Options.ScreenWidth;
            int x, y, cx, cy;

            int screenBottom = Options.ScreenBounds.Bottom;

            int parallaxDecal = 2 * this.OffsetLevel * Options.ParallaxEffect;

            bool modeSbS = Options.ModeSbS;
            bool keepRatio = Options.KeepRatio;


            double dX = modeSbS || keepRatio ? 2 : 1;
            double dY = !modeSbS || keepRatio ? 2 : 1;
            int decalSbSX = modeSbS ? Options.ScreenBounds.Width / 2 : 0;
            int decalSbSY = modeSbS ? 0 : Options.ScreenBounds.Height / 2;
            int decalRatioX = keepRatio && !modeSbS ? Options.ScreenBounds.Width / 4 : 0;
            int decalRatioY = keepRatio && modeSbS ? Options.ScreenBounds.Height / 4 : 0;

            if (!isTaskBar && !this.SourceRect.IsMaximized() && this.SourceRect.Top < Options.ScreenWorkspace.Bottom)
            {
                screenBottom = Options.ScreenWorkspace.Bottom;
            }

            DwmApi.DWM_THUMBNAIL_PROPERTIES props = new DwmApi.DWM_THUMBNAIL_PROPERTIES();
            props.fVisible = true;
            props.dwFlags = DwmApi.DWM_TNP_VISIBLE | DwmApi.DWM_TNP_RECTDESTINATION | DwmApi.DWM_TNP_RECTSOURCE;

            // Left

            RECT parallaxSourceRectLeft = new RECT(
                this.SourceRect.Left + parallaxDecal,
                this.SourceRect.Top,
                this.SourceRect.Right + parallaxDecal,
                this.SourceRect.Bottom);

            props.rcSource = new RECT
            {
                Left = Math.Max(0, -parallaxSourceRectLeft.Left + Options.ScreenBounds.Left),
                Top = Math.Max(0, -parallaxSourceRectLeft.Top + Options.ScreenBounds.Top),
                Right = Math.Min(Options.ScreenBounds.Right, parallaxSourceRectLeft.Right) - parallaxSourceRectLeft.Left,
                Bottom = Math.Min(screenBottom, parallaxSourceRectLeft.Bottom) - parallaxSourceRectLeft.Top
            };

            props.rcDestination = new RECT
            {
                Left = 0,
                Top = 0,
                Right = (int)Math.Ceiling((props.rcSource.Right - props.rcSource.Left) / dX),
                Bottom = (int)Math.Ceiling((props.rcSource.Bottom - props.rcSource.Top) / dY)
            };

            x = Options.ScreenBounds.Left + decalRatioX + (int)Math.Floor(Math.Max(0, parallaxSourceRectLeft.Left - Options.ScreenBounds.Left) / dX);
            y = Options.ScreenBounds.Top + decalRatioY + (int)Math.Floor(Math.Max(0, parallaxSourceRectLeft.Top - Options.ScreenBounds.Top) / dY);
            cx = props.rcDestination.Right;
            cy = props.rcDestination.Bottom;

            User32.SetWindowPos(this.ThumbLeft.Handle, this.Owner?.ThumbLeft.Handle ?? IntPtr.Zero,
                x, y, cx, cy,
                SWP.SWP_ASYNCWINDOWPOS);

            DwmApi.DwmUpdateThumbnailProperties(this.ThumbLeft.Thumb, ref props);

            // Right

            RECT parallaxSourceRectRight = new RECT(
               this.SourceRect.Left - parallaxDecal,
               this.SourceRect.Top,
               this.SourceRect.Right - parallaxDecal,
               this.SourceRect.Bottom);

            props.rcSource = new RECT
            {
                Left = Math.Max(0, -parallaxSourceRectRight.Left + Options.ScreenBounds.Left),
                Top = Math.Max(0, -parallaxSourceRectRight.Top + Options.ScreenBounds.Top),
                Right = Math.Min(Options.ScreenBounds.Right, parallaxSourceRectRight.Right) - parallaxSourceRectRight.Left,
                Bottom = Math.Min(screenBottom, parallaxSourceRectRight.Bottom) - parallaxSourceRectRight.Top
            };

            props.rcDestination = new RECT
            {
                Left = 0,
                Top = 0,
                Right = (int)Math.Ceiling((props.rcSource.Right - props.rcSource.Left) / dX),
                Bottom = (int)Math.Ceiling((props.rcSource.Bottom - props.rcSource.Top) / dY)
            };

            x = Options.ScreenBounds.Left + decalRatioX + decalSbSX + (int)Math.Floor(Math.Max(0, parallaxSourceRectRight.Left - Options.ScreenBounds.Left) / dX);
            y = Options.ScreenBounds.Top + decalRatioY + decalSbSY + (int)Math.Floor(Math.Max(0, parallaxSourceRectRight.Top - Options.ScreenBounds.Top) / dY);
            cx = props.rcDestination.Right;
            cy = props.rcDestination.Bottom;

            User32.SetWindowPos(this.ThumbRight.Handle, this.Owner?.ThumbRight.Handle ?? IntPtr.Zero,
                x, y, cx, cy,
                SWP.SWP_ASYNCWINDOWPOS);

            DwmApi.DwmUpdateThumbnailProperties(this.ThumbRight.Thumb, ref props);

            if (isTaskBar)
            {
                this.ThumbLeft.Background = Brushes.Black;
                this.ThumbRight.Background = Brushes.Black;
            }


#if DEBUG
            //if (this.Title.Contains("About"))
            //{
            //    DebugWindow.Instance.UpdateMessage($"Source Win {this.SourceRect}{Environment.NewLine}Src Thumb {props.rcSource}{Environment.NewLine}Dst Thumb {props.rcDestination}{Environment.NewLine}Dst Pos Left: {Math.Max(0, this.SourceRect.Left) / 2} Top: {Math.Max(0, this.SourceRect.Top)}");
            //}

#endif

        }



        public override string ToString()
        {
            return this.Title;
        }
    }

}

