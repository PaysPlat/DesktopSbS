using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskTopSBS
{
    public class WinSBS
    {
        private string title = null;
        public string Title
        {
            get
            {
                if (this.title == null)
                {
                    StringBuilder sb = new StringBuilder(100);
                    User32.GetWindowText(this.Handle, sb, sb.Capacity);
                    this.title = sb.ToString();
                }
                return this.title;
            }
        }
        public IntPtr Handle { get; private set; }

        public IntPtr ThumbLeft { get; private set; }
        public IntPtr ThumbRight { get; private set; }

        public DwmApi.WS WinStyle { get; set; }
        public DwmApi.WSEX WinStyleEx { get; set; }

        public User32.RECT SourceRect { get; set; }


        public WinSBS(IntPtr inHandle)
        {
            this.Handle = inHandle;
        }

        public void RegisterThumbs(IntPtr destHandle)
        {

            IntPtr thumbLeft = IntPtr.Zero,
                thumbRight = IntPtr.Zero;


            int tlRes = DwmApi.DwmRegisterThumbnail(destHandle, this.Handle, out thumbLeft);
            int trRes = DwmApi.DwmRegisterThumbnail(destHandle, this.Handle, out thumbRight);

            if (tlRes == 0 && trRes == 0)
            {
                this.ThumbLeft = thumbLeft;
                this.ThumbRight = thumbRight;

            }

        }

        public void UnRegisterThumbs()
        {
            DwmApi.DwmUnregisterThumbnail(this.ThumbLeft);
            DwmApi.DwmUnregisterThumbnail(this.ThumbRight);
        }



        public void UpdateThumbs()
        {


            DwmApi.DWM_THUMBNAIL_PROPERTIES props = new DwmApi.DWM_THUMBNAIL_PROPERTIES();
            props.fVisible = true;
            props.dwFlags = DwmApi.DWM_TNP_VISIBLE | DwmApi.DWM_TNP_RECTDESTINATION;

            User32.RECT sourceRect = this.SourceRect;

            props.rcDestination = new User32.RECT
            {
                Left = sourceRect.Left / 2,
                Top = sourceRect.Top,
                Right = sourceRect.Right / 2,
                Bottom = sourceRect.Bottom
            };

            DwmApi.DwmUpdateThumbnailProperties(this.ThumbLeft, ref props);

            int screenWidthMiddle = (int)System.Windows.SystemParameters.PrimaryScreenWidth / 2;
            props.rcDestination = new User32.RECT
            {
                Left = screenWidthMiddle + sourceRect.Left / 2,
                Top = sourceRect.Top,
                Right = screenWidthMiddle + sourceRect.Right / 2,
                Bottom = sourceRect.Bottom
            };


            DwmApi.DwmUpdateThumbnailProperties(this.ThumbRight, ref props);

        }


        public override string ToString()
        {
            return this.Title;
        }
    }

}

