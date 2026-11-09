#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Quire.Controls.Plots.Plot3D.View;

public partial class PlotGraph3DContainerParentMarginConverter
{
    public static readonly PlotGraph3DContainerParentMarginConverter Instance = new PlotGraph3DContainerParentMarginConverter();

    private PlotGraph3DContainerParentMarginConverter()
    {
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        bool flag = (bool)values[0];
        Size val = (Size)values[1];
        return new Thickness
        {
            Left = val.Width,
            Top = val.Height,
            Right = val.Width,
            Bottom = (flag ? 0.0 : val.Height),
        };
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
