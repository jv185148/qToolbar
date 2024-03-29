﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace qData
{
    public abstract class aFile : IDisposable
    {
        public abstract string FileName { get; set; }

        private Field[] fields;

        public abstract Field[] FieldData { get; }
        protected abstract void PrepData();
        protected abstract void LoadData();
        protected abstract void LoadDefaults();

        protected abstract void childDispose();

        public void Save()
        {
            string temp = FileName + ".tmp";
            System.IO.FileStream fs = new System.IO.FileStream(temp, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
            PrepData();

            StringBuilder sb = new StringBuilder();
            foreach (Field f in FieldData)
            {
                sb.AppendLine(f.Title + "=" + f.Data);
            }

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            sb.Clear();
            sb = null;

            fs.Write(buffer, 0, buffer.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();

            Array.Clear(buffer, 0, buffer.Length);
            if (System.IO.File.Exists(FileName))
                System.IO.File.Delete(FileName);
            System.IO.File.Move(temp, FileName);
        }


        public void Load()
        {
            if (!System.IO.File.Exists(FileName))
            {
                goto Error;
            }

            while (fileLocked())
            {
                //do nothing
            }
            System.IO.FileStream fs = new System.IO.FileStream(FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            fs.Dispose();

            string data = Encoding.UTF8.GetString(buffer);
            Array.Clear(buffer, 0, buffer.Length);
            data = data.Replace("\n", "");

            string[] lines = data.Split('\r');
            for (int i = 0; i < FieldData.Length; i++)
            {
                try
                {

                    FieldData[i].Data = lines[i].Split('=')[1];
                }
                catch (Exception ex)
                {
                    goto Error;
                }
            }
            LoadData();

            return;

        Error:
            LoadDefaults();
            return;
        }


        private bool fileLocked()
        {
            bool isLocked = true;
            bool secondTime = false;

            System.IO.FileStream fs = null;
            try
            {
                fs = System.IO.File.Open(FileName, System.IO.FileMode.Open);
                return false;

            }
            catch (System.IO.IOException e)
            {
                var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);
                isLocked = errorCode == 32 || errorCode == 33;

                if (!secondTime)
                {
                    secondTime = true;
                    new System.Threading.ManualResetEvent(false).WaitOne(500);

                }
            }
            finally
            {
                fs?.Close();
                fs?.Dispose();
            }

            return isLocked;
        }



        public void Dispose()
        {
            childDispose();
        }

    }


    public class Field
    {
        string title;
        string data;

        public string Title { get => title; set => title = value; }
        public string Data { get => data; set => data = value; }

        public override string ToString()
        {
            return title + "=" + data + ";";
        }
    }
}
