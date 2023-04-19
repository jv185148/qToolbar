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
        Field fForegroundSelect;
        Field fRunWithSingleClick;

        public  Brush SelectedTileColor { get; set; }
        public Brush ForegroundSelectColor { get; set; }
        public  Brush ForegroundColor { get; set; }
        public bool RunWithSingleClick { get; set; }

        public SettingsFile()
        {
            fTileColor = new Field() { Title = "TileColor" };
            fForeground = new Field() { Title = "Fourground" };
            fForegroundSelect = new Field() { Title = "ForegroundSelect" };
            fRunWithSingleClick = new Field() { Title = "RunWithSingleClick" };

            fields = new Field[4];
            fields[0] = fTileColor;
            fields[1] = fForeground;
            fields[2] = fForegroundSelect;
            fields[3] = fRunWithSingleClick;
        }

        protected override void PrepData()
        {
            fTileColor.Data = SelectedTileColor.ToString();
            fForeground.Data = ForegroundColor.ToString();
            fForegroundSelect.Data = ForegroundSelectColor.ToString();
            fRunWithSingleClick.Data = RunWithSingleClick.ToString();
        }

        protected override void LoadData()
        {
            BrushConverter bc = new BrushConverter();
            Brush b=(Brush) bc.ConvertFromString(fTileColor.Data);
            SelectedTileColor = b.Clone();

            if (fForegroundSelect.Data == null)
                fForegroundSelect.Data = "#000000";

            b = (Brush)bc.ConvertFromString(fForegroundSelect.Data);
            ForegroundSelectColor = b.Clone();

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
            ForegroundSelectColor = Brushes.Black;

            RunWithSingleClick = true;
        }

        protected override void childDispose()
        {
            Array.Clear(fields, 0, fields.Length);
        }
    }
}
