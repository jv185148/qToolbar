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
    /// Interaction logic for _Image.xaml
    /// </summary>
    public partial class _Image : UserControl
    {

        internal event EventHandler SelectedChanged;

        Brush plainBrush;

        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;

               
                SelectedChanged?.Invoke(this, new EventArgs());


            }

        }

        public string ImageFilePapth { get; set; }

        public _Image(string fileName)
        {
            InitializeComponent();
            this.SelectedChanged += _Image_SelectedChanged;

            imgBackground.Source = new BitmapImage(new Uri(fileName, UriKind.Absolute));

            lblFileName.Content = fileName.Substring(fileName.LastIndexOf("\\") + 1);

            plainBrush = this.Background;
            ImageFilePapth = fileName;
        }

        private void _Image_SelectedChanged(object sender, EventArgs e)
        {

            _Image image = (_Image)sender;
            if (image.Selected)
            {
                this.Background = Brushes.Blue;
            }
            else
            {
                this.Background = plainBrush;
            }
            
        }

        _Image[] GetImageButtons()
        {
            List<_Image> buttons = new List<_Image>();

            foreach(_Image img in (this.Parent as WrapPanel).Children)
            {
                buttons.Add(img);
            }
            return buttons.ToArray();
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Selected = true;
          
            //_Image imageButton = (_Image)sender;

            foreach (_Image image in GetImageButtons())
            {
                if (image == this)
                    continue;

                image.Selected = false;
            }

        }
    }
}
