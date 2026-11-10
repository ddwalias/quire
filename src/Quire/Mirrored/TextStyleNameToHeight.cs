#nullable disable

using System;
using System.Globalization;

namespace Quire;

public partial class TextStyleNameToHeight
{
    private const int ControlHeightStyleIsNone = 24;
    private const int ControlHeightStyleIsNotNone = 52;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((string)value == "(none)")
        {
            return ControlHeightStyleIsNone;
        }

        return ControlHeightStyleIsNotNone;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
