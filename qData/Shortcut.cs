using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
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


            shell = null;
            _shortcut = null;
            return shortcut;

        }

        public class DesktopShortcut : qCommon.Interfaces.iShortcut
        {
            public string Description { get; set; }
            public string IconLocation { get; set; }
            public string TargetPath { get; set; }
            public string WorkingDirectory { get; set; }
        }
    }


}

