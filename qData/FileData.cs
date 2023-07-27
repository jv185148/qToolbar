using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace qData
{
    public class FileData : IDisposable
    {
        const string DEFAULT_FILE = "fileData.qtb";
        string qtbFile = "";

        string path = System.AppDomain.CurrentDomain.BaseDirectory;


        public FileData(string qtbFile)
        {
            if (qtbFile == "default")
                qtbFile = DEFAULT_FILE;
            else
                path += "\\ShortcutCollections";

            this.qtbFile = qtbFile.EndsWith(".qtb")?qtbFile:qtbFile+".qtb";
        }

        public void Dispose()
        {
            qtbFile = string.Empty;
            path = string.Empty;
        }

        public void Save(qControls.qFileButton[] buttons)
        {
            string data = createData(buttons);
            byte[] buffer = Encoding.UTF8.GetBytes(data);

            System.IO.FileStream fs = new System.IO.FileStream(path + "\\" + qtbFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            fs.Write(buffer, 0, buffer.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();

            Array.Clear(buffer, 0, buffer.Length);

        }

        private string createData(qControls.qFileButton[] buttons)
        {
            string data = "";
            int index = 0;

            foreach (var button in buttons)
            {
                index += 1;
                if (button.IconLocation == "") button.IconLocation = null;
                if (button.Arguments == "") button.Arguments = null;
                string description = button.Description;
                string isShortcut = button.isShortcut ? "1" : "0";
                string iconLocation = button.IconLocation ?? "NA";
                string targetPath = button.TargetPath;
                string arguments = button.Arguments ?? "NA";
                string workingDirectory = button.WorkingDirectory ?? "NA";
                string isSteamApp = button.IsSteamApp.ToString();

                StringBuilder buttonData = new StringBuilder();
                buttonData.AppendLine("button" + (index < 10 ? "0" : "") + index);
                buttonData.AppendLine(description);
                buttonData.AppendLine(isShortcut);
                buttonData.AppendLine(iconLocation);
                buttonData.AppendLine(targetPath);
                buttonData.AppendLine(arguments);
                buttonData.AppendLine(workingDirectory);
                buttonData.AppendLine(isSteamApp);
                buttonData.AppendLine(getBase64(button.Image));
                buttonData.AppendLine(); buttonData.AppendLine();

                data += buttonData.ToString();
            }


            return data;
        }

        private string getBase64(ImageSource image)
        {
            byte[] buffer;
            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            System.Windows.Media.Imaging.PngBitmapEncoder encoder = new PngBitmapEncoder();
            //encoder.QualityLevel = 100;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image));
            encoder.Save(ms);

            buffer = ms.ToArray();
            ms.Close();
            ms.Dispose();

            string retval = Convert.ToBase64String(buffer);
            Array.Clear(buffer, 0, buffer.Length);
            buffer = null;

            return retval;
        }

        public qControls.qFileButton[] Load()
        {

            if (!System.IO.File.Exists(path + "\\" + qtbFile))
                return null;
            System.IO.FileStream fs = new System.IO.FileStream(path + "\\" + qtbFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            fs.Dispose();

            qControls.qFileButton[] buttons = null;
            try
            {
                buttons = getButtons(buffer);
            }
            catch (qCommon.Exceptions.ReadingException ex)
            {
                throw ex;
            }
            return buttons;
        }

        private qControls.qFileButton[] getButtons(byte[] buffer)
        {
            List<qControls.qFileButton> buttons = new List<qControls.qFileButton>();

            qData.SettingsFile settings = new SettingsFile();

            try
            {
                settings.Load();

                string[] data = Encoding.UTF8.GetString(buffer).Split(new string[] { "\r\n\r\n\r\n" }, StringSplitOptions.None);
                for (int i = 0; i <= data.Length - 1; i++)
                {
                    string d = data[i];
                    if (d == "")
                        continue;
                    qControls.qFileButton button = new qControls.qFileButton();
                    //buttonData.AppendLine("button" + (index < 10 ? "0" : "") + index);   << 0
                    //buttonData.AppendLine(button.Description);
                    //buttonData.AppendLine((button.isShortcut ? "1" : "0"));
                    //buttonData.AppendLine(button.IconLocation);
                    //buttonData.AppendLine(button.TargetPath);
                    //buttonData.AppendLine(button.WorkingDirectory);
                    //buttonData.AppendLine(getBase64(button.Image));
                    //buttonData.AppendLine(); buttonData.AppendLine();

                    string[] lines = d.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                    button.Description = lines[1];
                    button.isShortcut = lines[2] == "1";

                    button.TargetPath = lines[4];
                    button.Arguments = lines[5] == "NA" ? "" : lines[5];
                    button.WorkingDirectory = lines[6] == "NA" ? "" : lines[6];
                    button.IsSteamApp = bool.Parse(lines[7]);
                    button.IconLocation = lines[3] == "NA" ? "" : lines[3];
                    button.Image = getImageSource(lines[8]);
                    button.RunAdmin = q.Common.GetAdminFlag(button.TargetPath);
                    button.SelectedBrush = settings.SelectColor;
                    button.TextForegroundSelect = settings.ForegroundSelectColor;
                    button.TextForeground = settings.ForegroundColor;
                    button.RunWithSingleClick = settings.RunWithSingleClick;

                    button.Refresh();

                    buttons.Add(button);
                }
                return buttons.ToArray();
            }
            catch (FormatException e)
            {
                throw new qCommon.Exceptions.ReadingException();
            }
            finally
            {
                buttons.Clear();
            }
        }

        public void Delete()
        {
            string file = path + "\\" + qtbFile;
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);

            }
        }

        private ImageSource getImageSource(string base64)
        {
            byte[] buffer = Convert.FromBase64String(base64);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();


            BitmapImage retval = new BitmapImage();
            retval = bitmap.Clone();

            bitmap = null;
            stream.Close();
            stream.Dispose();

            return retval;
        }
    }


}

