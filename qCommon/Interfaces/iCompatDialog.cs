using q;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace qCommon.Interfaces
{
    public interface iCompatDialog
    {
        event qCommon.Events.dClicked AcceptClicked;
        event qCommon.Events.dClicked CancelClicked;
        bool UseCompat { get; set; }
        ComboBox comboBox { get; }

        string targetFile { get; set; }

        q.Common.CompatFlags flag { get; set; }

        bool? DialogResult { get; set; }
        Common.CompatFlags CurrentFlag { get; set; }

        bool? ShowDialog();
    }
}
