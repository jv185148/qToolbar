using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qCommon.Interfaces
{
    public interface iMain
    {
        System.Windows.Controls.WrapPanel iGridArea { get; }
        iSettings iSettingsForm { get; }

        void Dispose();
    }
}
    

