using System.Windows.Media;

namespace qCommon.Interfaces
{
    public interface iSettings
    {
        Brush ForegroundColor { get; set; }
        Brush ForegroundSelectColor { get; set; }
        Brush SelectColor { get; set; }

        bool RunWithSingleClick { get; set; }

        bool OpenAllShortcutFiles { get; set; }

        bool ShowBorders { get; set; }
        bool ShowBorderForMain { get; set; }

    }
}
