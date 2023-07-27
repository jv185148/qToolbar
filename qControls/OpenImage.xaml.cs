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

namespace qControls
{
    /// <summary>
    /// Interaction logic for OpenImage.xaml
    /// </summary>
    public partial class OpenImageWindow : Window
    {
        string imageFiles;
        public string SelectedImage;

        List<string> imageTypes;

        public OpenImageWindow()
        {
            InitializeComponent();
        }

        private void dirBrowser_FolderSelectionChanged(string folder)
        {
            LoadImages(folder);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dirBrowser.SelectFirst();

            imageTypes = new List<string>();
            imageTypes.Add("png");
            imageTypes.Add("bmp");
            imageTypes.Add("jpg");
            imageFiles = "";
            foreach (string s in imageTypes)
            {
                imageFiles += string.Format("; *.{0}", s);
            }
            imageFiles = imageFiles.Substring(1);
        }

        private void LoadImages(string path)
        {
            panelImages.Children.Clear();
            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
                foreach (string fileType in imageTypes)
                {
                    foreach (System.IO.FileInfo fi in di.GetFiles("*." + fileType))
                    {
                        //System.Windows.Controls.Image img = new Image();
                        _Image img = new _Image(fi.FullName);

                        img.MaxHeight = 200;
                        img.MaxWidth = 200;
                        img.Margin = new Thickness(2, 2, 2, 2);
                        panelImages.Children.Add(img);
                    }
                }
            }
            catch(System.UnauthorizedAccessException notAuth)
            {
                return;
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            foreach (_Image image in panelImages.Children)
            {
                if(image.Selected)
                {
                    SelectedImage = image.ImageFilePapth;
                    break;
                }
            }
            this.Close();
        }
    }
}
