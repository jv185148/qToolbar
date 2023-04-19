
using System.Windows;
using System.Windows.Media;

namespace qToolbar
{
    /// <summary>
    /// Interaction logic for frmSettings.xaml
    /// </summary>
    public partial class frmSettings : Window,qCommon.Interfaces.iSettings
    {
        public Brush ForegroundColor { get; set; }
        public Brush SelectColor { get; set; }
        public Brush ForegroundSelectColor { get; set; }

        public bool RunWithSingleClick
        {
            get
            {
                return (bool)radSingle.IsChecked;
            }
            set
            {
                radSingle.IsChecked = value;
                radDouble.IsChecked = !value;
            }
        }
        public frmSettings()
        {
            InitializeComponent();
        }

        private void bntCancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnAccept(object sender, RoutedEventArgs e)
        {
            ForegroundColor = btnForecolor.Foreground;
            SelectColor = btnSelectColor.Background;
            ForegroundSelectColor = btnForegroundSelectColor.Foreground;

            this.DialogResult = true;
            //this.Close();
        }

        private void btnForegroundColor(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;

            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();

            if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dlg.Color;

                System.Windows.Media.Color c = q.Common.ConvertColor(color);
                button.Foreground = new System.Windows.Media.SolidColorBrush(c);
            }
            btnGame.TextForeground = button.Foreground;
        }

        private void btnSelectColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;

            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dlg.Color;

                System.Windows.Media.Color c = q.Common.ConvertColor(color);
                button.Background = new System.Windows.Media.SolidColorBrush(c);
            }
            btnGame.SelectedBrush = button.Background;
           
        }

        private void btnTextSelectColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;

            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dlg.Color;

                System.Windows.Media.Color c = q.Common.ConvertColor(color);
                button.Foreground = new System.Windows.Media.SolidColorBrush(c);
            }
            btnGame.TextForegroundSelect = button.Background;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            qData.SettingsFile settings = new qData.SettingsFile();
            settings.Load();

            btnForecolor.Foreground = settings.ForegroundColor;
            btnSelectColor.Background = settings.SelectedTileColor;
            btnForegroundSelectColor.Foreground = settings.ForegroundSelectColor;
            RunWithSingleClick = settings.RunWithSingleClick;

            btnGame.TextForeground = btnForecolor.Foreground;
            btnGame.SelectedBrush = btnSelectColor.Background;
            btnGame.TextForegroundSelect = btnForegroundSelectColor.Foreground;

            btnGame.ForceSelected = true;

            settings.Dispose();
        }

    }
}
