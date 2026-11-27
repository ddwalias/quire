#nullable disable

using System;
using System.Globalization;
using System.Windows;

namespace Quire;

internal partial class SizeToVisibilityMultiConverter
{
    public SizeToVisibilityMultiConverter() { }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values != null && values.Length == 2 && values[0] is double firstSize && values[1] is double secondSize && parameter is string text)
        {
            string[] bounds = text.Split(',');
            if ((double.TryParse(bounds[0], out var minimumSize) && firstSize < minimumSize) || (bounds.Length == 2 && double.TryParse(bounds[1], out var maximumSize) && secondSize > maximumSize))
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
