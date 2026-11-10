#nullable disable

using System;
using System.Globalization;

namespace Quire;

public partial class StyleFontSizeToStringConverter
{
    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((string)value == string.Empty)
        {
            return null;
        }

        return base.ConvertBack(value, targetType, parameter, culture);
    }
}
