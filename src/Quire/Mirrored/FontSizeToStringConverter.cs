#nullable disable

using System;
using System.Globalization;
using System.Windows;

namespace Quire;

public partial class FontSizeToStringConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double && double.IsNaN((double)value))
        {
            return DependencyProperty.UnsetValue;
        }

        return base.Convert(value, targetType, parameter, culture);
    }
}
