using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qMain
{
    public class Main
    {

        public qCommon.Interfaces.iMain child;

        public Main(qCommon.Interfaces.iMain child)
        {
            this.child = child;
        }

        public void Load()
        {
            throw new NotImplementedException();
        }
    }
}
