using qControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace qMain
{
    public class Main
    {

        public qCommon.Interfaces.iMain child;

        public Main(qCommon.Interfaces.iMain child)
        {
            this.child = child;
        }

        public void Load()
        {

        }

        public void AddFiles(string[] data)
        {
            foreach (string file in data)
            {
                qControls.qFileButton button = new qControls.qFileButton();
                string name = file.Substring(file.LastIndexOf("\\") + 1);
                if (name.Contains('.'))
                    name = name.Substring(0, name.LastIndexOf('.'));

                button.Description = name;
                button.TargetPath = file;


                string ext = "";
                if (file.Contains('.'))
                    ext = file.Substring(file.LastIndexOf('.'));
                
                if (ext.ToLower().Equals(".lnk"))
                {
                    button.isShortcut = true;
                    button.LoadShortcut( qData.Shortcut.LoadShortcut(file));
                }

                child.iGridArea.Children.Add(button);

                button.Clicked += Button_Clicked;
            }
        }

        private void Button_Clicked(object sender)
        {

            qControls.qFileButton button = (qControls.qFileButton)sender;

            Execute(button);

        }

        private void Execute(qFileButton button)
        {
            System.Diagnostics.Process p=null;

            // When our shortcut referres to a file directly
            if (!button.isShortcut)
            {
                p = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", button.TargetPath)
                };
            }
            else
            {

                p = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                         WorkingDirectory=button.WorkingDirectory,
                          FileName=button.TargetPath
                    }
                     
                };
            }
            p?.Start();
        }
    }
}

