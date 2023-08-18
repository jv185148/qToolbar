using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace qCommon
{
    public class Events
    {

        public delegate void RightClickHandler(object sender);
        public delegate void TimerDelegate();

        public delegate void dClicked(object sender);
        public delegate void dDragging(object sender);
        public delegate void dDraggingDone(object sender, System.Windows.Input.MouseEventArgs e);
            
    }
}
