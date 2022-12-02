using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qData
{
    public abstract class aFile:IDisposable
    {
        public abstract string FileName { get; }

        private Field[] fields;

        public abstract Field[] FieldData { get; }
        protected abstract void childDispose();

        public void Save()
        {
            System.IO.FileStream fs = new System.IO.FileStream(FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);

        }
        public void Load()
        {

        }
        
        public  void Dispose()
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
