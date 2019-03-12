using DesktopSbS.Model;
using DesktopSbS.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SWF = System.Windows.Forms;


namespace DesktopSbS.View
{
    /// <summary>
    /// Logique d'interaction pour SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {

        #region Screens

        public Rectangle AllScreensBounds { get; private set; }

        public List<Tuple<int, SWF.Screen>> AllScreens { get; private set; }

        private int screenSourceId;
        public int ScreenSourceId
        {
            get { return this.screenSourceId; }
            set
            {
                if (this.screenSourceId != value)
                {
                    this.screenSourceId = value;
                    this.updateAreaSourceBound(true);
                    this.RaisePropertyChanged();
                }
            }
        }

        private int _ScreenDestinationId;
        public int ScreenDestinationId
        {
            get { return this._ScreenDestinationId; }
            set
            {
                if (this._ScreenDestinationId != value)
                {
                    this._ScreenDestinationId = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private bool _IsScreenScaleAuto;
        public bool IsScreenScaleAuto
        {
            get { return this._IsScreenScaleAuto; }
            set
            {
                if (this._IsScreenScaleAuto != value)
                {
                    this._IsScreenScaleAuto = value;
                    if (this._IsScreenScaleAuto)
                    {
                        this.ScreenScalePerCent = (int)(Options.GetScreenScaleAuto() * 100);
                    }
                    this.RaisePropertyChanged();
                }
            }
        }

        private int _ScreenScalePerCent;
        public int ScreenScalePerCent
        {
            get { return this._ScreenScalePerCent; }
            set
            {
                if (this._ScreenScalePerCent != value)
                {
                    this._ScreenScalePerCent = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private bool _IsSourceFullScreen;
        public bool IsSourceFullScreen
        {
            get { return this._IsSourceFullScreen; }
            set
            {
                if (this._IsSourceFullScreen != value)
                {
                    this._IsSourceFullScreen = value;
                    this.updateAreaSourceBound();
                    this.RaisePropertyChanged();
                }
            }
        }

        private int _AreaLeft;
        public int AreaLeft
        {
            get { return this._AreaLeft; }
            set
            {
                if (this._AreaLeft != value)
                {
                    this._AreaLeft = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private int _AreaTop;
        public int AreaTop
        {
            get { return this._AreaTop; }
            set
            {
                if (this._AreaTop != value)
                {
                    this._AreaTop = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private int _AreaWidth;
        public int AreaWidth
        {
            get { return this._AreaWidth; }
            set
            {
                if (this._AreaWidth != value)
                {
                    this._AreaWidth = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private int _AreaHeight;
        public int AreaHeight
        {
            get { return this._AreaHeight; }
            set
            {
                if (this._AreaHeight != value)
                {
                    this._AreaHeight = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #region SetScreenSource

        private RelayCommand cmdSetScreenSource;
        public ICommand CmdSetScreenSource
        {
            get
            {
                if (cmdSetScreenSource == null) { cmdSetScreenSource = new RelayCommand(fctSetScreenSource, canSetScreenSource, true); }
                return cmdSetScreenSource;
            }
        }

        private bool canSetScreenSource(object y)
        {
            return y is int id && id != this.ScreenSourceId;
        }

        private void fctSetScreenSource(object x)
        {
            if (CmdSetScreenSource.CanExecute(x))
            {
                this.ScreenSourceId = (int)x;
            }

        }

        #endregion

        #region SetScreenDestination

        private RelayCommand cmdSetScreenDestination;
        public ICommand CmdSetScreenDestination
        {
            get
            {
                if (cmdSetScreenDestination == null) { cmdSetScreenDestination = new RelayCommand(fctSetScreenDestination, canSetScreenDestination, true); }
                return cmdSetScreenDestination;
            }
        }

        private bool canSetScreenDestination(object y)
        {
            return y is int id && id != this.ScreenDestinationId;
        }

        private void fctSetScreenDestination(object x)
        {
            if (CmdSetScreenDestination.CanExecute(x))
            {
                this.ScreenDestinationId = (int)x;
            }

        }

        #endregion


        private void updateAreaSourceBound(bool force = false)
        {
            if (force || this.IsSourceFullScreen)
            {
                if (this.AllScreens.FirstOrDefault(t => t.Item1 == this.ScreenSourceId)?.Item2.Bounds is Rectangle rect)
                {
                    this.AreaLeft = rect.Left;
                    this.AreaTop = rect.Top;
                    this.AreaWidth = rect.Width;
                    this.AreaHeight = rect.Height;
                }
            }
        }

        #endregion

        #region Keyboard Shortcuts

        public ObservableCollection<Tuple<ModifierKeys, Key, ShortcutCommands>> KeyboardShortcuts { get; private set; }

        private Tuple<ModifierKeys, Key, ShortcutCommands> editedKeyboardShortcut = null;
        public Tuple<ModifierKeys, Key, ShortcutCommands> EditedKeyboardShortcut
        {
            get { return this.editedKeyboardShortcut; }
            set { this.editedKeyboardShortcut = value; RaisePropertyChanged(); }
        }

        #region EditShortcut

        private RelayCommand cmdEditShortcut;
        public ICommand CmdEditShortcut
        {
            get
            {
                if (cmdEditShortcut == null) { cmdEditShortcut = new RelayCommand(fctEditShortcut, canEditShortcut, true); }
                return cmdEditShortcut;
            }
        }

        private bool canEditShortcut(object y)
        {
            return y is Tuple<ModifierKeys, Key, ShortcutCommands> && this.EditedKeyboardShortcut == null;
        }

        private void fctEditShortcut(object x)
        {
            if (CmdEditShortcut.CanExecute(x))
            {
                Tuple<ModifierKeys, Key, ShortcutCommands> shortcut = (Tuple<ModifierKeys, Key, ShortcutCommands>)x;
                this.EditedKeyboardShortcut = shortcut;
            }

        }

        #endregion

        #region ResetShortcuts

        private RelayCommand cmdResetShortcuts;
        public ICommand CmdResetShortcuts
        {
            get
            {
                if (cmdResetShortcuts == null) { cmdResetShortcuts = new RelayCommand(fctResetShortcuts, canResetShortcuts, true); }
                return cmdResetShortcuts;
            }
        }

        private bool canResetShortcuts(object y)
        {
            return true;
        }

        private void fctResetShortcuts(object x)
        {
            if (CmdResetShortcuts.CanExecute(x))
            {
                this.EditedKeyboardShortcut = null;
                this.KeyboardShortcuts.Clear();
                Options.GetDefaultShortcuts().ForEach(this.KeyboardShortcuts.Add);
            }

        }

        #endregion

        private void SettingsWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!e.IsRepeat && this.EditedKeyboardShortcut != null)
            {
                Key key = e.Key == Key.System ? e.SystemKey : e.Key;
                ModifierKeys modifiers = e.KeyboardDevice.Modifiers;
                ShortcutCommands editedCommand = this.EditedKeyboardShortcut.Item3;

                switch (key)
                {
                    case Key.LWin:
                    case Key.RWin:
                    case Key.LeftShift:
                    case Key.RightShift:
                    case Key.LeftCtrl:
                    case Key.RightCtrl:
                    case Key.LeftAlt:
                    case Key.RightAlt:
                        // Don't consider modifiers keys
                        break;
                    case Key.Escape:
                        this.EditedKeyboardShortcut = null;
                        break;
                    default:
                        int editedIndex = this.KeyboardShortcuts.IndexOf(this.EditedKeyboardShortcut);
                        if (editedIndex > -1)
                        {
                            Tuple<ModifierKeys, Key, ShortcutCommands> conflictedShortcut = this.KeyboardShortcuts.FirstOrDefault(t => t.Item1 == modifiers && t.Item2 == key && t.Item3 != editedCommand);
                            if (conflictedShortcut != null)
                            {
                                if (MessageBox.Show(this, $"The shortcut \"{(modifiers != ModifierKeys.None ? $"{modifiers} + " : "")}{key}\" is already assigned to {conflictedShortcut.Item3} command.{Environment.NewLine}Do you want to swap shortcuts ?",
                                    "Shortcut conflict",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning) == MessageBoxResult.No)
                                {
                                    return;
                                }
                                int conflictIndex = this.KeyboardShortcuts.IndexOf(conflictedShortcut);
                                this.KeyboardShortcuts[conflictIndex] = new Tuple<ModifierKeys, Key, ShortcutCommands>(
                                    this.EditedKeyboardShortcut.Item1,
                                    this.EditedKeyboardShortcut.Item2,
                                    conflictedShortcut.Item3);
                            }
                            this.KeyboardShortcuts[editedIndex] = new Tuple<ModifierKeys, Key, ShortcutCommands>(modifiers, key, this.EditedKeyboardShortcut.Item3);
                            this.EditedKeyboardShortcut = null;
                        }
                        break;
                }
            }
        }


        #endregion

        #region Excluded Applications

        public ObservableCollection<NotifiableValue<string>> ExcludedApplications { get; private set; }

        #region AddApplication

        private RelayCommand cmdAddApplication;
        public ICommand CmdAddApplication
        {
            get
            {
                if (cmdAddApplication == null) { cmdAddApplication = new RelayCommand(fctAddApplication, canAddApplication, true); }
                return cmdAddApplication;
            }
        }

        private bool canAddApplication(object y)
        {
            return true;
        }

        private void fctAddApplication(object x)
        {
            if (CmdAddApplication.CanExecute(x))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    DefaultExt = ".exe",
                    Filter = "Executables (.exe)|*.exe|All files (.*)|*.*",
                    Title = "Select executable to exclude"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    string fileName = Path.GetFileName(openFileDialog.FileName);
                    if (!this.ExcludedApplications.Any(ea => ea.Value == fileName))
                    {
                        this.ExcludedApplications.Add(fileName);
                    }
                }

            }

        }

        #endregion

        #region RemoveApplication

        private RelayCommand cmdRemoveApplication;
        public ICommand CmdRemoveApplication
        {
            get
            {
                if (cmdRemoveApplication == null) { cmdRemoveApplication = new RelayCommand(fctRemoveApplication, canRemoveApplication, true); }
                return cmdRemoveApplication;
            }
        }

        private bool canRemoveApplication(object y)
        {
            return true;
        }

        private void fctRemoveApplication(object x)
        {
            if (CmdRemoveApplication.CanExecute(x) && x is NotifiableValue<string> app)
            {
                this.ExcludedApplications.Remove(app);
            }

        }

        #endregion

        #endregion

        #region Windows

        public SettingsWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            this.initProperties();

            this.PreviewKeyDown += SettingsWindow_PreviewKeyDown;
        }


        #region SaveClose

        private RelayCommand cmdSaveClose;
        public ICommand CmdSaveClose
        {
            get
            {
                if (cmdSaveClose == null) { cmdSaveClose = new RelayCommand(fctSaveClose, canSaveClose, true); }
                return cmdSaveClose;
            }
        }

        private bool canSaveClose(object y)
        {
            return true;
        }

        private void fctSaveClose(object x)
        {
            if (CmdSaveClose.CanExecute(x))
            {
                Options.ScreenSrcId = this.ScreenSourceId;
                Options.ScreenDestId = this.ScreenDestinationId;
                Settings.Default.ScreenScale = this.IsScreenScaleAuto ? 0 : this.ScreenScalePerCent / 100.0;
                if (this.IsSourceFullScreen)
                {
                    Settings.Default.AreaSrcOrigin = System.Drawing.Point.Empty;
                    Settings.Default.AreaSrcSize = System.Drawing.Size.Empty;
                }
                else
                {
                    Settings.Default.AreaSrcOrigin = new System.Drawing.Point(this.AreaLeft, this.AreaTop);
                    Settings.Default.AreaSrcSize = new System.Drawing.Size(this.AreaWidth, this.AreaHeight);
                }

                Options.KeyboardShortcuts = this.KeyboardShortcuts.ToList();
                Options.ExcludedApplications = this.ExcludedApplications.Select(ea => ea.Value).ToList();

                Options.Save();
                Options.InitializeScreenOptions();

                this.Close();
            }

        }

        #endregion

        #region Cancel

        private RelayCommand cmdCancel;
        public ICommand CmdCancel
        {
            get
            {
                if (cmdCancel == null) { cmdCancel = new RelayCommand(fctCancel, canCancel, true); }
                return cmdCancel;
            }
        }

        private bool canCancel(object y)
        {
            return true;
        }

        private void fctCancel(object x)
        {
            if (CmdCancel.CanExecute(x))
            {
                this.Close();
            }

        }

        #endregion

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.PreviewKeyDown -= SettingsWindow_PreviewKeyDown;

            base.OnClosed(e);

        }

        public void initProperties()
        {
            this.AllScreens = new List<Tuple<int, SWF.Screen>>();
            for (int i = 0; i < SWF.Screen.AllScreens.Length; ++i)
            {
                SWF.Screen screen = SWF.Screen.AllScreens[i];
                this.AllScreens.Add(new Tuple<int, SWF.Screen>(i + 1, screen));
                AllScreensBounds = Rectangle.Union(AllScreensBounds, screen.Bounds);
            }
            this.ScreenSourceId = Options.ScreenSrcId;
            this.ScreenDestinationId = Options.ScreenDestId;
            this.IsScreenScaleAuto = Settings.Default.ScreenScale <= 0;
            this.ScreenScalePerCent = (int)(Options.ScreenScale * 100);
            this.IsSourceFullScreen = Settings.Default.AreaSrcSize == System.Drawing.Size.Empty;
            if (this.IsSourceFullScreen)
            {
                this.updateAreaSourceBound();
            }
            else
            {
                this.AreaLeft = Options.AreaSrcBounds.Left;
                this.AreaTop = Options.AreaSrcBounds.Top;
                this.AreaWidth = Options.AreaSrcBounds.Width;
                this.AreaHeight = Options.AreaSrcBounds.Height;
            }

            this.KeyboardShortcuts = new ObservableCollection<Tuple<ModifierKeys, Key, ShortcutCommands>>(Options.KeyboardShortcuts);

            this.ExcludedApplications = Options.ExcludedApplications != null
                ? new ObservableCollection<NotifiableValue<string>>(Options.ExcludedApplications.Select(ea => (NotifiableValue<string>)ea))
                : new ObservableCollection<NotifiableValue<string>>();

        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
