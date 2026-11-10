#nullable disable

using System;
using System.Globalization;

namespace Quire;

public partial class FormatPlotValuesRowConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isTickMarkValues = value.Equals("Ribbon.Tab.Plots.FormatTickMarkValues");
        if (parameter.Equals("displayPrecision"))
        {
            return isTickMarkValues ? 1 : 2;
        }

        if (parameter.Equals("ShowTrailingZeros"))
        {
            return isTickMarkValues ? 3 : 0;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
