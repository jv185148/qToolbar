
using System.Windows;
using System.Windows.Media;

namespace qToolbar
{
    /// <summary>
    /// Interaction logic for frmSettings.xaml
    /// </summary>
    public partial class frmSettings : Window,qCommon.Interfaces.iSettings
    {
        public Brush TextColor { get; set; }
        public Brush SelectColor { get; set; }

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
            TextColor = btnForecolor.Background;
            SelectColor = btnSelectColor.Background;

            this.DialogResult = true;
            //this.Close();
        }

        private void btnColor(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;

            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();

            if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = dlg.Color;

                System.Windows.Media.Color c = q.Common.ConvertColor(color);
                button.Background = new System.Windows.Media.SolidColorBrush(c);

            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            qData.SettingsFile settings = new qData.SettingsFile();
            settings.Load();

            btnForecolor.Background = settings.ForegroundColor;
            btnSelectColor.Background = settings.SelectedTileColor;

            settings.Dispose();
        }
    }
}
