using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace qToolbar
{
    /// <summary>
    /// Interaction logic for OpenWindow.xaml
    /// </summary>
    public partial class frmOpenIconCollection : Window,qCommon.Interfaces.iOpenW
    {

        string path
        {
            get
            {
                string str = System.AppDomain.CurrentDomain.BaseDirectory;

                if (str.EndsWith("\\"))
                    str = str.Substring(0, str.Length - 1);

                str = string.Format("{0}\\ShortcutCollections", str);
                return str;
            }
        }
        string selectedShortcut;

        public string ShortcutName => selectedShortcut;

        public frmOpenIconCollection()
        {
            InitializeComponent();

            LoadCollection();
        }

        Brush defaultButtonColor = null;
        Brush selectedButtonColor = null;

        private void LoadCollection()
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

            Panel.Children.Clear();

            foreach(var file in di.GetFiles("*.qtb"))
            {
                Button button = new Button();
                string name = file.Name.Substring(0, file.Name.LastIndexOf("."));
                button.Content = name;
                button.Tag = file.FullName;

                if (defaultButtonColor == null)
                    defaultButtonColor = button.Background;

                button.Click += Button_Click;
                button.Margin = new Thickness(10);
                button.FontSize = 20d;
                Panel.Children.Add(button);
            }
            qData.SettingsFile settings = new qData.SettingsFile();
            settings.Load();
            selectedButtonColor = settings.SelectedTileColor;

            settings.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            button.Background = selectedButtonColor;

            foreach(Button b in Panel.Children)
            {
                if(b != button)
                {
                    b.Background = defaultButtonColor;
                }
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            selectedShortcut = null;

            foreach(Button button in Panel.Children)
            {
                if (ButtonSelected(button))
                {
                    selectedShortcut = button.Content.ToString();
                    break;
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        private bool ButtonSelected(Button button)
        {
            return button.Background == selectedButtonColor;
        }
    }
}
