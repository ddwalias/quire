#nullable disable

using System;
using System.Globalization;

namespace Quire;

public partial class IntToStringConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }

        return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string text)
        {
            return int.Parse(text);
        }

        return null;
    }
}
