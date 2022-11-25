using qMain;
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

namespace qToolbar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, qCommon.Interfaces.iMain, IDisposable
    {
        private Main main;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            
        }

        public System.Windows.Controls.WrapPanel iGridArea => GridArea;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.main = new qMain.Main(this);
            main.Load();
        }

        #region Menu events

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Drag events

        bool dragging;

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                dragging = true;
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {

        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (dragging)
            {
                dragging = false;
                string[] data =(string[]) e.Data.GetData(System.Windows.DataFormats.FileDrop);

                main.AddFiles(data);
               
            }
        }


        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            dragging = false;
        }

        #endregion

    }
}
