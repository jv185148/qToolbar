using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qCommon.Interfaces
{
    public interface iMain
    {
        System.Windows.Controls.WrapPanel iWrapPanel { get; }
        iSettings iSettingsForm { get; }
        System.Windows.Controls.Grid iGrid { get; }


        void Dispose();
    }
}


