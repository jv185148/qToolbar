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
        TextBox txtText;

        public List<Subitem> Subitems = new List<Subitem>();

        public InputBox()
        {
            InitializeComponent();
            txtText = new TextBox() { Margin = new Thickness(10, 5, 0, 0), Width = 363 };
            Panel.Children.Add(txtText);

        }

        private void SetHeight()
        {

            this.Height = txtText.RenderSize.Height + btnAccept.Height + 55;
            if (Subitems.Count == 0) goto skip;
            foreach(var item in Subitems)
            {
                this.Height += item.Label.RenderSize.Height;
                this.Height += item.TextBox.RenderSize.Height;
            }

            skip:
            return;
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
            SetHeight();

            txtText.Focus();
            txtText.SelectAll();
        }

        public void AddSubitem()
        {
            Subitem item = new Subitem();
            Subitems.Add(item);
            Panel.Children.Add(item.Label);
            Panel.Children.Add(item.TextBox);
          
        }

        public class Subitem
        {


            Label label = new Label();
            TextBox textBox = new TextBox() { Margin = new Thickness(10, 0, 0, 0), Width = 363 };

            public string Heading { get => label.Content.ToString(); set => label.Content = value; }
            public string Content { get => textBox.Text; set => textBox.Text = value; }

            internal Label Label => label;
            internal TextBox TextBox => textBox;
        }
    }
}
