#nullable disable

using System;
using System.Globalization;
using System.Windows.Data;

namespace Quire;

public partial class SubscriptStateToCheckedConverter : IMultiValueConverter
{
    public static readonly SubscriptStateToCheckedConverter Instance = new SubscriptStateToCheckedConverter();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 3)
        {
            return false;
        }

        if (values[1] is bool flag && flag)
        {
            return values[2] is bool flag2 && flag2;
        }

        return values[0] is bool flag3 && flag3;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not supported for SubscriptStateToCheckedConverter");
    }

    public SubscriptStateToCheckedConverter()
    {
    }
}
