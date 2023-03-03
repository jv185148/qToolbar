using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qData
{
    public static class Shortcut
    {
        public static void Create(string filePath)
        {

        }

        public static qCommon.Interfaces.iShortcut LoadShortcut(string fileName)
        {
 
            DesktopShortcut shortcut = new DesktopShortcut();

            WshShell shell = new WshShell();
            IWshShortcut _shortcut = (IWshShortcut)shell.CreateShortcut(fileName);
            
            shortcut.Description = _shortcut.Description;
            shortcut.IconLocation = _shortcut.IconLocation;
            shortcut.TargetPath = _shortcut.TargetPath;
            shortcut.WorkingDirectory = _shortcut.WorkingDirectory;
       
            if(shortcut.Description == "")
            {
                string d = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                d = d.Substring(0, d.LastIndexOf("."));
                shortcut.Description = d;
            }
            shell = null;
            _shortcut = null;
            return shortcut;

        }

        private static string GetSteamIconPath(string file)
        {
            uint appId = GetSteamAppID(file);
            string manifestPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Steam", "steamapps", "appmanifest_" + appId.ToString() + ".acf");

            // read the contents of the appmanifest file
            string[] lines = System.IO.File.ReadAllLines(manifestPath);

            // find the line that contains the icon file name
            string iconLine = Array.Find(lines, line => line.Contains("\"icon\""));

            // extract the icon file name from the line
            string[] iconParts = iconLine.Split('"');
            string iconName = iconParts[3];

            // construct the path to the icon file
            string iconPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Steam", "steamapps", "appmanifest_" + appId.ToString() + ".acf");
            iconPath = Path.GetDirectoryName(iconPath) + "\\" + iconName + ".ico";

            // return the icon file path
            return iconPath;
        }

        private static uint GetSteamAppID(string file)
        {
            uint id = 1;

            return id;
        }

        public class DesktopShortcut : qCommon.Interfaces.iShortcut
        {
            private string description;
            private string iconLocation;
            private string targetPath;
            private string workingDirectory;

            public string Description { get => description; set => description = value; }
            public string IconLocation { get => iconLocation; set => iconLocation = value; }
            public string TargetPath { get => targetPath; set => targetPath = value; }
            public string WorkingDirectory { get => workingDirectory; set => workingDirectory = value; }

            public void Dispose()
            {
                description = string.Empty;
                IconLocation = string.Empty;
                targetPath = string.Empty;
                workingDirectory = string.Empty;
            }
        }
    }


}

