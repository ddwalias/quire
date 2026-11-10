#nullable disable

using System;
using System.Globalization;
using Quire.Controls.Core;

namespace Quire;

public partial class RadixStyleToStringMultiConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || !(values[0] is NumberFormat.RadixStyles) || !(values[1] is NumberFormat.NumberStyles))
        {
            return null;
        }

        string result = values[0].ToString();
        string text = values[1].ToString();
        if (text == NumberFormat.NumberStyles.General.ToString() || text == NumberFormat.NumberStyles.Decimal.ToString())
        {
            return result;
        }

        return NumberFormat.RadixStyles.Decimal.ToString();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
