using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

namespace qControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class qFileButton : UserControl//, qCommon.Interfaces.iFileData
    {

        //public string Text { get => lblText.Content.ToString(); set => lblText.Content = value; }

        private delegate void TimerDelegate();

        public delegate void dClicked(object sender);

        public event dClicked Clicked;

        public delegate void dDragging(object sender);
        public delegate void dDraggingDone(object sender, System.Windows.Input.MouseEventArgs e);

        public event dDragging Dragging;
        public event dDraggingDone DraggingDone;


        public delegate void dRightClicked(object sender);
        public event dRightClicked RightClicked;

        private Brush _foreground;
        [Category("qToolbar")]
        public Brush TextForeground { get => lblText.Foreground; set => lblText.Foreground = value; }

        private Brush _selectedColor;
        public Brush SelectedBrush
        {
            get => _selectedColor; set
            {
                _selectedColor = value;
                if (ForceSelected)
                    Grid_MouseEnter(null, null);
            }
        }


        string iconLocation;
        string targetPath;
        string workingDirectory;
        bool isSteamApp;
        bool runAdmin;

        [Category("qToolbar")]
        public string Description
        {
            get
            {
                return (lblText.Content as TextBlock).Text;
            }
            set
            {
                TextBlock block = (TextBlock)lblText.Content;
                block.Text = value;
                lblText.Content = block;
            }
        }
        private event EventHandler IconChangedEvent;

        public bool isShortcut { get; set; }
        public string IconLocation { get => iconLocation; set { iconLocation = value; IconChangedEvent?.Invoke(this, null); } }
        [Category("qToolbar")]
        public string TargetPath { get => targetPath; set => targetPath = value; }
        [Category("qToolbar")]
        public string WorkingDirectory { get => workingDirectory; set => workingDirectory = value; }
        [Category("qToolbar")]
        public bool IsSteamApp { get => isSteamApp; set { isSteamApp = value; SetSteam(); } }

        public bool RunAdmin { get => runAdmin; set { runAdmin = value; SetRunAdmin(); } }

        private ImageSource imageSource;
        [Category("qToolbar")]
        public ImageSource Image
        {
            get
            {
                return imageSource;
            }
            set
            {
                imageSource = value;
                imgSource.Source = imageSource;
            }
        }

        public static ImageSource FolderIcon { get => GetFolderIcon(); }

        [Category("qToolbar")]
        public bool RunWithSingleClick { get; set; }

        private bool _forceSeleced;

        [Category("qToolbar")]
        public bool ForceSelected
        {
            get => _forceSeleced;
            set
            {
                if (value) Grid_MouseEnter(null, null);
                _forceSeleced = value;
            }
        }

        System.Timers.Timer dragTimer;

        bool draggingCalled;
        bool draggingDoneCalled;

        bool _dragging;
        WrapPanel wrapPanel => this.Parent as WrapPanel;
        bool dragging
        {
            get { return _dragging; }
            set
            {
                _dragging = value;
                if (value)
                {
                    if (!draggingCalled)
                        Dragging?.Invoke(this);
                    draggingCalled = true;
                    draggingDoneCalled = false;
                }

                else
                {
                    draggingDoneCalled = true;
                    draggingCalled = false;
                }
            }
        }

        int timerElapsedCount;

        bool buttonDown = false;

        public string ParentTitle { get => parentText; set => parentText = value; }
        private string parentText
        {
            get
            {
                return ((((this.Parent as WrapPanel).Parent as ScrollViewer).Parent as Grid).Parent as Window).Title;
            }
            set
            {
                ((((this.Parent as WrapPanel).Parent as ScrollViewer).Parent as Grid).Parent as Window).Title = value;
            }
        }
        internal void SetSteam()
        {
            if (!isSteamApp)
            {
                imgSteam.Visibility = Visibility.Hidden;
                return;
            }

            imgSteam.Source = q.Common.GetSteamIcon().Clone();
            imgSteam.Visibility = Visibility.Visible;
        }

        internal void SetRunAdmin()
        {
            if (RunAdmin)
                loadAdminImage();
            else
                imgAdmin.Visibility = Visibility.Hidden;
        }
        private void loadAdminImage()
        {
            var bitmap = Properties.Resources.adminShield;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            BitmapImage bImage = new BitmapImage();
            bImage.BeginInit();
            bImage.StreamSource = ms;
            bImage.CacheOption = BitmapCacheOption.OnLoad;
            bImage.EndInit();
            bImage.Freeze();


            bitmap.Dispose();

            imgAdmin.Source = bImage;
            imgAdmin.Visibility = Visibility.Visible;
        }

        public qFileButton()
        {
            InitializeComponent();
            IconChangedEvent += QFileButton_IconChangedEvent;
        }


        private static ImageSource GetFolderIcon()
        {
            var bitmap = Properties.Resources.Folder_icon;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            BitmapImage bImage = new BitmapImage();
            bImage.BeginInit();
            bImage.StreamSource = ms;
            bImage.CacheOption = BitmapCacheOption.OnLoad;
            bImage.EndInit();
            bImage.Freeze();


            bitmap.Dispose();

            return bImage;
        }

        private void QFileButton_IconChangedEvent(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(IconLocation))
                return;

            int location = 0;
            //string fileLocation = IconLocation.Split(',')[0];    //<=this causes issues. Split off last ','
            string fileLocation = iconLocation.Substring(0, iconLocation.LastIndexOf(','));

            int.TryParse(IconLocation.Split(',')[1], out location);

            if (fileLocation == "")
                fileLocation = targetPath;

            ImageSource bmp = q.Common.IconX.GetIconFromFile(fileLocation, location);
            if (bmp == null)
            {
                imageSource = imgSource.Source; // set to the default image
                return;
            }


            this.Image = bmp.Clone();

        }

        public void Dispose()
        {
            Image = null;

        }

        private void imgSource_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //did the mouseDown event start here? Used for MouseUp.
            buttonDown = true;
            // Dragging is true after 300ms of buttonDown


            //check for dragging
            dragTimer.Start();

            if (e.ChangedButton == MouseButton.Left && !RunWithSingleClick && e.ClickCount == 2)
                Clicked?.Invoke(this);
            else if (e.ChangedButton == MouseButton.Right)
                RightClicked?.Invoke(this);
        }


        private void imgSource_MouseMove(object sender, MouseEventArgs e)
        {

        }

        public void imgSource_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // If the MouseDown event did not start in this button, do not process anything here
            if (!buttonDown)
                return;

            if (dragging) goto skip;

            if (e.ChangedButton == MouseButton.Left && RunWithSingleClick && e.ClickCount == 1)
                Clicked?.Invoke(this);
            else if (e.ChangedButton == MouseButton.Right)
                RightClicked?.Invoke(this);

            skip:
            // we can't execute click events as we were dragging.
            dragTimer.Stop();
                DraggingDone?.Invoke(this,e);
           
            dragging = false;

            timerElapsedCount = 0;
            dragging = false;

            buttonDown = false;
        }

        public void LoadShortcut(qCommon.Interfaces.iShortcut shortcut)
        {
            this.Description = shortcut.Description;
            this.TargetPath = shortcut.TargetPath;
            this.IconLocation = shortcut.IconLocation;
            this.WorkingDirectory = shortcut.WorkingDirectory;
            this.IsSteamApp = false;

            this.RunAdmin = q.Common.GetAdminFlag(targetPath);

            shortcut.Dispose();
        }

        public void LoadFile(string file)
        {

        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid1.Background = SelectedBrush;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!ForceSelected)
                Grid1.Background = Brushes.Transparent;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dragTimer = new System.Timers.Timer();
            dragTimer.Interval = 100;
            dragTimer.Elapsed += DragTimer_Elapsed;
        }

        private void Timer_SetDragging()
        {
            dragging = true;
        }
        private void DragTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (timerElapsedCount > 3)
            {
                dragTimer.Stop();
                TimerDelegate td = new TimerDelegate(Timer_SetDragging);
                Application.Current.Dispatcher.BeginInvoke(td);

            }
            timerElapsedCount++;

        }

    }
}
