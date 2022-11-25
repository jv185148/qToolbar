using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qData
{
    public class SettingsFile : aFile
    {
        public override string FileName => "settings.ini";

        public override Field[] FieldData => throw new NotImplementedException();
    }
}
