// Decompiled with JetBrains decompiler
// Type: DesktopSbS.Options
// Assembly: DesktopSbS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 49DE7989-53B5-4209-8CFF-027C6071B6C3
// Assembly location: D:\Sources\Divers\DesktopSbS\DesktopSbS\bin\Release\DesktopSbS.exe

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

        public static StringCollection ExcludedApplications
        {
            get
            {
                return Settings.Default.ExcludedApplications;
            }
        }

        public static int TaskBarHeight { get; private set; }

        //public static int ScreenWidth { get; private set; }

        //public static int ScreenHeight { get; private set; }

        public static double ScreenScale { get; private set; }

        public static Rectangle ScreenBounds { get; private set; }

        
        public static int ScreenId
        {
            get
            {
                return Math.Max(Math.Min(Settings.Default.ScreenId, Screen.AllScreens.Length),1);
            }
            set
            {
                Settings.Default.ScreenId = value;
            }
        }


        static Options()
        {
            Options.TaskBarHeight = Settings.Default.TaskBarHeight;
            if (Options.TaskBarHeight <= 0)
                Options.TaskBarHeight = 40;
            Options.ScreenScale = Settings.Default.ScreenScale;
            if (Options.ScreenScale <= 0.0)
            {
                using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
                    Options.ScreenScale = (double)graphics.DpiX / 96.0;
            }
            //Options.ScreenWidth = Settings.Default.ScreenWidth;
            //if (Options.ScreenWidth <= 0)
            //    Options.ScreenWidth = (int)(SystemParameters.PrimaryScreenWidth * Options.ScreenScale);
            //Options.ScreenHeight = Settings.Default.ScreenHeight;
            //if (Options.ScreenHeight > 0)
            //    return;
            //Options.ScreenHeight = (int)(SystemParameters.PrimaryScreenHeight * Options.ScreenScale);
            Options.ScreenBounds = Screen.AllScreens[Options.ScreenId - 1].Bounds;

        }

        public static void Save()
        {
            Settings.Default.Save();
        }
    }
}
