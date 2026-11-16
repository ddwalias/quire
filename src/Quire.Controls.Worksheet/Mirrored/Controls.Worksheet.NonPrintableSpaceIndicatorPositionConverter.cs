using System;
using System.Globalization;
using System.Windows;

namespace Quire.Controls.Worksheet;

public partial class NonPrintableSpaceIndicatorPositionConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new Thickness(0.0, (double)value, 0.0, 0.0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public NonPrintableSpaceIndicatorPositionConverter()
    {
    }
}
