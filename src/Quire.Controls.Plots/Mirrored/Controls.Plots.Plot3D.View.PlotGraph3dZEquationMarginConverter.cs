using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Quire.Controls.Plots.Plot3D.View;
using Quire.Wpf;

namespace Quire.Controls.Plots.Plot3D.View;

[ValueConversion(typeof(bool), typeof(Thickness))]
[ValueConversion(typeof(Size), typeof(Thickness))]
public partial class PlotGraph3dZEquationMarginConverter : IMultiValueConverter
{
    public static readonly PlotGraph3dZEquationMarginConverter Instance = new PlotGraph3dZEquationMarginConverter();

    private PlotGraph3dZEquationMarginConverter()
    {
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        bool includeAxesWidgetMargin = (bool)values[0];
        Size size = (Size)values[1];
        Thickness thickness = new Thickness
        {
            Left = size.Width,
            Top = 7.0,
            Right = WpfUtils.InchesToDips(0.1),
            Bottom = WpfUtils.InchesToDips(0.1)
        };

        if (includeAxesWidgetMargin)
        {
            thickness.Right += PlotControl3D.AxesWidgetMargin.Left + PlotControl3D.AxesWidgetMargin.Right;
        }

        return thickness;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
