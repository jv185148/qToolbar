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
    /// Interaction logic for DirectoryBrowser.xaml
    /// </summary>
    public partial class DirectoryBrowser : UserControl
    {

        internal delegate void dFolderSelectionChanged(string folder);
        internal event dFolderSelectionChanged FolderSelectionChanged;

        internal void SelectFirst()
        {
            cboDrive.SelectedIndex = 0;
        }

        public DirectoryBrowser()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string[] drives = System.Environment.GetLogicalDrives();
            foreach (string drive in drives)
            {
                cboDrive.Items.Add(drive);
            }

            //cboDrive.SelectedIndex = 0;

        }

        private void cboDrive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadDriveContent(cboDrive.SelectedItem.ToString());
        }

        private void loadDriveContent(string drive)
        {
            treeFolders.Items.Clear();

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(drive);

            foreach (var dir in di.GetDirectories())
            {
                TreeViewItem item = new TreeViewItem();
                item.Tag = dir.FullName;
                item.Header = dir.Name;
                item.Expanded += Item_Expanded;
                item.Selected += Item_Selected;


                treeFolders.Items.Add(item);
            }

            treeFolders.UpdateLayout();

            System.Threading.Thread t = new System.Threading.Thread(() =>
            {
                foreach (TreeViewItem treeItem in treeFolders.Items)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (HasSubFolders(treeItem.Tag.ToString()))
                        {
                            foreach (var subItem in LoadMore(treeItem.Tag.ToString()))
                            {
                                treeItem.Items.Add(subItem);
                            }
                        }
                    });
                    System.Threading.Thread.Sleep(10);
                }
            });
            t.Start();
        }

        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            this?.FolderSelectionChanged((e.Source as TreeViewItem).Tag.ToString());
            e.Handled = true;
        }

        System.Threading.Thread subItemThread;

        private void Item_Expanded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            TreeViewItem item = (TreeViewItem)e.Source;
            //this.Tag = e.Source;

            if (subItemThread != null && subItemThread.IsAlive)
                subItemThread.Abort();

            subItemThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(DiscoverSubFolders));
            subItemThread.SetApartmentState(System.Threading.ApartmentState.MTA);
            subItemThread.IsBackground = false;

            subItemThread.Start(item);
        }

        void DiscoverSubFolders(Object oItem)
        {

            TreeViewItem item = (TreeViewItem)oItem;
            foreach (TreeViewItem subItem in item.Items)
            {
                try
                {

                    this.Dispatcher.Invoke(() =>
                    {
                        if (HasSubFolders(subItem.Tag.ToString()))
                        {
                            subItem.Items.Clear();
                            foreach (var newItem in LoadMore(subItem.Tag.ToString()))
                            {
                                //if (!HasItem(subItem, newItem))
                                subItem.Items.Add(newItem);
                            }
                        }
                    });
                }
                catch
                {
                    subItemThread.Abort();
                }
               // System.Threading.Thread.Sleep(1);
            }

        }

        private bool HasItem(TreeViewItem mainItem, TreeViewItem newItem)
        {
            bool result = false;
            foreach (TreeViewItem childItem in mainItem.Items)
            {

                result = childItem.Header.Equals(newItem.Header);

                if (result) break;
            }
            return result;
        }

        private TreeViewItem[] LoadMore(string path)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            List<TreeViewItem> subItems = new List<TreeViewItem>();
            foreach (var dir in di.GetDirectories())
            {
                TreeViewItem item = new TreeViewItem();
                item.Tag = dir.FullName;
                item.Header = dir.Name;
                item.Expanded += Item_Expanded;
                item.Selected += Item_Selected;

                subItems.Add(item);
            }

            return subItems.ToArray();
        }

        private bool HasSubFolders(string path)
        {
            try
            {
                return new System.IO.DirectoryInfo(path).GetDirectories().Length > 0;
            }
            catch
            {
                return false;
            }

        }


    }
}
