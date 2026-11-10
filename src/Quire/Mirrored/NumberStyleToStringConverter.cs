#nullable disable

using System;
using System.Globalization;
using Quire.Controls.Core;

namespace Quire;

public partial class NumberStyleToStringConverter
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
        switch (value as string)
        {
            case "Decimal":
                return NumberFormat.NumberStyles.Decimal;
            case "Scientific":
                return NumberFormat.NumberStyles.Scientific;
            case "Engineering":
                return NumberFormat.NumberStyles.Engineering;
            case "Percent":
                return NumberFormat.NumberStyles.Percent;
            case "General":
                return NumberFormat.NumberStyles.General;
            default:
                return null;
        }
    }
}
