#nullable disable

using System;
using System.Globalization;
using System.Windows;

namespace Quire;

public partial class FormatPlotValueVisiblityConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.Equals("Ribbon.Tab.Plots.FormatTickMarkValues") ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
