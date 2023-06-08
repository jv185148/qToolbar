using Microsoft.Win32;
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
    /// Interaction logic for GetIconBox.xaml
    /// </summary>
    public partial class GetIconBox : Window
    {

        qFileButton button;

        public ImageSource newIcon;

        public GetIconBox()
        {
            InitializeComponent();
        }

        public void SetIcon(ref qFileButton button)
        {

            this.button = button;

            imgIcon.Source = button.Image.Clone();
        }

        private void chkProgramDefault_Checked(object sender, RoutedEventArgs e)
        {
            ImageSource source = null;

            CheckBox box = (CheckBox)sender;

            if (box.IsChecked == true)
            {
                if (box.Equals(chkProgramDefault))
                    chkFileDefault.IsChecked = false;

                if (box.Equals(chkFileDefault))
                    chkProgramDefault.IsChecked = false;
            }

            if (chkProgramDefault.IsChecked == true)
            {
                if (q.Common.IsFile(button.TargetPath))
                    source = qFileButton.GetGameIcon();
                else
                    source = qFileButton.GetFolderIcon();
            }
            else
            {
                source = button.Image.Clone();
            }

            imgIcon.Source = source;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {

            newIcon = imgIcon.Source;
            this.DialogResult = true;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            string imageFiles = "Image files *.bmp; *.jpg; *.png|*.bmp;*.jpg;*.png";

            dlg.Filter = "Application Files *.exe|*.exe|" + imageFiles + "|All files *.*|*.*";
            dlg.FilterIndex = 0;

            if (dlg.ShowDialog() == true)
            {
                ImageSource source = null;
                string file = dlg.FileName;

                if (q.Common.IsExeFile(file))
                    source = q.Common.IconX.GetIconFromFile(file);
                else if (q.Common.IsImageFile(file))
                {
                    source = q.Common.GetIconImage(file);
                }
                else
                {
                    MessageBox.Show("Couldn't use selected file as an image");
                    return;
                }
                imgIcon.Source = source.Clone();
            }
        }
    }
}
