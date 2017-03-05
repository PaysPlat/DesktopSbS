using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace DeskTopSBS
{
    public class WinSBS
    {
        public string Title { get; set; }

        public IntPtr Handle { get; private set; }

        public ThumbWindow ThumbLeft { get; private set; }
        public ThumbWindow ThumbRight { get; private set; }

        public WinSBS Owner { get; set; }


        //public DwmApi.WS WinStyle { get; set; }
        //public DwmApi.WSEX WinStyleEx { get; set; }

        public User32.RECT SourceRect { get; set; }

        public WinSBS(IntPtr inHandle)
        {
            this.Handle = inHandle;
        }

        public void CopyThumbInstances(WinSBS inOriginal)
        {
            this.ThumbLeft = inOriginal.ThumbLeft;
            this.ThumbRight = inOriginal.ThumbRight;
        }

        public void RegisterThumbs()
        {

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
            int screenWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            int screenHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;


            DwmApi.DWM_THUMBNAIL_PROPERTIES props = new DwmApi.DWM_THUMBNAIL_PROPERTIES();
            props.fVisible = true;
            props.dwFlags = DwmApi.DWM_TNP_VISIBLE | DwmApi.DWM_TNP_RECTDESTINATION | DwmApi.DWM_TNP_RECTSOURCE;

            int decalLeft = Math.Max(0, -this.SourceRect.Left),
                decalTop = Math.Max(0, -this.SourceRect.Top);

            props.rcSource = new User32.RECT
            {
                Left = decalLeft,
                Top = decalTop,
                Right = Math.Min(screenWidth, this.SourceRect.Right) - this.SourceRect.Left,
                Bottom = Math.Min(screenHeight, this.SourceRect.Bottom) - this.SourceRect.Top
            };

            props.rcDestination = new User32.RECT
            {
                Left = 0,
                Top = 0,
                Right = (int)Math.Ceiling((props.rcSource.Right - props.rcSource.Left) / 2.0),
                Bottom = props.rcSource.Bottom - props.rcSource.Top
            };

            int posLeft = (int)Math.Floor(Math.Max(0, this.SourceRect.Left) / 2.0),
                posTop = Math.Max(0, this.SourceRect.Top);

            User32.SetWindowPos(this.ThumbLeft.Handle, this.Owner?.ThumbLeft.Handle ?? IntPtr.Zero,
                posLeft,
                posTop,
                props.rcDestination.Right,
                props.rcDestination.Bottom,
                0);

            DwmApi.DwmUpdateThumbnailProperties(this.ThumbLeft.Thumb, ref props);


            User32.SetWindowPos(this.ThumbRight.Handle, this.Owner?.ThumbRight.Handle ?? IntPtr.Zero,
              screenWidth / 2 + posLeft,
              posTop,
              props.rcDestination.Right,
              props.rcDestination.Bottom,
              0);

            DwmApi.DwmUpdateThumbnailProperties(this.ThumbRight.Thumb, ref props);

#if DEBUG
            //if (this.Title.Contains("Chrome"))
            //{
            //   DebugWindow.Instance.UpdateMessage( $"Source Win {this.SourceRect}{Environment.NewLine}Src Thumb {props.rcSource}{Environment.NewLine}Dst Thumb {props.rcDestination}{Environment.NewLine} Dst Pos Left: {Math.Max(0, this.SourceRect.Left) / 2} Top: {Math.Max(0, this.SourceRect.Top)}");
            //}

#endif

        }



        public override string ToString()
        {
            return this.Title;
        }
    }

}

