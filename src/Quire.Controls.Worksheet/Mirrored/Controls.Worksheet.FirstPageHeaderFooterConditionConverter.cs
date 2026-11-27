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

        if (values[0] is bool hasHeaderFooter && values[1] is int pageNumber && values[2] is bool isFirstPage)
        {
            return (hasHeaderFooter && pageNumber == 1) || !isFirstPage;
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
