using DesktopSbS.Model;
using DesktopSbS.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

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

        private static List<string> excludedApplications = Settings.Default.ExcludedApplications?.OfType<string>()?.ToList();
        public static List<string> ExcludedApplications
        {
            get
            {
                return excludedApplications;
            }
            set
            {
                excludedApplications = value;
                Settings.Default.ExcludedApplications = value.ToStringCollection();
            }
        }

        #region Shortcuts

        private static List<Tuple<ModifierKeys, Key, ShortcutCommands>> keyboardShortcuts = initKeyboardShortcuts();
        public static List<Tuple<ModifierKeys, Key, ShortcutCommands>> KeyboardShortcuts
        {
            get { return keyboardShortcuts; }
            set
            {
                keyboardShortcuts = value;

                Settings.Default.Shortcuts = value.ToStringCollection(t => $"{t.Item1};{t.Item2};{t.Item3}");
            }
        }

        private static List<Tuple<ModifierKeys, Key, ShortcutCommands>> readKeyboardShortcuts(StringCollection settingsShortcuts)
        {
            List<Tuple<ModifierKeys, Key, ShortcutCommands>> result = new List<Tuple<ModifierKeys, Key, ShortcutCommands>>(settingsShortcuts.Count);
            foreach (string s in settingsShortcuts)
            {
                string[] split = s.Split(';');
                if (split.Length == 3
                    && Enum.TryParse(split[0], out ModifierKeys modifiers)
                    && Enum.TryParse(split[1], out Key key)
                    && Enum.TryParse(split[2], out ShortcutCommands command))
                {
                    result.Add(new Tuple<ModifierKeys, Key, ShortcutCommands>(modifiers, key, command));
                }
            }

            return result;
        }

        private static List<Tuple<ModifierKeys, Key, ShortcutCommands>> initKeyboardShortcuts()
        {
            if (Settings.Default.Shortcuts != null)
            {
                return readKeyboardShortcuts(Settings.Default.Shortcuts);
            }
            else
            {
                return GetDefaultShortcuts();
            }
        }

        public static List<Tuple<ModifierKeys, Key, ShortcutCommands>> GetDefaultShortcuts()
        {
            List<Tuple<ModifierKeys, Key, ShortcutCommands>> result = new List<Tuple<ModifierKeys, Key, ShortcutCommands>>();
            result.Add(new Tuple<ModifierKeys, Key, ShortcutCommands>(ModifierKeys.Control | ModifierKeys.Alt, Key.F1, ShortcutCommands.About));
            result.Add(new Tuple<ModifierKeys, Key, ShortcutCommands>(ModifierKeys.Control | ModifierKeys.Alt, Key.W, ShortcutCommands.DecreaseParallax));
            result.Add(new Tuple<ModifierKeys, Key, ShortcutCommands>(ModifierKeys.Control | ModifierKeys.Alt, Key.X, ShortcutCommands.IncreaseParallax));
            result.Add(new Tuple<ModifierKeys, Key, ShortcutCommands>(ModifierKeys.Control | ModifierKeys.Alt, Key.C, ShortcutCommands.ShutDown));
            result.Add(new Tuple<ModifierKeys, Key, ShortcutCommands>(ModifierKeys.Control | ModifierKeys.Alt, Key.V, ShortcutCommands.SwitchSbSMode));
            result.Add(new Tuple<ModifierKeys, Key, ShortcutCommands>(ModifierKeys.Control | ModifierKeys.Alt, Key.B, ShortcutCommands.Pause3D));
            result.Add(new Tuple<ModifierKeys, Key, ShortcutCommands>(ModifierKeys.Control | ModifierKeys.Alt, Key.H, ShortcutCommands.HideDestCursor));
            result.Add(new Tuple<ModifierKeys, Key, ShortcutCommands>(ModifierKeys.Control | ModifierKeys.Alt, Key.K, ShortcutCommands.KeepRatio));
            return result;

        }

        #endregion

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

        public static double GetScreenScaleAuto()
        {
            double screenScale = 1;
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                screenScale = graphics.DpiX / 96.0;
            }

            return screenScale;
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

        #region Update

        public static bool CheckUpdateAtStartup
        {
            get
            {
                return Settings.Default.CheckUpdateAtStartup;
            }
            set
            {
                Settings.Default.CheckUpdateAtStartup = value;
            }
        }

        public static Version CurrentVersion { get; } = new Version(App.VERSION);

        public static Version LatestVersion
        {
            get
            {
                Version.TryParse(Settings.Default.LatestVersion, out Version version);
                if (version != null && version >= CurrentVersion)
                {
                    return version;
                }
                else
                {
                    return CurrentVersion;
                }
            }
            set { Settings.Default.LatestVersion = value.ToString(); }
        }

        #endregion

        public static SbSComputedVariables ComputedVariables;

        static Options()
        {
            InitializeScreenOptions();
        }

        public static void InitializeScreenOptions()
        {
            Options.ScreenSrcBounds = Screen.AllScreens[Options.ScreenSrcId - 1].Bounds;
            Options.ScreenSrcWorkspace = Screen.AllScreens[Options.ScreenSrcId - 1].WorkingArea;

            Options.AreaSrcBounds = new Rectangle(
                Settings.Default.AreaSrcOrigin != System.Drawing.Point.Empty ? Settings.Default.AreaSrcOrigin : Options.ScreenSrcBounds.Location,
                Settings.Default.AreaSrcSize != System.Drawing.Size.Empty ? Settings.Default.AreaSrcSize : Options.ScreenSrcBounds.Size
                );

            Options.ScreenDestBounds = Screen.AllScreens[Options.ScreenDestId - 1].Bounds;

            Options.ScreenScale = Settings.Default.ScreenScale > 0
                ? Settings.Default.ScreenScale
                : GetScreenScaleAuto();

            ComputedVariables.UpdateVariables();
        }

        public static void Save()
        {
            Settings.Default.Save();
        }

        public static StringCollection ToStringCollection<T>(this IEnumerable<T> list, Func<T, string> selector = null)
        {
            if (list?.Any() == true)
            {
                StringCollection result = new StringCollection();
                foreach (T item in list)
                {
                    string s = selector != null ? selector(item) : item.ToString();
                    if (!string.IsNullOrEmpty(s))
                    {
                        result.Add(s);
                    }
                }
                return result;
            }
            else
            {
                return null;
            }
        }


    }
}
