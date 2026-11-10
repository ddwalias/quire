#nullable disable

using System;
using System.Globalization;

namespace Quire;

internal partial class PageBackgroundColorMatchToBooleanConverter
{
    public static PageBackgroundColorMatchToBooleanConverter Instance = new PageBackgroundColorMatchToBooleanConverter();

    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return false;
        }

        if (parameter.ToString() == value.ToString())
        {
            return true;
        }

        return false;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
