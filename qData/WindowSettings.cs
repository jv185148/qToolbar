using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qData
{
    public class WindowSettings : aFile, qCommon.Interfaces.iWindowSettings
    {

        #region iWindowSettings properties

        public string background { get; set; }
        public System.Windows.Point iWindowPosition { get; set; }
        public System.Windows.Point iWindowSize { get; set; }

        public string iBackgroundColor { get; set; }
        #endregion

        string appPath = System.AppDomain.CurrentDomain.BaseDirectory;

        string _fileName;

        public override string FileName { get => _fileName; set => _fileName = value; }

        #region Field definitions

        private Field[] fields;
        public override Field[] FieldData => fields;

        private Field backgroundField;
        private Field windowSizeField;
        private Field windowPositionField;
        private Field backgroundColorField;

        #endregion

        public WindowSettings(string fileName)
        {
            fields = new Field[4];

            _fileName = appPath + "\\ShortcutCollections\\" + fileName.Replace(".qtb", ".dat");

            backgroundField = new Field() { Title = "Background" };
            backgroundColorField = new Field() { Title = "BackgroundColor" };
            windowSizeField = new Field() { Title = "WindowSize" };
            windowPositionField = new Field() { Title = "WindowPosition" };

            fields[0] = backgroundField;
            fields[1] = backgroundColorField;
            fields[2] = windowSizeField;
            fields[3] = windowPositionField;
        }

        protected override void PrepData()
        {
            backgroundField.Data = background;
            backgroundColorField.Data = iBackgroundColor;
            windowSizeField.Data = string.Format("{0};{1}", iWindowSize.X, iWindowSize.Y);
            windowPositionField.Data = string.Format("{0};{1}", iWindowPosition.X, iWindowPosition.Y);
        }

        protected override void childDispose()
        {
            Array.Clear(fields, 0, fields.Length);
            fields = null;
        }

        protected override void LoadData()
        {
            background = backgroundField.Data;
            iBackgroundColor = backgroundColorField.Data;

            double x, y;
            string[] s = null;

            s = windowSizeField.Data.Split(';');
            x = 0; y = 0;

            double.TryParse(s[0], out x);
            double.TryParse(s[1], out y);

            if (x == 0) x = 400;
            if (y == 0) y = 400;

            iWindowSize = new System.Windows.Point(x, y);

            x = 0; y = 0;
            s = windowPositionField.Data.Split(';');
            double.TryParse(s[0], out x);
            double.TryParse(s[1], out y);

            iWindowPosition = new System.Windows.Point(x, y);

        }

        protected override void LoadDefaults()
        {
            background = "";
            iBackgroundColor = "255,255,255,255";
            iWindowPosition = new System.Windows.Point(0, 0);
            iWindowSize = new System.Windows.Point(400, 400);
        }


    }
}
