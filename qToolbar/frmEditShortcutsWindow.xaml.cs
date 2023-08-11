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
    public partial class frmEditShortcutsWindow : Window, qCommon.Interfaces.iEditShortcutW
    {

        bool endResult = false;

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

        public string ShortcutName => selectedShortcut + ".qtb";

        public string LoadedShortcut { get; set; }

        bool _changed;
        public bool LoadedShortcutChanged
        {
            get
            {
                return _changed;
            }
            set
            {
                _changed = value;
            }
        }

        public frmEditShortcutsWindow()
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

            foreach (var file in di.GetFiles("*.qtb"))
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

                button.MouseRightButtonUp += Button_MouseRightButtonUp;
            }
            qData.SettingsFile settings = new qData.SettingsFile();
            settings.Load();
            selectedButtonColor = settings.SelectColor;

            settings.Dispose();
        }

        private void Button_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = new ContextMenu();
            Button button = (Button)sender;

            MenuItem mnuEdit = new MenuItem() { Header = "Rename shortcut collection" };
            mnuEdit.Click += ((object asender, RoutedEventArgs ee) =>
            {
                qControls.InputBox input = new qControls.InputBox();
                input.Heading = "Enter a new name for the shortcut collection";
                input.Text = button.Content.ToString();
                if (input.ShowDialog() == true)
                {
                    if (button.Content.ToString().Equals(LoadedShortcut.Substring(0, LoadedShortcut.LastIndexOf("."))))
                    {
                        LoadedShortcutChanged = true;
                        endResult = true;
                    }

                    Rename(input.Text,button.Tag.ToString());
                   
                }
            });

            MenuItem mnuDelete = new MenuItem() { Header = "Delete shortcut collection" };
            mnuDelete.Click += ((object oSender, RoutedEventArgs args) =>
            {
                Delete(button.Tag.ToString());
            });
            cm.Items.Add(mnuEdit);
            cm.Items.Add(mnuDelete);


            cm.PlacementTarget = (UIElement)sender;
            cm.IsOpen = true;


        }

        private void Delete(string filePath)
        {
            System.IO.File.Delete(filePath);
            LoadCollection();
        }

        private void Rename(string newFileName, string filePath)
        {
            string oldName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
            oldName = oldName.Substring(0, oldName.LastIndexOf("."));
            string newFilePath = filePath.Replace(oldName + ".qtb", newFileName + ".qtb");
            string dat = newFilePath.Replace(".qtb", ".dat");
            string oldDat = filePath.Replace(".qtb", ".dat");

            System.IO.File.Move(filePath, newFilePath);

            if (System.IO.File.Exists(oldDat))
            {
                System.IO.File.Move(oldDat, dat);
            }

            if (LoadedShortcutChanged)
                LoadedShortcut = newFileName+".qtb";
            LoadCollection();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            button.Background = selectedButtonColor;

            foreach (Button b in Panel.Children)
            {
                if (b != button)
                {
                    b.Background = defaultButtonColor;
                }
            }

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //this.DialogResult = false;
            this.Close();
        }


        private bool ButtonSelected(Button button)
        {
            return button.Background == selectedButtonColor;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = endResult;
        }
    }
}
