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
    }
}


