using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qCommon.Interfaces
{
    public interface iShortcut
    {
        string Description { get; set; }
        string IconLocation { get; set; }
        string TargetPath { get; set; }
        string WorkingDirectory { get; set; }
    }
}
