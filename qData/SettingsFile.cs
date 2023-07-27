using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace qData
{
    public class SettingsFile : aFile, qCommon.Interfaces.iSettings
    {
        string fileName = "settings.ini";
        public override string FileName { get => fileName; set {/*Do Nothing*/ } }

        private Field[] fields;
        public override Field[] FieldData => fields;


        Field fTileColor;
        Field fForeground;
        Field fForegroundSelect;
        Field fRunWithSingleClick;
        Field fOpenAllShortcuts;

        public Brush SelectColor { get; set; }
        public Brush ForegroundSelectColor { get; set; }
        public Brush ForegroundColor { get; set; }
        public bool RunWithSingleClick { get; set; }
        public bool OpenAllShortcutFiles { get; set; }

        public SettingsFile()
        {
            fTileColor = new Field() { Title = "TileColor" };
            fForeground = new Field() { Title = "Fourground" };
            fForegroundSelect = new Field() { Title = "ForegroundSelect" };
            fRunWithSingleClick = new Field() { Title = "RunWithSingleClick" };
            fOpenAllShortcuts = new Field() { Title = "OpenAllShortcutsOnStartup" };

            fields = new Field[5];
            fields[0] = fTileColor;
            fields[1] = fForeground;
            fields[2] = fForegroundSelect;
            fields[3] = fRunWithSingleClick;
            fields[4] = fOpenAllShortcuts;
        }

        protected override void PrepData()
        {
            fTileColor.Data = SelectColor.ToString();
            fForeground.Data = ForegroundColor.ToString();
            fForegroundSelect.Data = ForegroundSelectColor.ToString();
            fRunWithSingleClick.Data = RunWithSingleClick.ToString();
            fOpenAllShortcuts.Data = OpenAllShortcutFiles.ToString();
        }

        protected override void LoadData()
        {
            BrushConverter bc = new BrushConverter();
            Brush b = (Brush)bc.ConvertFromString(fTileColor.Data);
            SelectColor = b.Clone();

            if (fForegroundSelect.Data == null)
                fForegroundSelect.Data = "#000000";

            b = (Brush)bc.ConvertFromString(fForegroundSelect.Data);
            ForegroundSelectColor = b.Clone();

            b = (Brush)bc.ConvertFromString(fForeground.Data);
            ForegroundColor = b.Clone();

            bool singleClick = false;
            bool.TryParse(fRunWithSingleClick.Data, out singleClick);
            RunWithSingleClick = singleClick;

            bool openAllShortcuts;
            bool.TryParse(fOpenAllShortcuts.Data, out openAllShortcuts);
            OpenAllShortcutFiles = openAllShortcuts;

            b = null;
            bc = null;
        }

        protected override void LoadDefaults()
        {
            ForegroundColor = Brushes.Black;
            SelectColor = Brushes.SteelBlue;
            ForegroundSelectColor = Brushes.Black;

            RunWithSingleClick = true;

            OpenAllShortcutFiles = true;
        }

        protected override void childDispose()
        {
            Array.Clear(fields, 0, fields.Length);
        }
    }
}
