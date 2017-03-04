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

            DwmApi.DWM_THUMBNAIL_PROPERTIES props = new DwmApi.DWM_THUMBNAIL_PROPERTIES();
            props.fVisible = true;
            props.dwFlags = DwmApi.DWM_TNP_VISIBLE | DwmApi.DWM_TNP_RECTDESTINATION;

            props.rcDestination = new User32.RECT
            {
                Left = 0,
                Top = 0,
                Right = (this.SourceRect.Right - this.SourceRect.Left) / 2,
                Bottom = this.SourceRect.Bottom - SourceRect.Top
            };

            User32.SetWindowPos(this.ThumbLeft.Handle, this.Owner?.ThumbLeft.Handle??IntPtr.Zero,
                1920 + this.SourceRect.Left/2,
                this.SourceRect.Top,
                props.rcDestination.Right,
                props.rcDestination.Bottom,
                0);

              DwmApi.DwmUpdateThumbnailProperties(this.ThumbLeft.Thumb, ref props);

            int screenWidthMiddle = (int)System.Windows.SystemParameters.PrimaryScreenWidth / 2;

            User32.SetWindowPos(this.ThumbRight.Handle, this.Owner?.ThumbRight.Handle ?? IntPtr.Zero,
              screenWidthMiddle + this.SourceRect.Left / 2,
              this.SourceRect.Top,
              props.rcDestination.Right,
              props.rcDestination.Bottom,
              0);

            DwmApi.DwmUpdateThumbnailProperties(this.ThumbRight.Thumb, ref props);
        }



        public override string ToString()
        {
            return this.Title;
        }
    }

}

