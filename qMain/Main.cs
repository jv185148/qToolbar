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

                return path.Replace("\\\\", "\\");
            }
        }
        public Main(qCommon.Interfaces.iMain child)
        {
            this.child = child;

            (child as Window).PreviewMouseUp += Main_PreviewMouseUp;
        }


        public void Load()
        {
            collectionSaved = true;
            qData.FileData fileData = new qData.FileData();
            qFileButton[] buttons = null;
            try
            {
                buttons = fileData.Load();
            }
            catch (qCommon.Exceptions.ReadingException exception)
            {
                MessageBox.Show("There was an exception loading file data. This could potentially be a version difference.\n\nData will be cleared");
                buttons = new qFileButton[0];
                fileData.Save(buttons);
            }
            if (buttons == null)
                return;

            foreach (var button in buttons)
            {
                Canvas.SetLeft(button, 0);
                Canvas.SetTop(button, 0);
                child.iWrapPanel.Children.Add(button);

                button.Clicked += Button_Clicked;
                button.RightClicked += Button_RightClicked;
                button.Dragging += Button_Dragging;
                button.DraggingDone += Button_DraggingDone;
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

        public void Save()
        {
            qData.FileData fileData = new qData.FileData();
            var buttons = getButtons();
            fileData.Save(buttons);

            fileData.Dispose();
        }

        private qControls.qFileButton[] getButtons()
        {
            List<qControls.qFileButton> list = new List<qFileButton>();
            for (int i = 0; i < child.iWrapPanel.Children.Count; i++)
            {
                list.Add((qFileButton)child.iWrapPanel.Children[i]);
            }

            return list.ToArray();
        }

        public void ShowSettings()
        {

            Window window = (Window)child.iSettingsForm;
            if (window.ShowDialog() == true)
            {


                qData.SettingsFile settings = new qData.SettingsFile();
                settings.ForegroundColor = ((qCommon.Interfaces.iSettings)window).ForegroundColor;
                settings.SelectedTileColor = ((qCommon.Interfaces.iSettings)window).SelectColor;
                settings.ForegroundSelectColor = ((qCommon.Interfaces.iSettings)window).ForegroundSelectColor;
                settings.RunWithSingleClick = ((qCommon.Interfaces.iSettings)window).RunWithSingleClick;
                settings.Save();
                settings.Dispose();

                LoadSettings();
            }
        }

        private void LoadSettings()
        {
            qData.SettingsFile settings = new qData.SettingsFile();
            settings.Load();
            foreach (var button in getButtons())
            {
                button.SelectedBrush = settings.SelectedTileColor;
                button.TextForegroundSelect = settings.ForegroundSelectColor;
                button.TextForeground = settings.ForegroundColor;
                button.RunWithSingleClick = settings.RunWithSingleClick;
            }
            settings.Dispose();
        }
        public void AddFiles(string[] data)
        {
            qData.SettingsFile settings = new qData.SettingsFile();
            settings.Load();

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
                            button.RunAdmin = q.Common.GetAdminFlag(button.TargetPath);
                            button.IconLocation = file + ",0";

                            break;

                        case "":
                            button.Image = qControls.qFileButton.FolderIcon;
                            break;
                        default:
                            string iconPath = "";
                            if (isSteam)
                                iconPath = q.Common.GetSteamGameIcon(file);
                            else
                                iconPath = qData.Icons.GetAssociatedProgramName(ext);
                            button.IconLocation = iconPath + ",0";
                            button.IsSteamApp = isSteam;
                            break;


                    }
                }

                Canvas.SetLeft(button, 0);
                Canvas.SetTop(button, 0);
                child.iWrapPanel.Children.Add(button);

                button.Clicked += Button_Clicked;
                button.RightClicked += Button_RightClicked;
                button.Dragging += Button_Dragging;
                button.DraggingDone += Button_DraggingDone;

                button.RunWithSingleClick = settings.RunWithSingleClick;

                button.SelectedBrush = settings.SelectedTileColor;
                button.TextForeground = settings.ForegroundColor;
                button.TextForegroundSelect = settings.ForegroundSelectColor;

                settings.Dispose();

                collectionSaved = false;
            }

            settings.Dispose();
        }

        qFileButton draggingButton;
        bool dragging;

        Point lastMousePosition;
        bool updateMousePosition;
        int newIndex;

        #region MouseEvents

        private void Main_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (draggingButton != null)
            {
                draggingButton.ReleaseMouseCapture();
                draggingButton.imgSource_MouseUp(draggingButton, e);

            }

        }


        private void Button_DraggingDone(object sender, System.Windows.Input.MouseEventArgs e)
        {
            dragging = false;


            // draggingButton.ParentTitle = "running";

            // Find the index of the new position for the button
            Point newPosition = e.GetPosition(child.iWrapPanel);
            newIndex = -1;
            double Ypos = 0;
            for (int i = 0; i < child.iWrapPanel.Children.Count; i++)
            {
                UIElement uiElement = child.iWrapPanel.Children[i];
                if (uiElement is qControls.qFileButton && uiElement != draggingButton)
                {
                    Point childPosition = uiElement.TranslatePoint(new Point(0, 0), child.iWrapPanel);
                    Ypos = childPosition.Y;
                    if (newPosition.X >= childPosition.X && newPosition.X <= childPosition.X + uiElement.RenderSize.Width
                        && newPosition.Y >= childPosition.Y && newPosition.Y <= childPosition.Y + uiElement.RenderSize.Height)
                    {
                        newIndex = i;
                        break;
                    }
                }
            }

            // Reorder the buttons in the WrapPanel
            //child.iWrapPanel.Children.Remove(draggingButton);
            draggingButton.RenderTransform = new TranslateTransform(0, 0);
            child.iGrid.Children.Remove(draggingButton);
            if (newIndex == -1)
            {
                child.iWrapPanel.Children.Add(draggingButton);
            }
            else
            {
                child.iWrapPanel.Children.Insert(newIndex, draggingButton);

            }

            child.iWrapPanel.ItemWidth = double.NaN;
            child.iWrapPanel.ItemHeight = double.NaN;
        }

        private void Button_Dragging(object sender)
        {
            dragging = true;
            draggingButton = (qFileButton)sender;
            draggingButton.MouseMove += Button_MouseMove;
            child.iWrapPanel.Children.Remove(draggingButton);
            child.iGrid.Children.Add(draggingButton);

            draggingButton.CaptureMouse();

            updateMousePosition = true;
        }
        private void Button_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (dragging && draggingButton != null)
            {
                if (updateMousePosition)
                {
                    lastMousePosition = e.GetPosition(child.iWrapPanel);
                    updateMousePosition = false;
                }

                Point newMousePosition = e.GetPosition(child.iWrapPanel);

                double deltaX = newMousePosition.X - lastMousePosition.X;
                double deltaY = newMousePosition.Y - lastMousePosition.Y;

                lastMousePosition = newMousePosition;

                Point newPosition = e.GetPosition(child.iGrid);

                Window window = (Window)child;
                double x = 0 - window.Width / 2;
                double y = 0 - window.Height / 2;
                x += newPosition.X;
                y += newPosition.Y;
                y += draggingButton.Height / 2;

                draggingButton.RenderTransform = new TranslateTransform(x, y);

            }
        }

        #endregion


        public void Clear()
        {
            int count = child.iWrapPanel.Children.Count;

            for (int i = count - 1; i >= 0; i--)
            {
                var thing = (qControls.qFileButton)child.iWrapPanel.Children[i];
                child.iWrapPanel.Children.RemoveAt(i);

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
            child.iWrapPanel.Children.Remove((UIElement)button);
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

            MenuItem admin = new MenuItem() { Header = "Toggle Run as Administrator" };
            admin.Click += ((object aSender, RoutedEventArgs e) =>
            {
                button.RunAdmin = q.Common.SetAdminFlag(button.TargetPath);
            });

            MenuItem remove = new MenuItem() { Header = "Remove" };
            remove.Click += ((object dSender, RoutedEventArgs e) =>
            {
                RemoveButton(button);
            });

            MenuItem edit = new MenuItem() { Header = "Edit" };
            edit.Click += ((object eSender, RoutedEventArgs e) =>
              {
                  qControls.InputBox input = new InputBox();
                  input.Heading = "Path for the file";
                  input.Text = button.TargetPath;
                  input.AddSubitem();
                  input.Subitems[0].Heading = "Executing Arguments";
                  input.Subitems[0].Content = button.Arguments;
                  if (input.ShowDialog() == true)
                  {
                      button.TargetPath = input.Text;
                      button.Arguments = input.Subitems[0].Content;
                  }
              });

            MenuItem rename = new MenuItem() { Header = "Rename" };
            rename.Click += ((object rSender, RoutedEventArgs e) =>
              {
                  qControls.InputBox input = new InputBox();
                  input.Heading = "Enter a new name for the shortcut";
                  input.Text = button.Description;
                  if (input.ShowDialog() == true)
                  {
                      button.Description = input.Text;
                  }
              });

            MenuItem changeIcon = new MenuItem() { Header = "Change Icon" };
            changeIcon.Click += ((object cSender, RoutedEventArgs e) =>
              {
                  GetIconBox box = new GetIconBox();
                  box.SetIcon(ref button);
                  if(box.ShowDialog() == true)
                  {
                      button.Image = box.newIcon.Clone();
                      Save();
                  }
              });

            cm.Items.Add(admin);
            cm.Items.Add(rename);
            cm.Items.Add(edit);
            cm.Items.Add(changeIcon);
            cm.Items.Add(new Separator());
            cm.Items.Add(remove);

            cm.Closed += ((object o, RoutedEventArgs e) =>button.ResetSelect());

            cm.PlacementTarget = (UIElement)sender;
            cm.IsOpen = true;


            button.ForceSelected = true;
        }



        private void Execute(qFileButton button)
        {
            System.Diagnostics.Process p = null;
            string target = button.TargetPath;
            string args = button.Arguments;


            if (q.Common.IsFile(target))
            {
                if (!System.IO.File.Exists(target))
                {
                    MessageBox.Show("This file doesn't exist on your hard drive.\n\n" + target,
                        "File doesn't exist", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

            }
            else
            {
                if (!System.IO.Directory.Exists(target))
                {
                    MessageBox.Show("This directory you're looking for doesn't exist.\n\n" + target,
                                "Directory doesn't exist", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }


            // When our shortcut referres to a file directly
            if (!button.isShortcut)
            {
                if (!string.IsNullOrEmpty(args))
                {
                    p = new System.Diagnostics.Process()
                    {

                        StartInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", target)
                        {
                            Arguments = args

                        }
                    };
                }
                else
                {
                    p = new System.Diagnostics.Process()
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", target)
                    };
                }
            }
            else
            {

                p = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                        Arguments = args,
                        WorkingDirectory = button.WorkingDirectory,
                        FileName = target
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

