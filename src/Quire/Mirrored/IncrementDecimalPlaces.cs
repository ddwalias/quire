#nullable disable

using System;
using System.Globalization;
using Quire.Controls.Core;

namespace Quire;

public partial class IncrementDecimalPlaces
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is int) || !(parameter is int))
        {
            return value;
        }

        return NumberFormat.CoerceDecimalPlaces((int)value, (int)parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
