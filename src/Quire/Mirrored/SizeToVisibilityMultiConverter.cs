#nullable disable

using System;
using System.Globalization;
using System.Windows;

namespace Quire;

internal partial class SizeToVisibilityMultiConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values != null && values.Length == 2 && values[0] is double num && values[1] is double num2 && parameter is string text)
        {
            string[] array = text.Split(',');
            if ((double.TryParse(array[0], out var result) && num < result) || (array.Length == 2 && double.TryParse(array[1], out var result2) && num2 > result2))
            {
                return Visibility.Hidden;
            }
        }

        return Visibility.Visible;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
