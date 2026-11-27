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

        if (values[1] is bool hasExplicitState && hasExplicitState)
        {
            return values[2] is bool explicitStateChecked && explicitStateChecked;
        }

        return values[0] is bool isChecked && isChecked;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not supported for SubscriptStateToCheckedConverter");
    }

    public SubscriptStateToCheckedConverter()
    {
    }
}
