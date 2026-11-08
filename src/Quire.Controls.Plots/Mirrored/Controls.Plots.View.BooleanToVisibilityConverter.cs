#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Quire.Controls.Plots.View;

[ValueConversion(typeof(bool), typeof(Visibility))]
internal partial class BooleanToVisibilityConverter
{
    internal static readonly BooleanToVisibilityConverter Instance = new BooleanToVisibilityConverter();

    private BooleanToVisibilityConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool && (bool)value)
        {
            return Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility && (Visibility)value == Visibility.Visible;
    }
}
