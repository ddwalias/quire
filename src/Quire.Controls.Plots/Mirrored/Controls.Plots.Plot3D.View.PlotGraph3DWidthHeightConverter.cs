#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows.Data;

namespace Quire.Controls.Plots.Plot3D.View;

public partial class PlotGraph3DWidthHeightConverter
{
    public static readonly PlotGraph3DWidthHeightConverter Instance = new PlotGraph3DWidthHeightConverter();

    private PlotGraph3DWidthHeightConverter()
    {
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return Math.Min((double)values[0], (double)values[1]);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
