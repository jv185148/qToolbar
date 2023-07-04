using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace qToolbar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow window = new MainWindow();
            string file = "default";
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            string collections = path + "\\ShortcutCollections\\";


            if (e.Args.Length == 0)
            {
                int count = getFileCount(collections);
                if(count > 0)
                {
                    string[] files = collectionFiles(collections);
                    for (int i = 1; i < count; i++)
                    {
                        string exe = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

                        System.Diagnostics.Process p = new System.Diagnostics.Process()
                        {
                            StartInfo = new System.Diagnostics.ProcessStartInfo(exe)
                            {
                                Arguments = files[i]
                            }
                        };
                        p.Start();
                    }
                    file = files[0];
                }
            }
            if (e.Args.Length > 0)
            {
                if (System.IO.File.Exists(collections + e.Args[0]))
                    file = e.Args[0];
            }

            window.iShortcutFile = file;
            MainWindow.Show();
        }

        private int getFileCount(string path)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            return di.GetFiles().Length;
        }

        private string[] collectionFiles(string path)
        {
            List<string> collections = new List<string>();
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            foreach(var file in di.GetFiles())
            {
                collections.Add(file.Name);
            }

            return collections.ToArray();
        }
    }
}
