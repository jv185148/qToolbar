using Microsoft.Win32;
using qControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace qMain
{
    public class Main
    {
        public qCommon.Interfaces.iMain child;
        string appPath = System.AppDomain.CurrentDomain.BaseDirectory;

        bool collectionSaved = false;
        string collectionsPath
        {
            get
            {
                string path = appPath + "\\ShortcutCollections";
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                return path.Replace("\\\\","\\");
            }
        }
        public Main(qCommon.Interfaces.iMain child)
        {
            this.child = child;
        }

        public void Load()
        {
            collectionSaved = true;
            qData.FileData fileData = new qData.FileData();
            var buttons = fileData.Load();
            if (buttons == null)
                return;

            foreach (var button in buttons)
            {
                child.iGridArea.Children.Add(button);

                button.Clicked += Button_Clicked;
                button.RightClicked += Button_RightClicked;
            }



            Array.Clear(buttons, 0, buttons.Length);
            fileData.Dispose();
        }

        public void Close()
        {
            qData.FileData fileData = new qData.FileData();
            var buttons = getButtons();
            fileData.Save(buttons);

            fileData.Dispose();

        }

        private qControls.qFileButton[] getButtons()
        {
            List<qControls.qFileButton> list = new List<qFileButton>();
            for (int i = 0; i < child.iGridArea.Children.Count; i++)
            {
                list.Add((qFileButton)child.iGridArea.Children[i]);
            }

            return list.ToArray();
        }


        public void AddFiles(string[] data)
        {
            foreach (string file in data)
            {
                qControls.qFileButton button = new qControls.qFileButton();
                string name = file.Substring(file.LastIndexOf("\\") + 1);
                if (name.Contains('.'))
                    name = name.Substring(0, name.LastIndexOf('.'));

                string ext = "";
                if (file.Contains('.'))
                    ext = file.Substring(file.LastIndexOf('.'));

                bool isSteam = q.Common.IsSteamApp(file);

                if (ext.ToLower().Equals(".lnk"))
                {
                    button.isShortcut = true;
                    button.LoadShortcut(qData.Shortcut.LoadShortcut(file));
                }
                else
                {
                    button.Description = name;
                    button.TargetPath = file;

                    switch (ext)
                    {
                        case ".exe":
                            button.WorkingDirectory = file.Substring(0, file.LastIndexOf("\\"));
                            break;

                        case "":
                            button.Image = qControls.qFileButton.FolderIcon;
                            break;
                        default:
                            string iconPath = "";
                            if (isSteam)
                                iconPath= q.Common.GetSteamGameIcon(file);
                            else
                                iconPath = qData.Icons.GetAssociatedProgramName(ext);
                            button.IconLocation = iconPath + ",0";
                            button.IsSteamApp = isSteam;
                            break;

                            
                    }
                }

                child.iGridArea.Children.Add(button);

                button.Clicked += Button_Clicked;
                button.RightClicked += Button_RightClicked;

                button.SelectedBrush = qData.SettingsFile.SelectedTileColor;

                collectionSaved = false;
            }
        }

        private bool IsSteamApp(string file)
        {
            bool result = false;

            result = file.StartsWith("Steam");

            return result;
        }


        public void Clear()
        {
            int count = child.iGridArea.Children.Count;

            for (int i = count - 1; i >= 0; i--)
            {
                var thing = (qControls.qFileButton)child.iGridArea.Children[i];
                child.iGridArea.Children.RemoveAt(i);

                thing.Dispose();
                thing = null;
            }

            qData.FileData fileData = new qData.FileData();
            fileData.Delete();
            fileData.Dispose();

            Load();
        }

        public void RemoveButton(qControls.qFileButton button)
        {
            child.iGridArea.Children.Remove((UIElement)button);
            button.Dispose();
        }

        private void Button_Clicked(object sender)
        {

            qControls.qFileButton button = (qControls.qFileButton)sender;

            Execute(button);

        }

        public void Button_RightClicked(object sender)
        {
            //MessageBox.Show("Button was right clicked");

            qControls.qFileButton button = (qControls.qFileButton)sender;

            ContextMenu cm = new ContextMenu();
            MenuItem remove = new MenuItem() { Header = "Remove" };
            remove.Click += ((object dSender, RoutedEventArgs e) =>
            {
                RemoveButton(button);
            });
            


            MenuItem rename = new MenuItem() { Header = "Rename" };
            rename.Click+=((object rSender, RoutedEventArgs e) =>
            {
                qControls.InputBox input = new InputBox();
                input.Heading = "Enter a new name for the shortcut";
                input.Text = button.Description;
                if(input.ShowDialog() == true)
                {
                    button.Description = input.Text;
                }
            });

            
            cm.Items.Add(rename);
            cm.Items.Add(new Separator());
            cm.Items.Add(remove);

            cm.PlacementTarget = (UIElement)sender;
            cm.IsOpen = true;
        }


        private void Execute(qFileButton button)
        {
            System.Diagnostics.Process p = null;

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
                        WorkingDirectory = button.WorkingDirectory,
                        FileName = button.TargetPath
                    }

                };
            }
            p?.Start();
        }

        #region Shortcut Collection


        public void OpenShortcutCollection()
        {
            if (!collectionSaved)
            {
                string message = "The current Shortcut Collection is unsaved.\nAre you sure you want to continue?";
                if (MessageBox.Show(message, "Unsaved Collection", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "qToolbar Collection Files *.qtb|*.qtb";
            dlg.FilterIndex = 0;
            dlg.InitialDirectory = collectionsPath;

            dlg.FileOk += ((object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                string newFile = dlg.FileName;
                string oldFile = appPath + "\\fileData.qtb";

                System.IO.File.Copy(newFile, oldFile, true);

                Load();
            });
            dlg.ShowDialog();
        }

        public void SaveShortcutCollection()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "qToolbar CollectionFiles *.qtb|*.qtb";
            dlg.FilterIndex = 0;

            dlg.InitialDirectory = collectionsPath; //collectionsPath.Replace("\\\\","\\");

            if (dlg.ShowDialog() == true)
            {
                string oldFile = appPath + "\\fileData.qtb";
                string newFile = dlg.FileName;

                System.IO.File.Copy(oldFile, newFile, true);

                collectionSaved = true;
            }
        }

        #endregion

    }
}

