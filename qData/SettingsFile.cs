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

        public override Field[] FieldData => throw new NotImplementedException();

        protected override void childDispose()
        {
            throw new NotImplementedException();
        }

        public static Brush SelectedTileColor => Brushes.BlueViolet;
    }
}
