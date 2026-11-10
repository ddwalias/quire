#nullable disable

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Quire;

internal partial class ZoomControlWidthToVisibilityMultiConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values != null && values.Length == 2 && values[0] is double width)
        {
            double? actualWidth = ((values[1] as ContentControl)?.Content as StackPanel)?.ActualWidth;
            if (width < actualWidth)
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

    public ZoomControlWidthToVisibilityMultiConverter()
    {
    }
}
