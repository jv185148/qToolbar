using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qCommon.Interfaces
{
    public interface iEditShortcutW
    {
        string LoadedShortcut { get; set; }
        bool LoadedShortcutChanged { get;  }
    }
}
