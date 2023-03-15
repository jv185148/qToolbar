using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace qData
{
    public class SettingsFile : aFile
    {
        public override string FileName => "settings.ini";

        private Field[] fields;
        public override Field[] FieldData => fields;


        Field fTileColor;
        Field fForeground;

        public  Brush SelectedTileColor { get; set; }

        public  Brush ForegroundColor { get; set; }


        public SettingsFile()
        {
            fTileColor = new Field() { Title = "TileColor" };
            fForeground = new Field() { Title = "Fourground" };

            fields = new Field[2];
            fields[0] = fTileColor;
            fields[1] = fForeground;
           
        }

        protected override void PrepData()
        {
            fTileColor.Data = SelectedTileColor.ToString();
            fForeground.Data = ForegroundColor.ToString();
        }

        protected override void LoadData()
        {
            BrushConverter bc = new BrushConverter();
            Brush b=(Brush) bc.ConvertFromString(fTileColor.Data);
            SelectedTileColor = b.Clone();

            b = (Brush)bc.ConvertFromString(fForeground.Data);
            ForegroundColor = b.Clone();

            b = null;
            bc = null;
        }

        protected override void LoadDefaults()
        {
            ForegroundColor= Brushes.Black;
            SelectedTileColor = Brushes.SteelBlue;
        }

        protected override void childDispose()
        {
            Array.Clear(fields, 0, fields.Length);
        }
    }
}
