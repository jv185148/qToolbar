using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qData
{
    public abstract class aFile
    {
        public abstract string FileName { get; }

        private Field[] fields;

        public abstract Field[] FieldData { get; }
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
