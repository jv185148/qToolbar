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
            string collectionsPath = path + "\\ShortcutCollections\\";

            qData.SettingsFile settings = new qData.SettingsFile();
            settings.Load();

            bool openAllShortcuts = settings.OpenAllShortcutFiles;
            settings.Dispose();

            bool setMain = false;
            if (e.Args.Length == 0)
            {
                //qMain.Main.isMain = true;
                setMain = true;
                qMain.Main.AllShortcutsOpened = false;
            }

            if (e.Args.Length > 0)
            {
                if (System.IO.File.Exists(collectionsPath + e.Args[0]))
                    file = e.Args[0];
            }

            window.iShortcutFile = file;
            window.isMain = setMain;


            MainWindow.Show();

        
        }


    }
}
