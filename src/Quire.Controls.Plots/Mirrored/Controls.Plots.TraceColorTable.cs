#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;

namespace Quire.Controls.Plots;

public partial class TraceColorTable
{
    public static TraceColorTable Default = new TraceColorTable();

    private TraceColorTable()
    {
        Add(Brushes.DarkBlue);
        Add(Brushes.Black);
        Add(Brushes.Red);
        Add(Brushes.Yellow);
        Add(Brushes.Orange);
        Add(Brushes.Green);
        Add(Brushes.Blue);
    }

    public static Color parseColor(string color)
    {
        return Color.FromArgb(byte.Parse(color.Substring(1, 2), NumberStyles.HexNumber), byte.Parse(color.Substring(3, 2), NumberStyles.HexNumber), byte.Parse(color.Substring(5, 2), NumberStyles.HexNumber), byte.Parse(color.Substring(7, 2), NumberStyles.HexNumber));
    }
}
