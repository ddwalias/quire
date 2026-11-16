using System.Windows.Media;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public static partial class PrintGridSettings
{
    public static double MajorLinesThickness = 1.0;
    public static double MinorLinesThickness = 0.8;
    public static Color MajorLinesColor = WpfUtils.WpfColorFromName("#FFBBDDFE");
    public static Color MinorLinesColor = WpfUtils.WpfColorFromName("#FFDDEEFE");
    public static Color FrameColor = WpfUtils.WpfColorFromName("Black");
}
