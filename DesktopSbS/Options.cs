using DesktopSbS.Model;
using DesktopSbS.Properties;
using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace DesktopSbS
{
    public static class Options
    {
        public static int ParallaxEffect
        {
            get
            {
                return Settings.Default.ParallaxEffect;
            }
            set
            {
                Settings.Default.ParallaxEffect = Math.Max(0, value);
            }
        }

        public static bool ModeSbS
        {
            get
            {
                return Settings.Default.ModeSbS;
            }
            set
            {
                Settings.Default.ModeSbS = value;
            }
        }

        public static bool HideAboutOnStartup
        {
            get
            {
                return Settings.Default.HideAboutOnStartup;
            }
            set
            {
                Settings.Default.HideAboutOnStartup = value;
            }
        }

        public static bool KeepRatio
        {
            get
            {
                return Settings.Default.KeepRatio;
            }
            set
            {
                Settings.Default.KeepRatio = value;
            }
        }

        public static StringCollection ExcludedApplications
        {
            get
            {
                return Settings.Default.ExcludedApplications;
            }
        }

        #region Src

        public static int ScreenSrcId
        {
            get
            {
                return Math.Max(Math.Min(Settings.Default.ScreenSrcId, Screen.AllScreens.Length), 1);
            }
            set
            {
                Settings.Default.ScreenSrcId = value;
            }
        }

        public static double ScreenScale { get; private set; }

        public static Rectangle ScreenSrcBounds { get; private set; }

        public static Rectangle AreaSrcBounds { get; private set; }

        public static Rectangle ScreenSrcWorkspace { get; private set; }

        public static bool HideSrcCursor
        {
            get
            {
#if DEBUG
                return false;
#else
                return ScreenSrcId == ScreenDestId;
#endif
            }
        }

        #endregion

        #region Dest

        public static int ScreenDestId
        {
            get
            {
                return Math.Max(Math.Min(Settings.Default.ScreenDestId, Screen.AllScreens.Length), 1);
            }
            set
            {
                Settings.Default.ScreenDestId = value;
            }
        }

        public static Rectangle ScreenDestBounds { get; private set; }

        public static bool HideDestCursor
        {
            get
            {
                return Settings.Default.HideDestCursor;
            }
            set
            {
                Settings.Default.HideDestCursor = value;
            }
        }

        #endregion

        public static SbSComputedVariables ComputedVariables;

        static Options()
        {

            Options.ScreenSrcBounds = Screen.AllScreens[Options.ScreenSrcId - 1].Bounds;
            Options.ScreenSrcWorkspace = Screen.AllScreens[Options.ScreenSrcId - 1].WorkingArea;

            Options.AreaSrcBounds = new Rectangle(
                Settings.Default.AreaSrcOrigin != System.Drawing.Point.Empty ? Settings.Default.AreaSrcOrigin : Options.ScreenSrcBounds.Location,
                Settings.Default.AreaSrcSize != System.Drawing.Size.Empty ? Settings.Default.AreaSrcSize : Options.ScreenSrcBounds.Size
                );

            Options.ScreenDestBounds = Screen.AllScreens[Options.ScreenDestId - 1].Bounds;


            Options.ScreenScale = Settings.Default.ScreenScale;
            if (Options.ScreenScale <= 0.0)
            {
                using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
                    Options.ScreenScale = graphics.DpiX / 96.0;
            }

           
            ComputedVariables.UpdateVariables();
        }

        public static void Save()
        {
            Settings.Default.Save();
        }

    }
}
