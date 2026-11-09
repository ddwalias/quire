#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Quire.Controls.Plots;

internal partial class ColorMapZToBrush
{
    internal static readonly ColorMapZToBrush Instance = new ColorMapZToBrush();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // Mathcad assigns one result local on every path and returns it once, which sets the branch widths.
        object result;
        if (values.Length > 2 && values[0] is IColorMap && values[1] is Interval && values[2] is double)
        {
            IColorMap colorMap = values[0] as IColorMap;
            Interval interval = (Interval)values[1];
            double num = (double)values[2];
            if (interval.Contains(num))
            {
                double num2 = (double)values[3];
                if (2.0 == num2)
                {
                    result = null;
                }
                else
                {
                    result = colorMap.GetBrush(interval.Normalize(num) + num2 * double.Epsilon);
                }
            }
            else
            {
                result = DependencyProperty.UnsetValue;
            }
        }
        else
        {
            result = DependencyProperty.UnsetValue;
        }
        return result;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private ColorMapZToBrush()
    {
    }
}
