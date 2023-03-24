using System.Windows.Media;

namespace qCommon.Interfaces
{
    public interface iSettings
    {
         Brush TextColor { get; set; }
         Brush SelectColor { get; set; }

        bool RunWithSingleClick { get; set; }
    }
}
