#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Quire.Controls.Plots;

internal partial class LimitLabelVisibilityMultiConverter
{
    public static readonly LimitLabelVisibilityMultiConverter Instance = new LimitLabelVisibilityMultiConverter();

    public const double OpacityVisible = 1.0;

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert(values) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    protected virtual bool Convert(object[] values)
    {
        double num = (double)values[0];
        bool flag = (bool)values[1];
        bool flag2 = (bool)values[2];
        return (1.0 != num && !flag) & flag2;
    }

    protected LimitLabelVisibilityMultiConverter()
    {
    }
}
