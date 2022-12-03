using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace qCommon
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
                System.Drawing.Bitmap bmp = Bitmap.FromHicon(iconsLarge[index].Handle); //iconsLarge[index].ToBitmap();
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png );
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
}
