using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopSbS
{
    public class CursorSbS
    {
        public CursorWindow ThumbLeft { get; private set; }
        public CursorWindow ThumbRight { get; private set; }

        public WinSbS Owner { get; set; }

        public User32.Win32Point Position { get; set; }

        public void RegisterThumbs()
        {


            this.ThumbLeft = new CursorWindow();
            this.ThumbRight = new CursorWindow();

            this.ThumbLeft.Show();
            this.ThumbRight.Show();

            this.UpdateThumbs();


        }

        public void UnRegisterThumbs()
        {

            this.ThumbLeft.Close();
            this.ThumbRight.Close();

        }



        public void UpdateThumbs()
        {


            User32.SetWindowPos(this.ThumbLeft.Handle, this.Owner?.ThumbLeft.Handle ?? IntPtr.Zero,
                this.Position.X / 2,
                this.Position.Y,
                16,
                32,
                0);


            int screenWidthMiddle = (int)System.Windows.SystemParameters.PrimaryScreenWidth / 2;

            User32.SetWindowPos(this.ThumbRight.Handle, this.Owner?.ThumbRight.Handle ?? IntPtr.Zero,
                screenWidthMiddle + this.Position.X / 2,
                this.Position.Y,
                16,
                32,
                0);
        }


    }
}
