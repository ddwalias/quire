#nullable disable

using System;
using System.Globalization;

namespace Quire;

public partial class StringToTextIndent
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == string.Empty)
        {
            return null;
        }

        return value;
    }
}
