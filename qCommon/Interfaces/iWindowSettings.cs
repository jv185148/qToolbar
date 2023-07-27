using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qCommon.Interfaces
{
    public interface iWindowSettings
    {
        string background { get; set; }
        string iBackgroundColor { get; set; }
        System.Windows.Point iWindowPosition { get; set; }
        System.Windows.Point iWindowSize { get; set; }

    }
}
