using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace DesktopSbS
{
    public class WinSbS
    {
        public string Title { get; set; }

        public IntPtr Handle { get; private set; }

        public ThumbWindow ThumbLeft { get; private set; }
        public ThumbWindow ThumbRight { get; private set; }

        public WinSbS Owner { get; set; }

        public int OffsetLevel { get; set; }

        //public DwmApi.WS WinStyle { get; set; }
        //public DwmApi.WSEX WinStyleEx { get; set; }

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

            this.ThumbLeft = new ThumbWindow();
            this.ThumbRight = new ThumbWindow();

            this.ThumbLeft.Show();
            this.ThumbRight.Show();


            int tlRes = DwmApi.DwmRegisterThumbnail(this.ThumbLeft.Handle, this.Handle, out thumbLeft);
            int trRes = DwmApi.DwmRegisterThumbnail(this.ThumbRight.Handle, this.Handle, out thumbRight);

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



        public void UpdateThumbs()
        {
            int screenWidth = App.Current.ScreenWidth;

            int screenHeight = App.Current.ScreenHeight;

            int parallaxDecal = 2 * this.OffsetLevel * App.Current.ParallaxEffect;

            bool modeSbS = App.Current.ModeSbS;

            double dX = modeSbS ? 2 : 1;
            double dY = modeSbS ? 1 : 2;
            int decalX = modeSbS ? screenWidth / 2 : 0;
            int decalY = modeSbS ? 0 : screenHeight / 2;

            if (this.SourceRect.Top < screenHeight - App.Current.TaskBarHeight)
            {
                screenHeight -= App.Current.TaskBarHeight;
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
                Left = Math.Max(0, -parallaxSourceRectLeft.Left),
                Top = Math.Max(0, -parallaxSourceRectLeft.Top),
                Right = Math.Min(screenWidth, parallaxSourceRectLeft.Right) - parallaxSourceRectLeft.Left,
                Bottom = Math.Min(screenHeight, parallaxSourceRectLeft.Bottom) - parallaxSourceRectLeft.Top
            };

            props.rcDestination = new RECT
            {
                Left = 0,
                Top = 0,
                Right = (int)Math.Ceiling((props.rcSource.Right - props.rcSource.Left) / dX),
                Bottom = (int)Math.Ceiling((props.rcSource.Bottom - props.rcSource.Top) / dY)
            };

            User32.SetWindowPos(this.ThumbLeft.Handle, this.Owner?.ThumbLeft.Handle ?? IntPtr.Zero,
                       (int)Math.Floor(Math.Max(0, parallaxSourceRectLeft.Left) / dX),
                       (int)Math.Floor(Math.Max(0, parallaxSourceRectLeft.Top) / dY),
                       props.rcDestination.Right,
                       props.rcDestination.Bottom,
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
                Left = Math.Max(0, -parallaxSourceRectRight.Left),
                Top = Math.Max(0, -parallaxSourceRectRight.Top),
                Right = Math.Min(screenWidth, parallaxSourceRectRight.Right) - parallaxSourceRectRight.Left,
                Bottom = Math.Min(screenHeight, parallaxSourceRectRight.Bottom) - parallaxSourceRectRight.Top
            };

            props.rcDestination = new RECT
            {
                Left = 0,
                Top = 0,
                Right = (int)Math.Ceiling((props.rcSource.Right - props.rcSource.Left) / dX),
                Bottom = (int)Math.Ceiling((props.rcSource.Bottom - props.rcSource.Top) / dY)
            };

            User32.SetWindowPos(this.ThumbRight.Handle, this.Owner?.ThumbRight.Handle ?? IntPtr.Zero,
                       decalX + (int)Math.Floor(Math.Max(0, parallaxSourceRectRight.Left) / dX),
                       decalY + (int)Math.Floor(Math.Max(0, parallaxSourceRectRight.Top) / dY),
                       props.rcDestination.Right,
                       props.rcDestination.Bottom,
                       SWP.SWP_ASYNCWINDOWPOS);

            DwmApi.DwmUpdateThumbnailProperties(this.ThumbRight.Thumb, ref props);

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

