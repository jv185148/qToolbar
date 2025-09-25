using q;
using qCommon;
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
    /// Interaction logic for frmCompatDialog.xaml
    /// </summary>
    public partial class frmCompatDialog : Window, qCommon.Interfaces.iCompatDialog
    {
        public frmCompatDialog()
        {
            InitializeComponent();
        }

        qMain.cCompatDialog parent;

        public event Events.dClicked AcceptClicked;
        public event Events.dClicked CancelClicked;

        public ComboBox comboBox => cboCompat;

        public bool UseCompat { get { return (bool)chkCompat.IsChecked; } set { chkCompat.IsChecked = value; } }

        public string targetFile { get; set; }
        public Common.CompatFlags flag { get; set; }

        public Common.CompatFlags CurrentFlag { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            parent = new qMain.cCompatDialog(this);


        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            AcceptClicked?.Invoke(this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelClicked?.Invoke(this);
        }

        private void cboCompat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
