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

namespace qData
{
    public class Icons
    {
        public static string GetAssociatedProgramName(string ext)
        {
            string program = "";

            //program = FindProgramName(ext.ToLower(), Registry.CurrentUser.OpenSubKey("Software\\Classes"));

            //program = FindProgramName(ext.ToLower(), Registry.LocalMachine.OpenSubKey("Software\\Classes"));

            // Find the userchoice program first.
            program = FindUserChoice(ext.ToLower());//
            if (program == "")
                program = FindPcDefault(ext.ToLower());

            if (program == "") return null;


            return program;
        }

        private static string FindUserChoice(string ext)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts");
            RegistryKey subkey = key.OpenSubKey(ext);

            string value = "";
            object o = null;
            string appId = "";

            if (subkey == null)
                return "";

            string[] keyNames = subkey.GetSubKeyNames();
            if (!keyNames.Contains("UserChoice"))
            {
                goto Close;
            }

            o = subkey.OpenSubKey("UserChoice").GetValue("ProgId").ToString();
            appId = o.ToString();
            if (appId.StartsWith("Applications\\"))
            {
                appId = appId.Substring("Applications\\".Length);

            }
            if (appId == "")
                goto Close;

            key.Close();
            subkey.Close();

            key = Registry.ClassesRoot.OpenSubKey("Applications");
            subkey = key.OpenSubKey(appId + "\\shell\\open\\command");

            if (subkey == null)
                goto Close;
            o = subkey.GetValue("");
            value = o.ToString();


            if (value.Contains("\""))
            {
                value = value.Substring(1, value.IndexOf("\"", 1));
                value = value.Substring(0, value.Length - 1);
            }


        Close:
            subkey?.Close();
            subkey?.Dispose();

            key.Close();
            key.Dispose();

            o = null;

            return value;

        }

        private static string FindPcDefault(string ext)
        {
            if (ext == "")
                return "";

            RegistryKey key = Registry.ClassesRoot.OpenSubKey(ext);

            string value = "";
            string appName = "";
            if (key == null)
                goto Close;

            appName = key.GetValue("").ToString();

            key.Close();
            key = Registry.ClassesRoot.OpenSubKey(appName + "\\shell\\open\\command");

            value = key.GetValue("").ToString();

            if (value.Contains("\""))
                value = value.Substring(1, value.IndexOf("\"", 1) - 1);


            if (value.Contains("%"))
                value = value.Substring(0, value.IndexOf("%"));
  
        Close:
            key.Close();
            key.Dispose();
            return value;
        }
    }
}