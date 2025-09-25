using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace qCommon.Interfaces
{

    public interface iMain
    {
        bool isMain { get; set; }
        System.Windows.Controls.WrapPanel iWrapPanel { get; }
        iSettings iSettingsForm { get; }
        System.Windows.Controls.Grid iGrid { get; }
        iOpenW iOpenWindow { get; }
        iEditShortcutW iEditShortcutWindow { get; }
        string iShortcutFile { get; set; }

        iCompatDialog compatDialog { get; }

        int iShortcutCount { get; set; }
        bool iDoubleClickToRun { get; set; }
        void Dispose();
        string iBackground { get; set; }
        System.Drawing.Color iBackgroundColor { get; set; }
        System.Windows.Point iSize { get; set; }
        System.Windows.Point iPosition { get; set; }

        bool iShowBorder { get; set; }
        bool OpacityWait { get; set; }

        void RequestOpacityChange_WaitOn(Window window);

        double lowOpacityValue { get; set; }
        void UpdateTitle();
        void SetLowOpacity();
        void SetNormalOpacity();



        event Events.RightClickHandler RightClicked;
        event Events.WindowCloseEvent WaitOnOpacityRequest;
     }
}


