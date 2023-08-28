using Microsoft.Win32;
using qControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        string appPath
        {
            get
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                if (path.EndsWith("\\"))
                {
                    path = path.Substring(0, path.Length - 1);
                }
                return path;
            }
        }



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


        System.IO.FileSystemWatcher fileWatcher;


        public Main(qCommon.Interfaces.iMain child)
        {
            this.child = child;

            (child as Window).PreviewMouseUp += Main_PreviewMouseUp;

            fileWatcher = new System.IO.FileSystemWatcher(System.AppDomain.CurrentDomain.BaseDirectory);

            fileWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite
                                    | System.IO.NotifyFilters.CreationTime
                                    | System.IO.NotifyFilters.Size;

            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.Filter = "*";
            fileWatcher.Changed += SettingsWatcher_Changed;

            child.RightClicked += Child_RightClicked;

        }


        private void SettingsWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            if (e.Name == "settings.ini")
            {
                Window w = (Window)child;
                w.Dispatcher.Invoke(() => { LoadSettings(); });
                //LoadSettings();
            }
        }

        #region Load & Save
        public void Load()
        {
            collectionSaved = true;
            qData.FileData fileData = new qData.FileData(child.iShortcutFile);


            qFileButton[] buttons = null;
            try
            {
                buttons = fileData.LoadButtons();
                if (fileData.ShortcutFileChanged)
                    child.iShortcutFile = fileData.ChangedFile;
            }
            catch (qCommon.Exceptions.ReadingException exception)
            {
                MessageBox.Show("There was an exception loading file data. This could potentially be a version difference.\n\nData will be cleared");
                buttons = new qFileButton[0];
                fileData.SaveButtons(buttons);
            }
            if (buttons == null)
                return;

            //Wrap panel must be cleared.
            child.iWrapPanel.Children.Clear();

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

            //qData.SettingsFile settings = new qData.SettingsFile();
            //settings.Load();
            //child.iShortcutCount = getButtons().Length;
            //child.iDoubleClickToRun = !settings.RunWithSingleClick;
            //settings.Dispose();

            LoadSettings();

            Array.Clear(buttons, 0, buttons.Length);
            fileData.Dispose();


            LoadWindowSettings();

            if (isMain)
            {
                CheckIfRunAllShortcuts();
            }

            child.UpdateTitle();
        }

        public void Save()
        {
            qData.FileData fileData = new qData.FileData(child.iShortcutFile);
            var buttons = getButtons();
            fileData.SaveButtons(buttons);

            fileData.Dispose();
        }

        #region Settings
        public void ShowSettings()
        {

            Window window = (Window)child.iSettingsForm;
            if (window.ShowDialog() == true)
            {


                qData.SettingsFile settings = new qData.SettingsFile();
                settings.ForegroundColor = ((qCommon.Interfaces.iSettings)window).ForegroundColor;
                settings.SelectColor = ((qCommon.Interfaces.iSettings)window).SelectColor;
                settings.ForegroundSelectColor = ((qCommon.Interfaces.iSettings)window).ForegroundSelectColor;
                settings.RunWithSingleClick = ((qCommon.Interfaces.iSettings)window).RunWithSingleClick;
                settings.OpenAllShortcutFiles = ((qCommon.Interfaces.iSettings)window).OpenAllShortcutFiles;

                settings.ShowBorders = ((qCommon.Interfaces.iSettings)window).ShowBorders;
                settings.ShowBorderForMain = ((qCommon.Interfaces.iSettings)window).ShowBorderForMain;
                settings.Save();
                settings.Dispose();

                if (!AllShortcutsOpened && settings.OpenAllShortcutFiles)
                {
                    //CheckIfRunAllShortcuts();
                }
                else
                {
                    //CloseExtraProcesses();
                }
                LoadSettings();
            }
        }

        private void LoadSettings()
        {
            qData.SettingsFile settings = new qData.SettingsFile();
            settings.Load();
            foreach (var button in getButtons())
            {
                button.SelectedBrush = settings.SelectColor;
                button.TextForegroundSelect = settings.ForegroundSelectColor;
                button.TextForeground = settings.ForegroundColor;
                button.RunWithSingleClick = settings.RunWithSingleClick;
                button.ForceSelected = false;

                button.Refresh();
            }

            child.iShortcutCount = getButtons().Length;
            child.iDoubleClickToRun = !settings.RunWithSingleClick;
            if (isMain)
                child.iShowBorder = settings.ShowBorderForMain;
            else
                child.iShowBorder = settings.ShowBorders;


            settings.Dispose();
        }


        #endregion //Settings

        #region WindowSettings

        private void SaveWindowSettings()
        {
            qData.WindowSettings windowSettings = new qData.WindowSettings(child.iShortcutFile);
            windowSettings.iWindowPosition = child.iPosition;
            windowSettings.iWindowSize = child.iSize;
            windowSettings.background = child.iBackground;
            windowSettings.iBackgroundColor = q.Common.ColorString(child.iBackgroundColor);
            windowSettings.Save();

            windowSettings.Dispose();
        }

        private void LoadWindowSettings()
        {
            qData.WindowSettings windowSettings = new qData.WindowSettings(child.iShortcutFile);
            windowSettings.Load();
            if (!string.IsNullOrEmpty(windowSettings.background))
                child.iBackground = windowSettings.background;
            else
                child.iBackground = "";

            child.iBackgroundColor = q.Common.ColorFromString(windowSettings.iBackgroundColor);


            if (!moveChildWindowToLocation)
            {
                child.iPosition = windowSettings.iWindowPosition;
                child.iSize = windowSettings.iWindowSize;
            }
            else
            {
                MoveChild(windowSettings.iWindowPosition, windowSettings.iWindowSize);
            }


            windowSettings.Dispose();
        }


        public void SetBackground()
        {
            qControls.OpenImageWindow openImage = new OpenImageWindow();
            if (openImage.ShowDialog() == true)
            {

                child.iBackground = openImage.SelectedImage;

                SaveWindowSettings();
            }
        }

        public void SetBackgroundColor()
        {
            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
            dlg.Color = child.iBackgroundColor;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                child.iBackgroundColor = dlg.Color;
                //child.iBackground = "";
            }
        }

        #endregion //WindowSettings

        #endregion // Load & Save

        public void CheckIfRunAllShortcuts()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            string collectionsPath = path + "\\ShortcutCollections\\";

            qData.SettingsFile settings = new qData.SettingsFile();
            settings.Load();
            bool openAllShortcuts = settings.OpenAllShortcutFiles;
            settings.Dispose();

            if (openAllShortcuts)
            {
                OpenAllShortcuts();
            }
        }

        private int getFileCount(string path)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            return di.GetFiles("*.qtb").Length;
        }

        private string[] collectionFiles(string path)
        {
            List<string> collections = new List<string>();
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            foreach (var file in di.GetFiles("*.qtb"))
            {
                collections.Add(file.Name);
            }

            return collections.ToArray();
        }

        #region Menu shortcut collections

        public void NewShortcutCollection()
        {
            string fileName = q.Common.GetFreeFileName();
            string file = collectionsPath + "\\" + fileName;

            System.IO.File.WriteAllText(file, "");
            System.Threading.Thread.Sleep(200);

            string exe = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            System.Diagnostics.Process p = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo(exe)
                {
                    Arguments = fileName
                }
            };

            p.Start();
        }



        public void OpenAllShortcuts()
        {

            int count = getFileCount(collectionsPath);
            if (count > 0)
            {
                string[] files = collectionFiles(collectionsPath);

                for (int i = 0; i < count; i++)
                {
                    if (isMain && files[i] == child.iShortcutFile)
                        continue;
                    // improve by using entire array.
                    if (IsOpen(files[i]))
                        continue;

                    string exe = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    //{
                    //    StartInfo = new System.Diagnostics.ProcessStartInfo(exe)
                    //    {
                    //        Arguments = files[i]
                    //    }
                    //};
                    p.StartInfo = new System.Diagnostics.ProcessStartInfo(exe);
                    p.StartInfo.Arguments = string.Format("{0} {1} {2} {3} {4}", files[i],
                        child.iPosition.X, child.iPosition.Y, child.iSize.X, child.iSize.Y);
                    p.Start();
                }

                AllShortcutsOpened = true;
            }
        }

        private bool IsOpen(string shortcutFileName)
        {
            string shortName = shortcutFileName;
            string exe = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

            if (shortName.Contains("\\"))
                shortName = shortName.Substring(shortName.LastIndexOf("\\") + 1);
            bool isOpen = false;

            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(exe);
            foreach (var process in processes)
            {
                string processName = process.MainWindowTitle.Substring(process.MainWindowTitle.IndexOf("[") + 1);
                if (string.IsNullOrEmpty(processName)) continue;
                processName = (processName.Substring(0, processName.IndexOf("]"))).Trim();

                isOpen = processName.Equals(shortName);

                if (isOpen) break;
            }

            return isOpen;


        }


        public void CloseAllShortcuts()
        {
            CloseExtraProcesses();
        }

        public void EditShortcuts()
        {
            child.iEditShortcutWindow.LoadedShortcut = child.iShortcutFile;

            Window editWindow = (Window)child.iEditShortcutWindow;

            if (editWindow.ShowDialog() == true)
            {
                if (child.iEditShortcutWindow.LoadedShortcutChanged)
                {
                    child.iShortcutFile = child.iEditShortcutWindow.LoadedShortcut;
                    Load();

                    qData.FileData fileData = new qData.FileData(child.iShortcutFile);
                    fileData.SetQTBDefult();
                    fileData.Dispose();
                }

                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("qToolbar");
                if (processes.Length > 1)
                {
                    CloseAllShortcuts();
                    OpenAllShortcuts();
                }

                Array.Clear(processes, 0, processes.Length);
            }


        }

        #endregion //Menu shortcut collections

        private void CloseExtraProcesses()
        {
            if (!isMain) return;
            var processes = System.Diagnostics.Process.GetProcessesByName("qToolbar");


            foreach (var process in processes)
            {

                if (process.Id == System.Diagnostics.Process.GetCurrentProcess().Id && isMain)
                    continue;

                process.CloseMainWindow();
            }

            Array.Clear(processes, 0, processes.Length);
        }

        public static bool AllShortcutsOpened;

        #region ShortcutButtons

        private qControls.qFileButton[] getButtons()
        {
            List<qControls.qFileButton> list = new List<qFileButton>();
            for (int i = 0; i < child.iWrapPanel.Children.Count; i++)
            {
                list.Add((qFileButton)child.iWrapPanel.Children[i]);
            }

            return list.ToArray();
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

                button.SelectedBrush = settings.SelectColor;
                button.TextForeground = settings.ForegroundColor;
                button.TextForegroundSelect = settings.ForegroundSelectColor;

                settings.Dispose();

                collectionSaved = false;
            }

            settings.Dispose();
        }

        public void RemoveButton(qControls.qFileButton button)
        {
            child.iWrapPanel.Children.Remove((UIElement)button);
            button.Dispose();
        }


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

            //qData.FileData fileData = new qData.FileData(child.iShortcutFile);
            //fileData.Delete();
            //fileData.Dispose();

            //Load();
        }

        #endregion //ShortcutButtons

        bool moveChildWindowToLocation = false;
        public void SetMoveWindowToLocation(bool value)
        {
            moveChildWindowToLocation = value;
        }

        private void MoveChild(Point position, Point size)
        {
            Window window = (Window)child;

            child.iSize = size;

            while (window.Left != position.X || window.Top != position.Y || window.Width != size.X || window.Height != size.Y)
            {
                if (window.Opacity < 1d)
                    window.Opacity += 0.1d;

                double amount = 0d;
                double moveAmount = 0.5d;
                if (window.Left < position.X)
                {
                    amount = position.X - window.Left > moveAmount ? moveAmount : position.X - window.Left;

                }
                else if (window.Left > position.X)
                {
                    amount = window.Left - position.X > moveAmount ? moveAmount : window.Left - position.X;
                    amount *= -1;
                }
                window.Left += amount;

                amount = 0d;
                if (window.Top < position.Y)
                {
                    amount = position.Y - window.Top > moveAmount ? moveAmount : position.Y - window.Top;
                }
                else if (window.Top > position.Y)
                {
                    amount = window.Top - position.Y > moveAmount ? moveAmount : window.Top - position.Y;
                    amount *= -1;
                }
                window.Top += amount;
            }

            child.iPosition = position;
        }

        #region Mouse Drag Events

        qFileButton draggingButton;
        bool dragging;

        Point lastMousePosition;
        bool updateMousePosition;
        int newIndex;

        public bool isMain { get => child.isMain; set => child.isMain = value; }

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

        #endregion //Mouse Drag Events


        #region Click Events


        private void Child_RightClicked(object sender)
        {
            // e.Handled = true;

            ContextMenu cm = new ContextMenu();

            MenuItem mnuBackgroundColor = new MenuItem() { Header = "Change Background Color" };
            mnuBackgroundColor.Click += ((object s, RoutedEventArgs args) =>
            {
                SetBackgroundColor();
            });

            MenuItem mnuSettings = new MenuItem() { Header = "Settings..." };
            mnuSettings.Click += ((object s, RoutedEventArgs args) =>
              {
                  ShowSettings();
              });

            cm.Items.Add(mnuBackgroundColor);
            cm.Items.Add(new Separator());
            cm.Items.Add(mnuSettings);

            cm.PlacementTarget = (UIElement)child;
            cm.IsOpen = true;
        }


        #region qButton Click Events

        private void Button_Clicked(object sender)
        {

            qControls.qFileButton button = (qControls.qFileButton)sender;

            Execute(button);

        }

        public void Button_RightClicked(object sender)
        {

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
                  if (box.ShowDialog() == true)
                  {
                      button.Image = box.newIcon.Clone();
                      Save();
                  }
              });

            MenuItem openLocation = new MenuItem() { Header = "Open File Location" };
            openLocation.Click += ((object oSender, RoutedEventArgs e) =>
              {
                  Explore(button.TargetPath);
              });
            cm.Items.Add(admin);
            cm.Items.Add(rename);
            cm.Items.Add(edit);
            cm.Items.Add(changeIcon);
            cm.Items.Add(openLocation);
            cm.Items.Add(new Separator());
            cm.Items.Add(remove);

            cm.Closed += ((object o, RoutedEventArgs e) => button.ResetSelect());

            cm.PlacementTarget = (UIElement)sender;
            cm.IsOpen = true;


            button.ForceSelected = true;
        }

        #endregion //qButton Click Events

        #endregion //Click Events

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
            bool runAsExe = false;
            if (q.Common.IsExeFile(button.TargetPath)) // run this as an EXE instead.
            {
                runAsExe = true;
                goto exe;
            }
            // When our shortcut referres to a file directly
            if (button.isShortcut)
            {

                if (!string.IsNullOrEmpty(args))
                {
                    p = new System.Diagnostics.Process()
                    {

                        StartInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", target)
                        {
                            Arguments = args,
                            WorkingDirectory = button.WorkingDirectory,

                        }
                    };
                }
                else
                {
                    p = new System.Diagnostics.Process()
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe", target)
                        {
                            WorkingDirectory = button.WorkingDirectory
                        }
                    };
                }
            }
        exe:
            if (!button.isShortcut || runAsExe)
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

        private void Explore(string filePath)
        {
            q.Common.ExploreFile(filePath);
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

            Window window = (Window)child.iOpenWindow;

            if (window.ShowDialog() == true)
            {
                string shortcutName = ((qCommon.Interfaces.iOpenW)window).ShortcutName;
                if (shortcutName == null)
                    return;
                child.iShortcutFile = shortcutName;
                Load();
            }
        }

        public void SaveShortcutCollection()
        {

            qControls.InputBox input = new InputBox();
            input.Heading = "Enter a name for your shortcut collection";

            if (input.ShowDialog() == true)
            {
                child.iShortcutFile = input.Text;
                Save();
                collectionSaved = true;
            }

        }

        #endregion //Shortcut Collection



        public void Close()
        {
            qData.FileData fileData = new qData.FileData(child.iShortcutFile);
            var buttons = getButtons();
            fileData.SaveButtons(buttons);

            if (isMain)
                fileData.SetQTBDefult();

            fileData.Dispose();

            SaveWindowSettings();

            if (AllShortcutsOpened)
            {
                CloseAllShortcuts();
            }
        }

    }
}

