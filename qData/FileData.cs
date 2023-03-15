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
        string fileName = "fileData.qtb";

        string path = System.AppDomain.CurrentDomain.BaseDirectory;


        public FileData()
        {

        }
        public void Dispose()
        {
            fileName = string.Empty;
            path = string.Empty;
        }

        public void Save(qControls.qFileButton[] buttons)
        {
            string data = createData(buttons);
            byte[] buffer = Encoding.UTF8.GetBytes(data);

            System.IO.FileStream fs = new System.IO.FileStream(path + "\\" + fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
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
                StringBuilder buttonData = new StringBuilder();
                buttonData.AppendLine("button" + (index < 10 ? "0" : "") + index);
                buttonData.AppendLine(button.Description);
                buttonData.AppendLine((button.isShortcut ? "1" : "0"));
                buttonData.AppendLine(button.IconLocation);
                buttonData.AppendLine(button.TargetPath);
                buttonData.AppendLine(button.WorkingDirectory);
                buttonData.AppendLine(button.IsSteamApp.ToString());
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
            if (!System.IO.File.Exists(path + "\\" + fileName))
                return null;
            System.IO.FileStream fs = new System.IO.FileStream(path + "\\" + fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            fs.Dispose();

            qControls.qFileButton[] buttons = getButtons(buffer);
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
                    button.WorkingDirectory = lines[5];
                    button.IsSteamApp = bool.Parse(lines[6]);
                    button.IconLocation = lines[3];
                    button.Image = getImageSource(lines[7]);
                    button.RunAdmin = q.Common.GetAdminFlag(button.TargetPath);

                    button.SelectedBrush = settings.SelectedTileColor;
                    button.TextForeground = settings.ForegroundColor;

                  

                    buttons.Add(button);
                }
                return buttons.ToArray();
            }
            finally
            {
                settings.Dispose();
                buttons.Clear();
            }
        }

        public void Delete()
        {
            string file = path + "\\" + fileName;
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

