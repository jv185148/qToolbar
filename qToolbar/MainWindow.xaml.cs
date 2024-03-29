﻿using qCommon.Interfaces;
using qMain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace qToolbar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, qCommon.Interfaces.iMain, IDisposable
    {
        private Main main;

        private bool _isMain;
        frmSettings fSettings;
        frmOpenIconCollection fOpen;
        frmEditShortcutsWindow fEditShortcutW;

        private string _shortcutFile;
        private string _background;
        int _shortcutCount;
        bool _doubleClickToRun;
        System.Drawing.Color _color;

        #region iMain properties

        public bool isMain
        {
            get => _isMain;
            set
            {
                _isMain = value;
                //mnuShortcuts.IsEnabled = value;
                mnuShortcuts.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                //mnuSettings.IsEnabled = value ? true : false;
            }
        }

        public string iShortcutFile
        {
            get => _shortcutFile;
            set => _shortcutFile = value.EndsWith(".qtb") ? value : value + ".qtb";
        }


        public qCommon.Interfaces.iOpenW iOpenWindow
        {
            get
            {
                if (fOpen == null)
                {
                    fOpen = new frmOpenIconCollection();
                    fOpen.Closed += FOpen_Closed;
                }
                return fOpen;
            }
        }

        public qCommon.Interfaces.iEditShortcutW iEditShortcutWindow
        {
            get
            {
                if (fEditShortcutW == null)
                {
                    fEditShortcutW = new frmEditShortcutsWindow();
                    fEditShortcutW.Closed += FEditShortcutW_Closed;
                }
                return fEditShortcutW;
            }
        }

        private void FEditShortcutW_Closed(object sender, EventArgs e)
        {
            //fEditShortcutW = null;
        }

        public int iShortcutCount
        {
            get
            {
                return _shortcutCount;
            }
            set
            {
                _shortcutCount = value;
                itmShortcutCount.Content = string.Format("{0} shortcuts loaded", _shortcutCount);
            }
        }

        public bool iDoubleClickToRun
        {
            get
            {
                return _doubleClickToRun;
            }
            set
            {
                _doubleClickToRun = value;
                itmDoubleClickToRun.Content = (value ? "Double " : "Single") + " click to open";
            }
        }

        public string iBackground
        {
            get => _background; set
            {
                _background = value;
                if (System.IO.File.Exists(value))
                    imgBackground.Source = new BitmapImage(new Uri(value, UriKind.Absolute));
                else
                    imgBackground.Source = null;
            }

        }

        public Point iSize
        {
            get => new Point(this.Width, this.Height); set
            {
                this.Width = value.X;
                this.Height = value.Y;
            }
        }
        public Point iPosition
        {
            get => new Point(this.Left, this.Top); set
            {
                this.Left = value.X;
                this.Top = value.Y;
            }
        }

        public System.Drawing.Color iBackgroundColor
        {
            get => _color;
            set
            {
                _color = value;
                this.Background = new SolidColorBrush(q.Common.ConvertColor(value));
            }
        }

        #endregion

        #region Settings.

        public qCommon.Interfaces.iSettings iSettingsForm
        {
            get
            {
                if (fSettings == null)
                {
                    fSettings = new frmSettings();

                    fSettings.Closed += FSettings_Closed;
                }

                return fSettings;
            }
        }

        private void FSettings_Closed(object sender, EventArgs e)
        {
            fSettings = null;
        }

        #endregion

        #region Open TLB Shortcut Window


        private void FOpen_Closed(object sender, EventArgs e)
        {
            fOpen = null;
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Dispose()
        {

        }

        public System.Windows.Controls.WrapPanel iWrapPanel => GridArea;

        public System.Windows.Controls.Grid iGrid => grid;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          
            this.Title = "qToolbar " + "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.main = new qMain.Main(this);
            main.Load();

        }

        #region Menu events

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void mnuClearItems_Click(object sender, RoutedEventArgs e)
        {
            this.main.Clear();
        }

        private void mnuSettings_Click(object sender, RoutedEventArgs e)
        {
            main.ShowSettings();
        }

        private void mnuSetBackground_Click(object sender, RoutedEventArgs e)
        {
            main.SetBackground();
        }
        private void mnuSetSolidBackground_Click(object sender, RoutedEventArgs e)
        {
            main.SetBackgroundColor();
        }


        private void mnuOpenShortcutCollection_Click(object sender, RoutedEventArgs e)
        {
            this.main.OpenShortcutCollection();
        }

        private void mnuSaveShortcutCollection_Click(object sender, RoutedEventArgs e)
        {
            this.main.SaveShortcutCollection();
        }


        private void mnuOpenAllShortcuts_Click(object sender, RoutedEventArgs e)
        {
            main.OpenAllShortcuts();
        }

        private void mnuCloseAllShortcuts_Click(object sender, RoutedEventArgs e)
        {
            main.CloseAllShortcuts();
        }

        private void mnuEditShortcuts_Click(object sender, RoutedEventArgs e)
        {
            main.EditShortcuts();
        }

        #endregion

        #region Drag events

        bool dragging;


        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                dragging = true;
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {

        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (dragging)
            {
                dragging = false;
                string[] data = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

                main.AddFiles(data);

            }
        }

        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            dragging = false;
        }

        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.main.Close();
        }


    }
}
