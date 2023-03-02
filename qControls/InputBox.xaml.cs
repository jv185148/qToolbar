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
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        public InputBox()
        {
            InitializeComponent();
        }

        public string Heading { get => this.Title; set => this.Title = value; }
        public string Text { get => txtText.Text; set => txtText.Text = value; }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void txtText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                btnCancel_Click(null, null);

            if (e.Key == Key.Enter)
                btnAccept_Click(null, null);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtText.Focus();
            txtText.SelectAll();
        }
    }
}
