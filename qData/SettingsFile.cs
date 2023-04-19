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


        Field fSelectColor;
        Field fForeground;
        Field fRunWithSingleClick;

        public  Brush SelectedTileColor { get; set; }

        public  Brush ForegroundColor { get; set; }
        public bool RunWithSingleClick { get; set; }

        public SettingsFile()
        {
            fSelectColor = new Field() { Title = "TileColor" };
            fForeground = new Field() { Title = "Fourground" };
            fRunWithSingleClick = new Field() { Title = "RunWithSingleClick" };

            fields = new Field[3];
            fields[0] = fSelectColor;
            fields[1] = fForeground;
            fields[2] = fRunWithSingleClick;
        }

        protected override void PrepData()
        {
            fSelectColor.Data = SelectedTileColor.ToString();
            fForeground.Data = ForegroundColor.ToString();
            fRunWithSingleClick.Data = RunWithSingleClick.ToString();
        }

        protected override void LoadData()
        {
            BrushConverter bc = new BrushConverter();
            Brush b=(Brush) bc.ConvertFromString(fSelectColor.Data);
            SelectedTileColor = b.Clone();

            b = (Brush)bc.ConvertFromString(fForeground.Data);
            ForegroundColor = b.Clone();

            bool singleClick = false;
            bool.TryParse(fRunWithSingleClick.Data, out singleClick);
            RunWithSingleClick = singleClick;

            b = null;
            bc = null;
        }

        protected override void LoadDefaults()
        {
            ForegroundColor= Brushes.Black;
            SelectedTileColor = Brushes.SteelBlue;
            RunWithSingleClick = true;
        }

        protected override void childDispose()
        {
            Array.Clear(fields, 0, fields.Length);
        }
    }
}
