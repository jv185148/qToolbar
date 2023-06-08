using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace q
{
    public static class Common
    {
        public class IconX
        {
            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            public static extern uint ExtractIconEx(string lpszFile, int nIconIndex, [Out] IntPtr[] phiconLarge, [Out] IntPtr[] phiconSmall, [In] uint nIcons);

            [DllImport("user32.dll")]
            public static extern int DestroyIcon(IntPtr hIcon);

            public static ImageSource GetIconFromFile(string file, int index = 0)
            {
                ImageSource imageSource = null;

                uint iconCount = ExtractIconEx(file, -1, null, null, 1);
                if (iconCount == 0)
                    return null;

                if (index >= iconCount - 1)
                    index = (int)iconCount - 1;

                IntPtr[] phiconLarge = new IntPtr[iconCount];
                IntPtr[] phiconSmall = new IntPtr[iconCount];
                ExtractIconEx(file, index, phiconLarge, phiconSmall, iconCount);

                Icon[] iconsSmall = new Icon[iconCount];
                Icon[] iconsLarge = new Icon[iconCount];

                for (int i = 0; i < iconCount; i++)
                {
                    if (phiconLarge != null)
                    {
                        iconsLarge[i] = (Icon)Icon.FromHandle(phiconLarge[i]).Clone();
                        DestroyIcon(phiconLarge[i]);
                    }
                    if (phiconSmall != null)
                    {
                        iconsSmall[i] = (Icon)Icon.FromHandle(phiconSmall[i]).Clone();
                        DestroyIcon(phiconSmall[i]);
                    }
                }
                System.IO.MemoryStream stream = null;
                stream = new System.IO.MemoryStream();
                if (iconsLarge.Length > 0 && (iconsLarge.Length > index || iconsLarge.Length - 1 == index))
                {
                    System.Drawing.Bitmap bmp = iconsLarge[index].ToBitmap();
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Position = 0;
                }
                else
                {
                    if (iconsSmall.Length > 0 && (iconsSmall.Length > index || iconsSmall.Length - 1 == index))
                    {
                        iconsSmall[index].ToBitmap().Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                //bitmap = new BitmapImage();
                //bitmap.BeginInit();
                //bitmap.StreamSource = stream;
                //bitmap.CacheOption = BitmapCacheOption.Default;
                //bitmap.EndInit();
                //bitmap.Freeze();

                imageSource = BitmapFrame.Create(stream);

                for (int i = iconsLarge.Length - 1; i >= 0; i--)
                {
                    iconsLarge[i].Dispose();
                    iconsLarge[i] = null;
                }
                for (int i = iconsSmall.Length - 1; i >= 0; i--)
                {
                    iconsSmall[i].Dispose();
                    iconsSmall[i] = null;
                }
                Array.Clear(iconsLarge, 0, iconsLarge.Length);
                Array.Clear(iconsSmall, 0, iconsSmall.Length);

                //stream.Close();

                return imageSource;
            }
        }

        public static ImageSource GetIconImage(string fileName)
        {
            System.Drawing.Image image = new System.Drawing.Bitmap(fileName);
            int w, h;
            w = image.Width;
            h = image.Height;

            if (w > h || w < h)
            {
                if (w > h)
                {
                    double d = w / (double)h;
                    w = 64;
                    h = (int)(w * d);
                }
                else
                {
                    double d = h / w;
                    h = 64;
                    w = (int)(h * d);
                }
            }
            else
            {
                w = h = 64;
            }
            Bitmap bitmap = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            Rectangle src = new Rectangle(0, 0, image.Width, image.Height);
            Rectangle dest = new Rectangle(0, 0, w, h);
            g.DrawImage(image, dest, src, GraphicsUnit.Pixel);

            g.Dispose();
            src = Rectangle.Empty;
            dest = Rectangle.Empty;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            image.Dispose();
            bitmap.Dispose();

            BitmapImage bImage = new BitmapImage();
            bImage.BeginInit();
            bImage.StreamSource = ms;
            bImage.CacheOption = BitmapCacheOption.OnLoad;
            bImage.EndInit();
            bImage.Freeze();

            bitmap.Dispose();
            return bImage;
        }


        public static System.Windows.Media.Color ConvertColor(System.Drawing.Color color)
        {
            System.Windows.Media.Color retColor = new System.Windows.Media.Color();

            retColor.A = color.A;
            retColor.R = color.R;
            retColor.G = color.G;
            retColor.B = color.B;


            return retColor;
        }

        public static string GetSteamGameIcon(string file)
        {
            string[] lines = System.IO.File.ReadAllLines(file);
            string iconPath = Array.Find(lines, l => l.StartsWith("IconFile="));
            iconPath = iconPath.Substring("IconFile=".Length);

            Array.Clear(lines, 0, lines.Length);
            return iconPath;
        }

        private static volatile ImageSource SteamIcon;
        private static bool steamIconSet = false;

        public static ImageSource GetSteamIcon()
        {
            if (!steamIconSet)
            {
                RegistryKey uninstallKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
                if (uninstallKey == null)
                    uninstallKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");

                RegistryKey key = uninstallKey.OpenSubKey("Steam");
                string path = key.GetValue("DisplayIcon").ToString();
                path = path.Substring(0, path.LastIndexOf("\\")); // This is where Steam is installed;

                SteamIcon = IconX.GetIconFromFile(path + "\\Steam.exe");
                steamIconSet = true;
            }


            return SteamIcon;
        }

        public static bool IsSteamApp(string file)
        {
            if (!file.ToLower().EndsWith(".url"))
                return false;

            bool res = false;

            string urlStr = "URL=";

            string[] lines = System.IO.File.ReadAllLines(file);
            string line = Array.Find(lines, l => l.StartsWith(urlStr));
            //URL = steam://rungameid/8500

            if (string.IsNullOrEmpty(line)) goto exit;
            if (line.Substring(urlStr.Length).StartsWith("steam"))
                res = true;


            exit:
            Array.Clear(lines, 0, lines.Length);
            return res;

        }

        public static bool GetAdminFlag(string file)
        {
            bool result = false;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers");
            if (key.GetValueNames().Contains(file))
            {
                result = key.GetValue(file).ToString().Contains("RUNASADMIN");
            }
            key.Close();
            key.Dispose();
            return result;
        }

        public static bool SetAdminFlag(string targetPath)
        {
            bool newValue = !GetAdminFlag(targetPath);
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers", true);
            if (newValue)
                key.SetValue(targetPath, "RUNASADMIN");
            else
                key.DeleteValue(targetPath, false);

            key.Close();
            key.Dispose();

            return newValue;
        }

        public static bool IsFile(string target)
        {
            bool result = false;

            string fileName = "";
            if (target.Contains("\\"))
            {
                fileName = target.Substring(target.LastIndexOf("\\") + 1);
            }
            if (fileName.Contains("."))
            {
                string ext = fileName.Substring(fileName.LastIndexOf("."));
                result = ext.Length == 4;
            }
            return result;
        }

        public static bool IsExeFile(string target)
        {
            return target.ToLower().EndsWith(".exe");
        }

        public static bool IsImageFile(string target)
        {
            bool result = false;
            string s = target.ToLower();

            if (s.EndsWith(".bmp")) result = true;
            if (s.EndsWith(".jpg")) result = true;
            if (s.EndsWith(".png")) result = true;

            s = string.Empty;

            return result;
        }

    }
}


