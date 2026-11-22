using System;
using System.Globalization;

namespace Quire.Controls.Worksheet;

public partial class CombinedInvalidConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        bool first = values[0] is bool f && f;
        bool second = values[1] is bool s && s;
        return first & second;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
