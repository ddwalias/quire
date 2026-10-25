#nullable disable
using System.Windows.Media;

namespace Quire.Controls.Core;

public static partial class BackgroundTypeColors
{
    public static Color Paper = new Color
    {
        A = byte.MaxValue,
        B = 246,
        G = 250,
        R = 246
    };

    public static Color Gray = new Color
    {
        A = byte.MaxValue,
        B = 192,
        G = 192,
        R = 192
    };
}
