#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Quire.Controls.Plots.View;

[ValueConversion(typeof(Color), typeof(SolidColorBrush))]
public partial class ColorToSolidColorBrushConverter
{
    public static readonly ColorToSolidColorBrushConverter Instance = new ColorToSolidColorBrushConverter();

    private ColorToSolidColorBrushConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        SolidColorBrush solidColorBrush = null;
        if (value is Color)
        {
            solidColorBrush = new SolidColorBrush((Color)value);
            solidColorBrush.Freeze();
        }
        if (solidColorBrush == null && parameter is SolidColorBrush)
        {
            solidColorBrush = parameter as SolidColorBrush;
        }
        return solidColorBrush ?? DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is SolidColorBrush))
        {
            return DependencyProperty.UnsetValue;
        }
        return ((SolidColorBrush)value).Color;
    }
}
