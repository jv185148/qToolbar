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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace qControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class qFileButton : UserControl, qCommon.Interfaces.iShortcut
    {

        //public string Text { get => lblText.Content.ToString(); set => lblText.Content = value; }

        public delegate void dClicked(object sender);

        public event dClicked Clicked;

        string iconLocation;
        string targetPath;
        string workingDirectory;

        public string Description { get => lblText.Content.ToString(); set => lblText.Content = value; }


        public bool isShortcut { get; set; }
        public string IconLocation { get => iconLocation; set => iconLocation = value; }
        public string TargetPath { get => targetPath; set => targetPath = value; }
        public string WorkingDirectory { get => workingDirectory; set => workingDirectory = value; }

        public qFileButton()
        {
            InitializeComponent();
        }


        public void Dispose()
        {
            IconLocation = string.Empty;
            targetPath = string.Empty;
            workingDirectory = string.Empty;
        }

        private void imgSource_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Clicked?.Invoke(this);
        }

        public void LoadShortcut(qCommon.Interfaces.iShortcut shortcut)
        {
            this.Description = shortcut.Description;
            this.IconLocation = shortcut.IconLocation;
            this.TargetPath = shortcut.TargetPath;
            this.WorkingDirectory = shortcut.WorkingDirectory;

            shortcut.Dispose();
        }

        public void LoadFile(string file)
        {

        }
    }
}
