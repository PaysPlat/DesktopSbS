using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSbS.Model
{
    public struct SbSComputedVariables
    {
        public double RatioX, RatioY;

        public int DestPositionX, DestPositionY, DecalSbSX, DecalSbSY;

        public void UpdateVariables()
        {
            bool modeSbS = Options.ModeSbS;
            bool keepRatio = Options.KeepRatio;

            // Size ratio between src size and dest size
            RatioX = (modeSbS ? 2.0 : 1.0) * Options.AreaSrcBounds.Width / Options.ScreenDestBounds.Width;
            RatioY = (!modeSbS ? 2.0 : 1.0) * Options.AreaSrcBounds.Height / Options.ScreenDestBounds.Height;

            DestPositionX = Options.ScreenDestBounds.Left;
            DestPositionY = Options.ScreenDestBounds.Top;

            if (keepRatio)
            {
                if (RatioX > RatioY)
                {
                    DestPositionY += (int)(Options.ScreenDestBounds.Height * (1 - RatioY / RatioX) / (!modeSbS ? 4 : 2));
                    RatioY = RatioX;
                }
                else
                {
                    DestPositionX += (int)(Options.ScreenDestBounds.Width * (1 - RatioX / RatioY) / (modeSbS ? 4 : 2));
                    RatioX = RatioY;
                }
            }

            DecalSbSX = modeSbS ? Options.ScreenDestBounds.Width / 2 : 0;
            DecalSbSY = modeSbS ? 0 : Options.ScreenDestBounds.Height / 2;

        }


    }

}
