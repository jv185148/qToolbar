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

        public string Description { get => lblText.Content.ToString(); set => lblText.Content = value; }
        public string IconLocation { get; set; }
        public string TargetPath { get; set; }
        public string WorkingDirectory { get; set; }

        public bool isShortcut { get; set; }


        public qFileButton()
        {
            InitializeComponent();
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

        }

        public void LoadFile(string file)
        {

        }
    }
}
