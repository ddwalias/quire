#nullable disable

using System;
using System.Globalization;

namespace Quire;

public partial class FormatPlotValuesColumnConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter.Equals("ShowTrailingZeros"))
        {
            return !value.Equals("Ribbon.Tab.Plots.FormatTickMarkValues") ? 1 : 0;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
