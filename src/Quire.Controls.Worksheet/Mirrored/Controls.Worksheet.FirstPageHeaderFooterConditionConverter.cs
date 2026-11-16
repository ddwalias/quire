using System;
using System.Globalization;

namespace Quire.Controls.Worksheet;

public partial class FirstPageHeaderFooterConditionConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 3)
        {
            return false;
        }

        if (values[0] is bool flag && values[1] is int num && values[2] is bool flag2)
        {
            return (flag && num == 1) || !flag2;
        }

        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("FirstPageHeaderFooterConditionConverter does not support backward conversion");
    }

    public FirstPageHeaderFooterConditionConverter()
    {
    }
}
